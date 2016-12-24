using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int winScore = 10;
    public HUD hud;
    public GameObject gameEnv;
     
    private int PlayerScore1 = 0;
    private int PlayerScore2 = 0;
    private BallControl theBall;


    private bool isPlaying;
    // Use this for initialization
    void Start () {
        theBall = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl>();

        isPlaying = false;
        gameEnv.SetActive(false);
        hud.ShowInitialScreen();
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
        hud.ShowWinScreen(true);
        isPlaying = false;
    }

    public void RestartGame()
    {
        if(isPlaying == false)
        {
            hud.ShowGameScreen();
            isPlaying = true;
        }

        PlayerScore1 = PlayerScore2 = 0;
        hud.UpdateScore(PlayerScore1, PlayerScore2);
        gameEnv.SetActive(true);
        theBall.RelaunchBall();
    }
}
