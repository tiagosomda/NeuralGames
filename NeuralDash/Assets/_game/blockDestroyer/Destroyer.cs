using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

    public ObstacleManager obstacleManager;
	
    void OnTriggerExit2D(Collider2D other)
    {
        obstacleManager.ObliterateObstacle(other.gameObject);
    }
}
