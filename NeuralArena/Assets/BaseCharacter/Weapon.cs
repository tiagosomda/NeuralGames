using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    public float actionRate;
    public bool actionEnabled;
    public abstract void Action();

    protected float actionTimer;

    public bool CanDoAction()
    {
        return actionEnabled;
    }

    public void Update()
    {
        if (!actionEnabled)
        {
            actionTimer -= Time.deltaTime * actionRate;

            if (actionTimer <= 0f)
            {
                actionEnabled = true;
            }
        }
    }
}
