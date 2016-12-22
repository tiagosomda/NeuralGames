using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    public int agility = 1;

    // Normal Movements Variables
    public float walkRate;
    public float rotateRate;
    public float strafeRate;
    
    private Rigidbody2D body;
    private Vector3 rotationAngle = new Vector3(0, 0, -1);
    private Vector3 walkVelocity = new Vector3(0, 0, 0);

    private Weapon weapon;

    public int score;

    // Use this for initialization
    void Awake () {
        body = GetComponentInChildren<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
	}

    public void Move(float walk, float rotate, float strafe)
    {
        //Debug.Log(string.Format(moveTemplate, walk, rotate, strafe));

        walk = Mathf.Lerp(0, walk * walkRate, 0.8f) * agility;
        rotate = Mathf.Lerp(0, rotate * rotateRate, 0.8f) * agility;
        strafe = Mathf.Lerp(0, strafe * strafeRate, 0.8f) * agility;

        walkVelocity = (strafe * transform.right) + (walk * transform.up);

        // Move 
        body.velocity = walkVelocity;

        // Rotate
        transform.Rotate(rotationAngle * rotate);
    }

    public void WeaponAction()
    {
        weapon.Action();
    }

    public bool CanDoAction()
    {
        return weapon.CanDoAction();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Damage")
        {
            var control = collision.gameObject.GetComponent<WeaponCollider>();
            if(control.gladiator != gameObject.transform)
            {
                control.AddPoint(1);
                Destroy(collision.gameObject);
            }
        }
    }

    public void AddPoint(int points)
    {
        score += points;
    }
}
