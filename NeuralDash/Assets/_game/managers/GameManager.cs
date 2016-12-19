using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int spawnRate = 2000;
    public int initialVelocity = 10;
    public float velocityIncreatePerLevel = 1;
    public int levelRange = 0;
    public int score = 0;
    public int maxScore;

    public GameObject skipperPrefab;
    public int simulationCount;
    public float characterSpacing;

    public Learner learner;
    public ObstacleManager obstacleManager;

    private HUD gameHud;
    private Character[] agents;

    private int levelCountdown;

    private bool isLearning;
    private GeneticAlgorithm geneticAlgorithm;

    void Awake()
    {
        isLearning = false;
    }

    // Use this for initialization
    void Start () {

        levelCountdown = levelRange;

        if(obstacleManager == null)
        {
            Debug.LogWarning("Missing ObstacleManager script on GameObject");
        }

        gameHud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();

        #region Creating AI Entities
        agents = new Character[simulationCount];

        for (int i = 0; i < simulationCount; i++)
        {
            var skipperGameObject = Instantiate(skipperPrefab);
            skipperGameObject.transform.SetParent(gameObject.transform);

            var pos = skipperGameObject.transform.position;
            pos.x -= characterSpacing * i;
            skipperGameObject.transform.position = pos;

            skipperGameObject.name = "[" + i + "]";
            agents[i] = skipperGameObject.GetComponent<Character>();
        }
        #endregion
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyUp(KeyCode.S))
        {
            obstacleManager.IsSpawning(!obstacleManager.IsSpawning());
            obstacleManager.SetObstacleSpeed(initialVelocity);
            obstacleManager.SetSpawnRateInMilliseconds(spawnRate);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetGame();
            StartGame();
            learner.iteration = 0;
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            ResetGame();
        }

        if (levelCountdown <= 0)
        {
            levelCountdown = levelRange;
            var velocity = obstacleManager.GetObstacleVelocity() + velocityIncreatePerLevel;
            obstacleManager.SetObstacleSpeed(velocity);
        }

        gameHud.SetIteration(learner.iteration);
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
        if(AnyAgentRunning())
        {
            return;
        }

        // no one is running
        // start new generation
        if (isLearning)
        {
            learner.iteration++;
            GameOver();

            learner.NextGen(agents);

            ResetGame();
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

    public void ObstaclePassed()
    {
        SetScore(score++);
        levelCountdown--;
        if(obstacleManager.GetSpawnRate() > 500)
        {
            var rate = spawnRate - (score * 10);
            if(rate < 500)
            {
                rate = 500;
            }

            obstacleManager.SetSpawnRateInMilliseconds(rate);
        }
    }

    public void SetScore(int score)
    {
        gameHud.SetScore(score, score > maxScore);

        if(score > maxScore)
        {
            maxScore = score;
        }
    }

    public void SetIterationCount(int iteration)
    {
        gameHud.SetIteration(iteration);
    }

    public void StartGame()
    {
        isLearning = true;
        obstacleManager.IsSpawning(true);
    }

    public void GameOver()
    {
        isLearning = false;
        obstacleManager.IsSpawning(false);
        obstacleManager.ResetObstacles();
    }

    public void ResetGame()
    {
        obstacleManager.SetSpawnRateInMilliseconds(spawnRate);
        obstacleManager.SetObstacleSpeed(initialVelocity);

        foreach (var skipper in agents)
        {
            skipper.ResetSkipper();
        }

        score = 0;
        SetScore(score);
    }
}
