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
    private HUD gameHud;
    private Character[] students;

    //public GameObject GameTitle;
    //public GameObject RunningScreen;
    //public GameObject GameOverScreen;

    //public Text ObstacleCountText;

    //public Text Score;

    public ObstacleManager obstacleManager;

    //private bool gameOver = true;

    private int levelCountdown;

    private bool isLearning;
    private bool noneRunning;

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

        CreateEntities();
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
            learner.NewGeneration();
            ResetGame();
            SetSpawning(true);
            ResetGame();
            isLearning = true;
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

        gameHud.SetIteration(learner.currentIteration);
    }

    public void FixedUpdate()
    {
        if (!isLearning)
        {
            return;
        }

        foreach (var skipper in students)
        {
            if (skipper.isRunning)
            {
                noneRunning = false;
                break;
            }

            noneRunning = true;
        }

        if (noneRunning)
        {
            isLearning = false;
            learner.EndGeneration();
            SetSpawning(false);
            GameOver();

            learner.NewGeneration();
            ResetGame();
            SetSpawning(true);
            noneRunning = false;
            isLearning = true;
            return;
        }

        learner.ActivateBrain();
    }

    public void CreateEntities()
    {
        students = new Character[simulationCount];

        for (int i = 0; i < simulationCount; i++)
        {
            var skipperGameObject = Instantiate(skipperPrefab);
            skipperGameObject.transform.SetParent(gameObject.transform);

            var pos = skipperGameObject.transform.position;
            pos.x -= characterSpacing * i;
            skipperGameObject.transform.position = pos;

            skipperGameObject.name = "[" + i + "]";
            students[i] = skipperGameObject.GetComponent<Character>();
        }

        learner.AddStudents(students);
    }

    public void ObstaclePassed()
    {
        score += 1;
        SetScore();
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

    public void SetScore()
    {
        //score = score * 100;

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

    public void ResetGame()
    {
        //gameOver = false;

        obstacleManager.SetSpawnRateInMilliseconds(spawnRate);
        
        //RunningScreen.SetActive(true);
        //GameTitle.SetActive(false);
        //GameOverScreen.SetActive(false);
        //SetScore();

        score = 0;
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
