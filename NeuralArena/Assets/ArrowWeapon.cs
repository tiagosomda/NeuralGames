using UnityEngine;
using System.Collections;

public class ArrowWeapon : MonoBehaviour {

    public Transform origin;
    public GameObject weaponPrefab;
    public KeyCode ActionKey = KeyCode.Space;
    public float activationRate;

    public int score;

    GameObject arrow;
    Rigidbody2D weaponBody;
    ArrowCollider arrowCollider;

    public float speed;

    // Use this for initialization
    void Awake () {
        arrow = Instantiate(weaponPrefab, origin.position, Quaternion.identity) as GameObject;
        arrowCollider = arrow.GetComponent<ArrowCollider>();
        weaponBody = arrow.GetComponent<Rigidbody2D>();
        arrow.SetActive(false);

        arrowCollider.weapon = this;
        arrowCollider.gladiator = gameObject.transform.parent.gameObject;
        score = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(ActionKey))
        {
            Action();
        }
    }

    public void Action()
    {
        if (arrow.activeSelf || !gameObject.activeSelf)
        {
            return;
        }

        arrow.SetActive(true);

        arrow.transform.position = origin.position;
        weaponBody.velocity = (speed * transform.up);

        if(gameObject.activeSelf)
        {
            StartCoroutine(KillProjectile());
        }
    }

    IEnumerator KillProjectile()
    {
        yield return new WaitForSeconds(activationRate);
        arrow.SetActive(false);
    }

    public bool CanShoot()
    {
        return !arrow.activeSelf;
    }

    public void IncreaseScore()
    {
        score++;
    }
}
