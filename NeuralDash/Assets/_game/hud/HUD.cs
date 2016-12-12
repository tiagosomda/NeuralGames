using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Text gameTitle;
    public Text iteration;
    public Text maxScore;
    public Text score;

    public Color onMaxScoreColor;
    public int onMaxScoreFontSize;
    public int scoreRegularSize;

    public bool playMaxScoreSound = false;
    public AudioClip onMaxScoreSound;

    public GameObject DataItemPrefab;
    public GameObject[] DataItemParent;

    public List<string> data_names;
    public List<Text> data_values;

    private AudioSource audioSource;
    private float maxScoreTimer;
    private float iterationTimer;

    public void Start()
    {
        data_names = new List<string>();
        data_values = new List<Text>();

        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if(maxScoreTimer > 0f)
        {
            maxScore.color = Color.Lerp(onMaxScoreColor, Color.white, 1f - maxScoreTimer);
            score.color = Color.Lerp(onMaxScoreColor, Color.white, 1f - maxScoreTimer);
            score.fontSize = (int)Mathf.Lerp(onMaxScoreFontSize, scoreRegularSize, 1f - maxScoreTimer);

            maxScoreTimer -= Time.deltaTime * 1f;
        }

        if(iterationTimer > 0f)
        {
            iteration.color = Color.Lerp(onMaxScoreColor, Color.white, 1f - iterationTimer);
            iterationTimer -= Time.deltaTime * 1f;
        }
    }

    public void SetIteration(int value)
    {
        string newIteration = "ITERATION " + value;

        if(iteration.text != newIteration)
        {
            iterationTimer = 1f;
            iteration.text = "ITERATION " + value;

        }
    }

    public void SetScore(int value, bool isMaxScore)
    {
        score.text = value.ToString();
        if(isMaxScore)
        {
            // play max score animation/sound

            maxScoreTimer = 1f;
            maxScore.text = "MAX SCORE " + value;

            if(playMaxScoreSound)
            {
                audioSource.PlayOneShot(onMaxScoreSound);
            }
        }
    }

    public void ResetTopValues()
    {
        score.text = "0";
        iteration.text = "ITERATION 0";
        maxScore.text = "MAX SCORE ---";
    }

    public void PanelLeft(string name, string value, Color color)
    {
        SetDataItem(0, name, value, color);
    }

    public void PanelRight(string name, string value, Color color)
    {
        SetDataItem(1, name, value, color);
    }

    private void SetDataItem(int side, string name, string value, Color color)
    {
        if (!data_names.Contains(name))
        {
            if (data_names == null)
            {
                data_names = new List<string>();
                data_values = new List<Text>();
            }

            data_names.Add(name);

            var item = Instantiate(DataItemPrefab) as GameObject;

            var data_n = item.transform.FindChild("Name").GetComponent<Text>();
            var data_v = item.transform.FindChild("Value").GetComponent<Text>();

            data_n.color = color;
            data_v.color = color;

            data_n.text = name;

            data_values.Add(data_v);

            item.transform.SetParent(DataItemParent[side].transform);
        }

        var index = data_names.IndexOf(name);
        data_values[index].text = value;
    }
}
