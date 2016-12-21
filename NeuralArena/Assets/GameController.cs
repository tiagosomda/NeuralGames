using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public int matchTimeInSeconds;
    public GameObject gladiatorPrefab;

    public Transform positions;
    public Transform gladiatorParent;

    private Gladiator[] agents;
    private Transform[] startPos;
    private int simCount;

    private Learner learner;
    private bool isLearning;

    private DateTime roundTimeout;

    public void Awake()
    {
        learner = GetComponent<Learner>();
        simCount = positions.transform.childCount;
        agents = new Gladiator[positions.transform.childCount];
        startPos = new Transform[positions.transform.childCount];

        for(int i = 0; i < simCount; i++)
        {
            var obj = Instantiate(gladiatorPrefab);
            obj.transform.SetParent(gladiatorParent);

            agents[i] = obj.GetComponent<Gladiator>();
            startPos[i] = positions.transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartGame();
            learner.iteration = 0;
        }
    }

    public void FixedUpdate()
    {
        if (!isLearning)
        {
            return;
        }

        // we we reach the max of iterations
        // stop running the simulation
        if (learner.iteration >= learner.maxIterations)
        {
            GameOver();
            return;
        }

        // Activate the brain for
        // all the agents running
        foreach (var agent in agents)
        {
            agent.ActivateBrain();
        }

        // as long as there is an agent running
        // we end the game loop here
        if (AnyAgentRunning() && DateTime.UtcNow < roundTimeout)
        {
            return;
        }

        // no one is running
        // start new generation
        if (isLearning)
        {
            learner.iteration++;

            Debug.Log("New Iteration");
            PrintMaxScore();
            GameOver();

            learner.NextGen(agents);

            StartGame();
        }
    }

    public bool AnyAgentRunning()
    {
        foreach (var agent in agents)
        {
            if (agent.isRunning)
            {
                return true;
            }
        }

        return false;
    }

    public void StartGame()
    {
        SetGladiatorInitialPositions();
        roundTimeout = DateTime.UtcNow.AddSeconds(matchTimeInSeconds);
        isLearning = true;

        for(int i = 0; i < simCount; i++)
        {
            agents[i].gameObject.SetActive(true);
            agents[i].isRunning = true;
            agents[i].isOperative = true;
        }
    }

    public void PrintMaxScore()
    {
        var max = 0;
        var min = 1;
        foreach(var a in agents)
        {
            if (max < a.GetScore())
            {
                max = a.GetScore();
            }

            if(min > a.GetScore())
            {
                min = a.GetScore();
            }
        }

        Debug.Log("MAX: " + max);
        Debug.Log("MIN: " + min);
    }
    public void SetGladiatorInitialPositions()
    {
        for (int i = 0; i < simCount; i++)
        {
            agents[i].gameObject.transform.position = startPos[i].position;
        }
    }

    public void GameOver()
    {
        isLearning = false;
    }
}
