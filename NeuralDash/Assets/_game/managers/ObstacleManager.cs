using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObstacleManager : MonoBehaviour {

    public GameObject obstaclePrefab;
    public Transform[] startPositions;
    public int[] sizes;


    private Vector2 setVelocity;
    private List<Rigidbody2D> allObstacles = new List<Rigidbody2D>();
    private Vector2 obstacleVelocity = Vector2.zero;
    private Vector3 initialSize;

    private int spawnRate;
    private bool spawning;

    private Transform ObstacleParentObject;

    public bool IsSpawning()
    {
        return spawning;
    }

    public void IsSpawning(bool isSpawning)
    {
        spawning = isSpawning;
    }

    public void StartSpawning()
    {
        spawning = true;
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    public void SetSpawnRateInMilliseconds(int rate)
    {
        spawnRate = rate;
    }

    public int GetSpawnRate()
    {
        return spawnRate;
    }

    public void SetObstacleSpeed(float velocity)
    {
        setVelocity.x = velocity * -1;
    }

    public float GetObstacleVelocity()
    {
        return Math.Abs(obstacleVelocity.x);
    }

    private void Start()
    {
        spawning = false;
        StartCoroutine(Spawn());


        ObstacleParentObject = transform.FindChild("Obstacles");
    }

    void Update () {
        if(obstacleVelocity != setVelocity)
        {
            UpdateSpeed();
        }
    }

    public void UpdateSpeed()
    {
        foreach (GameObject obstacle in GetAllObstacles())
        {
            obstacle.GetComponent<Rigidbody2D>().velocity = setVelocity;
        }

        obstacleVelocity = setVelocity;
    }

    private IEnumerator Spawn()
    {
        if (spawning)
        {
            // get obstacle
            var obstacle = GetObstacle();

            // set initial position
            var randomPos = UnityEngine.Random.Range(0, startPositions.Length);
            obstacle.transform.position = startPositions[randomPos].position;

            // set size
            var randomSize = UnityEngine.Random.Range(0, sizes.Length);

            var scale = initialSize;
            scale.x = initialSize.x * sizes[randomSize];
            obstacle.transform.localScale = scale;

            // set obstacle initial speed
            var rigidbody = obstacle.GetComponent<Rigidbody2D>();
            rigidbody.velocity = setVelocity;
            yield return new WaitForSecondsRealtime(spawnRate / 1000f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(3);
        }

        StartCoroutine(Spawn());
    }

    public void ObliterateObstacle(GameObject go)
    {
        var rigidbody = go.GetComponent<Rigidbody2D>();
        rigidbody.velocity = Vector2.zero;

        go.SetActive(false);

    }

    public void ResetObstacles()
    {
        foreach(var obstacle in allObstacles)
        {
            ObliterateObstacle(obstacle.gameObject);
        }
    }

    private GameObject GetObstacle()
    {
        GameObject obstacle = null;


        foreach( var obs in allObstacles)
        {
            if(!obs.gameObject.activeSelf)
            {
                obstacle = obs.gameObject;
                obstacle.SetActive(true);
                break;
            }
        }

        if(obstacle == null)
        {
            obstacle = Instantiate(obstaclePrefab) as GameObject;
            initialSize = obstacle.transform.localScale;
            allObstacles.Add(obstacle.GetComponent<Rigidbody2D>());

            obstacle.transform.SetParent(ObstacleParentObject);
        }

        return obstacle;
    }

    private GameObject[] GetAllObstacles()
    {
        return GameObject.FindGameObjectsWithTag("Obstacle");
    }

}
