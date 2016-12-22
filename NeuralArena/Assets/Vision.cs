using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour {

    public float degrees;
    public float reach;

    private LayerMask[] layer;

    public float Range
    {
        get
        {
            return degrees;
        }

        set
        {
            if (value > 120)
            {
                degrees = 120;
            }
            else if (value < 20)
            {
                degrees = 20;
            }
            else
            {
                degrees = value;
            }
        }
    }

    private Dictionary<LayerMask, bool> presense;

    private float offset = 0.7f;
    private RaycastHit2D hit;

    private Transform visionTransform;

    public void Awake()
    {
        presense = new Dictionary<LayerMask, bool>();
        visionTransform = Instantiate(new GameObject()).transform;

    }

    public bool IsPresent(LayerMask layer)
    {
        if(!presense.ContainsKey(layer))
        {
            presense.Add(layer, false);
        }

        return presense[layer];
    }

    public void Update()
    {
        foreach(var l in presense.Keys)
        {
            if(Anyone(l))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
        DrawRange();
    }

    private bool Anyone(LayerMask l)
    {
        var rot = transform.rotation.eulerAngles;
        rot.z -= degrees / 2;

        if(!presense.ContainsKey(l))
        {
            presense.Add(l, false);
        }

        if (presense[l])
        {
            presense[l] = false;
        }

        for (int i = 0; i < degrees; i++)
        {
            rot.z++;
            visionTransform.rotation = Quaternion.Euler(rot);
            hit = Physics2D.Raycast(visionTransform.position + (visionTransform.up * offset), visionTransform.up, reach, l);

            if(hit.collider != null)
            {
                presense[l] = true;
                return true;
            }
        }

        return false;
    }

    public void DrawRange()
    {
        var rot = transform.rotation.eulerAngles;

        rot.z -= degrees / 2;

        visionTransform.rotation = Quaternion.Euler(rot);

        Debug.DrawLine(transform.position, transform.position + ((reach + offset) * visionTransform.up));

        rot.z += degrees;
        visionTransform.rotation = Quaternion.Euler(rot);
        Debug.DrawLine(transform.position, transform.position + ((reach + offset) * visionTransform.up));
    }
}
