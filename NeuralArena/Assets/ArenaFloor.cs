using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaFloor : MonoBehaviour {

    LayerMask gladiator;
    public void Awake()
    {
        gladiator = LayerMask.NameToLayer("Gladiator");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == gladiator)
        {
            collision.gameObject.GetComponent<Gladiator>().Die();
        }
    }
}
