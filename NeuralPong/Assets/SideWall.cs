using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour {
    public GameManager gameController;

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.name == "Ball")
        {
            string wallName = transform.name;
            gameController.Score(wallName);
            hitInfo.gameObject.SendMessage("RelaunchBall", 1.0f, SendMessageOptions.RequireReceiver);
        }
    }
}
