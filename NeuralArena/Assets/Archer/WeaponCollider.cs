﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour {


    public CharacterControl controller;
    public Transform gladiator;

    public void AddPoint(int points)
    {
        controller.AddPoint(points);
    }

}
