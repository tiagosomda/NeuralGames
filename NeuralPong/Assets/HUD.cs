using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {
    public Text scoreP1;
    public Text scoreP2;
    public Text winScreenP1;
    public Text winScreenP2;

    public Text restartButton;

    public Text WinText;

    public void UpdateScore(int p1, int p2)
    {
        scoreP1.text = p1.ToString();
        scoreP2.text = p2.ToString();

        winScreenP1.text = "Player One : " + scoreP1.text;
        winScreenP2.text = "Player Two : " + scoreP2.text;
    }

    public void SetWinText(string txt)
    {
        WinText.text = txt;
    }

}
