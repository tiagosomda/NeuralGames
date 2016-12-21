using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gladiator : AICharacter {

    private Vision sensor;
    private ArrowWeapon weapon;
    private CharacterMovement movement;
    private double[] sensorInput = new double[5];

    private LayerMask lava, bullet, gladiator;

    private int lavaIndex;
    private int bulletIndex;
    private int gladiatorIndex;
    private int canShootIndex;
    private int visionIndex;

    private int surviveScore;

    // Use this for initialization
    void Awake () {

        InitializeAI();

        sensor = gameObject.GetComponentInChildren<Vision>();
        weapon = gameObject.GetComponentInChildren<ArrowWeapon>();
        movement = gameObject.GetComponent<CharacterMovement>();

        lava = LayerMask.NameToLayer("Water");
        lavaIndex = 0;

        gladiator = LayerMask.NameToLayer("Gladiator");
        gladiatorIndex = 1;

        bullet = LayerMask.NameToLayer("Bullet");
        bulletIndex = 2;

        canShootIndex = 3;

        visionIndex = 4;

        surviveScore = 1;
    }
	
    public override int GetScore()
    {
        return weapon.score + surviveScore;
    }

    public override double[] GetSensorReading()
    {
        sensorInput[bulletIndex] = sensor.IsPresent(bullet) ? 1f : 0f;
        sensorInput[gladiatorIndex] = sensor.IsPresent(gladiator) ? 1f : 0f;
        sensorInput[lavaIndex] = sensor.IsPresent(lava) ? 1f : 0f;
        sensorInput[canShootIndex] = weapon.CanShoot() ? 1f : 0f;
        sensorInput[visionIndex] = sensor.Range;

        return sensorInput;
    }

    public override void ProcessBrainOutput(double[] actions)
    {
        double walk = actions[0];
        double rotate = actions[1];
        double strafe = actions[2];
        double range = actions[4];

        bool shoot = Mathf.Abs((float)actions[3]  % 1) > 0.5 ? true : false;

        movement.Move((float)walk, (float)rotate, (float)strafe);

        if(shoot)
        {
            weapon.Action();
        }

        sensor.Range = (float)range;
    }

    public override void ResetSkipper()
    {
        weapon.score = 0;
        surviveScore = 1;
    }

    public void Die()
    {
        surviveScore = 0;
        this.gameObject.SetActive(false);
    }
}
