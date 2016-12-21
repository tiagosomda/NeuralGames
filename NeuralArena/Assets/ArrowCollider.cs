using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollider : MonoBehaviour {

    public GameObject gladiator;
    public ArrowWeapon weapon;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Gladiator")
        {
            if (collision.gameObject != gladiator)
            {
                weapon.IncreaseScore();
                gameObject.SetActive(false);
            }
        }
    }
}
