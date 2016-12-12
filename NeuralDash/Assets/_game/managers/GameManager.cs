using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int spawnRate = 2000;
    public int initialVelocity = 10;
    public float velocityIncreatePerLevel = 1;
    public int levelRange = 0;
    public int obstaclesOvercome = 0;

    //public GameObject GameTitle;
    //public GameObject RunningScreen;
    //public GameObject GameOverScreen;

    //public Text ObstacleCountText;

    //public Text Score;

    public ObstacleManager obstacleManager;

    //private bool gameOver = true;

    private int levelCountdown;

    // Use this for initialization
    void Start () {

        levelCountdown = levelRange;

        if(obstacleManager == null)
        {
            Debug.LogWarning("Missing ObstacleManager script on GameObject");
        }
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyUp(KeyCode.S))
        {
            obstacleManager.IsSpawning(!obstacleManager.IsSpawning());
            obstacleManager.SetObstacleSpeed(initialVelocity);
            obstacleManager.SetSpawnRateInMilliseconds(spawnRate);
        }

	    if(Input.GetKeyUp(KeyCode.Return))
        {
            ResetGame();
        }

        if (levelCountdown <= 0)
        {
            levelCountdown = levelRange;
            var velocity = obstacleManager.GetObstacleVelocity() + velocityIncreatePerLevel;
            obstacleManager.SetObstacleSpeed(velocity);
        }
    }

    public void ObstaclePassed()
    {
        obstaclesOvercome += 1;
        //SetScore();
        levelCountdown--;
        if(obstacleManager.GetSpawnRate() > 500)
        {
            var rate = spawnRate - (obstaclesOvercome * 10);
            if(rate < 500)
            {
                rate = 500;
            }

            obstacleManager.SetSpawnRateInMilliseconds(rate);
        }
    }

    //public void SetScore()
    //{
    //    Score.text = (obstaclesOvercome * 100).ToString();
    //    ObstacleCountText.text = obstaclesOvercome.ToString();
    //}

    public void ResetGame()
    {
        //gameOver = false;

        obstacleManager.SetSpawnRateInMilliseconds(spawnRate);
        
        //RunningScreen.SetActive(true);
        //GameTitle.SetActive(false);
        //GameOverScreen.SetActive(false);
        //SetScore();

        obstaclesOvercome = 0;
        obstacleManager.SetObstacleSpeed(initialVelocity);
    }


    public void SetSpawning(bool isSpawning)
    {
        obstacleManager.IsSpawning(isSpawning);
    }

    public void GameOver()
    {
        obstacleManager.ResetObstacles();

        //RunningScreen.SetActive(false);
        //GameTitle.SetActive(true);
        //GameOverScreen.SetActive(true);

        //gameOver = true;
    }
}
