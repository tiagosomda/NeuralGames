using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {

    public Transform origin;
    public GameObject arrowPrefab;
    public float speed;

    Rigidbody2D arrowBody;

    private CharacterControl gladiator;

    private CharacterControl Gladiator
    {
        get
        {
            if(gladiator == null)
            {
                gladiator = GetComponentInParent<CharacterControl>();
            }

            return gladiator;
        }
    }

    void Awake()
    {
        actionEnabled = true;
    }

    public override void Action()
    {
        if (!actionEnabled)
        {
            return;
        }
        actionEnabled = false;


        var arrow = Instantiate(arrowPrefab, origin.position, Quaternion.identity) as GameObject;
        arrowBody = arrow.GetComponent<Rigidbody2D>();

        //arrow.transform.SetParent(Gladiator);
        arrow.GetComponent<WeaponCollider>().controller = Gladiator;
        arrow.tag = gladiator.gameObject.tag;

        arrow.transform.position = origin.position;
        arrow.transform.rotation = origin.rotation;
        arrowBody.velocity = (speed * transform.up);

        actionTimer = actionRate;
    }
}
