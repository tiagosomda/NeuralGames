using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

    public int agility = 1;

    // Normal Movements Variables
    public float walkRate;
    public float rotateRate;
    public float strafeRate;
    public float maxSpeed;

    private Rigidbody2D body;
    private Vector3 rotationAngle = new Vector3(0, 0, -1);
    private Vector3 walkVelocity = new Vector3(0,0,0);

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
 
    void FixedUpdate()
    {
        var walk   = Input.GetAxis("Vertical");
        var rotate = Input.GetAxis("Horizontal");
        var strafe = Input.GetAxis("Strafe");

        //Move(walk, rotate, strafe);
    }

    string moveTemplate = "WALK: {0} ROT: {1} STRF:{2}";
    public void Move(float walk, float rotate, float strafe)
    {
        //Debug.Log(string.Format(moveTemplate, walk, rotate, strafe));

        walk   = Mathf.Lerp(0, walk   * walkRate, 0.8f)   * agility;
        rotate = Mathf.Lerp(0, rotate * rotateRate, 0.8f) * agility;
        strafe = Mathf.Lerp(0, strafe * strafeRate, 0.8f) * agility;

        walkVelocity = (strafe * transform.right) + (walk * transform.up);

        // Move 
        body.velocity = walkVelocity;

        // Rotate
        transform.Rotate(rotationAngle * rotate);
    }
}
