using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour {

    public float maxDegrees;
    public float degrees;
    public float reach;
    public GameObject obj;

    public LayerMask[] layer;

    public float Range
    {
        get
        {
            return degrees;
        }

        set
        {
            var percentage = value > 1f ? 1f : value < 0f ? 0f : value;
            degrees = maxDegrees * percentage;
        }
    }

    private Dictionary<LayerMask, bool> presense;

    private LayerMask[] maskArray;
    private bool[] isPresentArray;

    private float offset = 0.7f;
    private RaycastHit2D hit;

    private Transform visionTransform;

    public void Awake()
    {
        presense = new Dictionary<LayerMask, bool>();

        maskArray = new LayerMask[layer.Length];
        isPresentArray = new bool[layer.Length];

        for(int i = 0; i < layer.Length; i++)
        {
            maskArray[i] = layer[i];
            isPresentArray[i] = false;
        }


        visionTransform = Instantiate(obj).transform;
        
        visionTransform.gameObject.name = "test";
        visionTransform.SetParent(this.transform);

    }

    public bool IsPresent(LayerMask layer)
    {
        for (int i = 0; i < maskArray.Length; i++)
        {
            if (maskArray[i] == layer)
            {
                return isPresentArray[i];
            }
        }

        return false;
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < maskArray.Length; i++)
        {
            if (Anyone(maskArray[i]))
            {
                isPresentArray[i] = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
            else if(isPresentArray[i])
            {
                isPresentArray[i] = false;
            }
        }
        DrawRange();
    }

    private bool Anyone(LayerMask l)
    {
        var rot = transform.rotation.eulerAngles;

        var rotPos = rot;
        var rotNeg = rot;
        var deg = degrees / 2;
        rot.z -= degrees / 2;

        for (int i = 0; i < deg; i++)
        {
            rotPos.z++;
            rotNeg.z--;
            
            //check positive side
            visionTransform.rotation = Quaternion.Euler(rotPos);
            hit = Physics2D.Raycast(transform.position + (visionTransform.up * offset), visionTransform.up, reach, l);
            if(hit.collider != null)
            {
                return true;
            }

            //check negative side
            visionTransform.rotation = Quaternion.Euler(rotNeg);
            hit = Physics2D.Raycast(transform.position + (visionTransform.up * offset), visionTransform.up, reach, l);
            if (hit.collider != null)
            {
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
