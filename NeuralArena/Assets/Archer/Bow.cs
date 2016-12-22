using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {

    public Transform origin;
    public GameObject arrowPrefab;
    public float actionRate;

    GameObject arrow;
    Rigidbody2D arrowBody;

    public float speed;

    private bool canShoot;
    private float actionTimer;

    // Use this for initialization
    void Awake()
    {
        arrow = Instantiate(arrowPrefab, origin.position, Quaternion.identity) as GameObject;
        arrowBody = arrow.GetComponent<Rigidbody2D>();
        arrow.SetActive(false);
        canShoot = true;
    }

    public void Update()
    {
        if(!canShoot)
        {
            actionTimer -= Time.deltaTime * actionRate;

            if(actionTimer <= 0f)
            {
                canShoot = true;
                arrow.SetActive(false);
            }
        }
    }

    public override void Action()
    {
        if (!canShoot)
        {
            return;
        }

        arrow.SetActive(true);

        arrow.transform.position = origin.position;
        arrow.transform.rotation = origin.rotation;
        arrowBody.velocity = (speed * transform.up);

        actionTimer = actionRate;
        canShoot = false;
    }
}
