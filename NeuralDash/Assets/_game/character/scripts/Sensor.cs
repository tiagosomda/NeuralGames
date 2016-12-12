using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour {

    public LayerMask detectionLayer;
    public float rayReach;
    private GameObject player;
    private Vector2 playerPosition;

    SensorData data;

    public SensorData Data
    {
        get { return data; }
    }

    private Vector2 previousPosition;

    public void SetPlayer(GameObject p)
    {
        player = p;
        playerPosition = player.transform.position;
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if(data != null)
            {
                Debug.Log(gameObject.name + " : " + data.ToString());
            }
        }
    }

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, rayReach, detectionLayer);

        if(hit.collider == null)
        {
            data = null;
            return;
        }

        if (data == null)
        {
            data = new SensorData();
        }

        Debug.DrawLine(transform.position, hit.point, Color.green);

        //retrieve object size
        var box = hit.collider.bounds.size;

        data.dimension.y = (int)box.y;
        data.dimension.x = (int)box.x;

        // the location of the object
        previousPosition = data.position;
        data.position = hit.point;

        // velocity : v = d/t
        data.velocity = (int) Mathf.Abs((previousPosition.x - data.position.x) / Time.fixedDeltaTime);

        // distance from player
        data.distance = (int) Mathf.Abs(playerPosition.x - hit.point.x);
        
    }
}

public class SensorData {
    public int velocity;
    public int distance;

    public Vector2 dimension;

    public Vector2 position;

    public override string ToString()
    {
        return string.Format("v: [{0}] dist: [{1}] size: [{2}x{3}] pos: [{4}x{5}]", velocity, distance, dimension.x, dimension.y, position.x, position.y);
    }

}
