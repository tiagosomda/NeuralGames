using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

    public GameObject swordPrefab;
    GameObject sword;

    public override void Action()
    {
        if (!actionEnabled)
        {
            return;
        }


    }
}
