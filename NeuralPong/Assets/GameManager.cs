using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int winScore = 10;
    public HUD hud;
    public GameObject gameEnv;
    public  BallControl theBall;


    public GameObject startScreen;
    public GameObject gameScreen;
    public GameObject winScreen;
     
    private int PlayerScore1 = 0;
    private int PlayerScore2 = 0;


    private bool isPlaying;
    // Use this for initialization
    void Awake () {
        //theBall = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl>();

        isPlaying = false;
        startScreen.SetActive(true);
    }

    public void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        if (PlayerScore1 == winScore || PlayerScore2 == winScore)
        {
            WinGame();
        }
    }

    public void Score(string wallID)
    {
        if (wallID == "right")
        {
            PlayerScore1++;
        }
        else
        {
            PlayerScore2++;
        }

        hud.UpdateScore(PlayerScore1, PlayerScore2);
    }

    public void WinGame()
    {
        gameEnv.SetActive(false);

        var winText = PlayerScore1 > PlayerScore2 ? "PLAYER ONE WINS" : "PLAYER TWO WINS";
        theBall.ResetBall();
        hud.SetWinText(winText);
        gameScreen.SetActive(false);
        winScreen.SetActive(true);
        isPlaying = false;
    }

    public void RestartGame()
    {
        if(isPlaying == false)
        {
            isPlaying = true;
            startScreen.SetActive(false);
        }

        winScreen.SetActive(false);
        gameScreen.SetActive(true);
        PlayerScore1 = PlayerScore2 = 0;
        hud.UpdateScore(PlayerScore1, PlayerScore2);
        gameEnv.SetActive(true);
        theBall.RelaunchBall();
    }
}
