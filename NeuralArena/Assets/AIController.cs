using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(CharacterControl))]
public class AIController : AICharacter {

    private int bulletIndex;
    private int gladiatorIndex;
    private int canShootIndex;
    private int visionIndex;

    private CharacterControl control;
    private Vision vision;

    private LayerMask bullet, gladiator;
    private double[] sensorInput = new double[4];

    void Awake()
    {
        InitializeAI();

        vision = gameObject.GetComponentInChildren<Vision>();
        control = gameObject.GetComponent<CharacterControl>();


        gladiator = LayerMask.NameToLayer("Gladiator");
        gladiatorIndex = 0;

        bullet = LayerMask.NameToLayer("Bullet");
        bulletIndex = 1;

        canShootIndex = 2;

        visionIndex = 3;
    }
    public override int GetScore()
    {
        return control.score;
    }

    public override double[] GetSensorReading()
    {
        sensorInput[bulletIndex] = vision.IsPresent(bullet) ? 1f : 0f;
        sensorInput[gladiatorIndex] = vision.IsPresent(gladiator) ? 1f : 0f;
        sensorInput[canShootIndex] = control.CanDoAction() ? 1f : 0f;
        sensorInput[visionIndex] = vision.Range;

        return sensorInput;
    }

    public override void ProcessBrainOutput(double[] actions)
    {
        double walk = actions[0];
        double rotate = actions[1];
        double strafe = actions[2];
        double range = actions[4];

        bool shoot = Mathf.Abs((float)actions[3] % 1) > 0.5 ? true : false;

        control.Move((float)walk, (float)rotate, (float)strafe);

        if (shoot)
        {
            control.WeaponAction();
        }

        vision.Range = (float)range;
    }

    public override void ResetSkipper()
    {
    }
}
