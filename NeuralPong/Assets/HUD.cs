using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {
    public Text scoreP1;
    public Text scoreP2;

    public Text restartButton;

    public Text WinText;

    public void UpdateScore(int p1, int p2)
    {
        scoreP1.text = p1.ToString();
        scoreP2.text = p2.ToString();
    }

    public void SetWinText(string txt)
    {
        WinText.text = txt;
    }

    public void ShowInitialScreen()
    {
        WinText.gameObject.SetActive(false);
        restartButton.text = "START GAME";
    }

    public void ShowGameScreen()
    {
        WinText.gameObject.SetActive(false);
        restartButton.text = "RESTART";
    }

    public void ShowWinScreen(bool show)
    {
        WinText.gameObject.SetActive(show);
    }
}
