using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreTrigger : MonoBehaviour {

    public GameManager gm;
    // Use this for initialization
    void Start()
    {
        try
        {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        catch
        {
            Debug.LogError("GameManager is missing");
        }
    }

    // Update is called once per frame
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            gm.ObstaclePassed();
        }
    }
}
