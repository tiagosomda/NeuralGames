using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
public class PlayerController : MonoBehaviour {

    public KeyCode WeaponKey = KeyCode.Space;
    private CharacterControl control;
	// Use this for initialization
	void Awake () {
        control = GetComponent<CharacterControl>();
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(WeaponKey))
        {
            control.WeaponAction();
        }

        var walk = Input.GetAxis("Vertical");
        var rotate = Input.GetAxis("Horizontal");
        var strafe = Input.GetAxis("Strafe");

        control.Move(walk, rotate, strafe);
    }
}
