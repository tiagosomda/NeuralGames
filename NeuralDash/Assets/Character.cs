using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public float jumpForce = 10f;
    public bool isGrounded = true;
    public LayerMask whatIsGround;

    public float groundRadiusCheck = 0.05f;

    private Animator animator;
    private Rigidbody2D rb;

    private Transform groundCheck;

    public void Awake()
    {
        groundCheck = transform.Find("GroundCheck");
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    public void Jump()
    {
        if (isGrounded)
        {
            //rb.AddForce(Vector2.up * jumpForce);
            rb.velocity = Vector2.up * jumpForce;
            isGrounded = false;
        }
    }


    private void CheckGround()
    {
        isGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadiusCheck, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                isGrounded = true;
        }
        animator.SetBool("isGrounded", isGrounded);
    }
}
