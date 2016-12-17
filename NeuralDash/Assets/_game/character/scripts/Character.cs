using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public Sensor sensor;

    public float jumpForce = 10f;
    public bool isGrounded = true;
    public LayerMask whatIsGround;

    public float groundRadiusCheck = 0.05f;

    private Animator animator;
    private Rigidbody2D rb;

    private Transform groundCheck;

    public bool isRunning;

    private int score;

    bool isInitPosSetOnce = false;
    Vector2 initialPosition;

    GameManager gm;
    HUD gameHud;

    private double[] neuronInput = new double[3];


    public void Awake()
    {
        groundCheck = transform.Find("GroundCheck");
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameHud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
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

    public void SetInitialPosition()
    {
        if (!isInitPosSetOnce)
        {
            isInitPosSetOnce = true;
            initialPosition = gameObject.transform.position;
        }

        gameObject.transform.position = initialPosition;
    }

    public void ResetSkipper()
    {
        this.gameObject.SetActive(true);

        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        SetInitialPosition();

        SetCharData("---", Color.black);
        isRunning = true;
    }

    public int GetScore()
    {
        return score;
    }

    public double[] GetSensorReading()
    {
        if (sensor.Data == null)
        {
            return null;
        }

        neuronInput[0] = sensor.Data.velocity;
        neuronInput[1] = sensor.Data.distance;
        neuronInput[2] = sensor.Data.dimension.x;

        return neuronInput;
    }

    public void HandleNeuronOutput(double[] actions)
    {
        float action = Mathf.Abs((float)actions[0]);
        //bool crouch = action < 0.20;

        bool jump = action > 0.80;

        string actionStr = jump ? "JUMP" : "RUN";

        SetCharData(actionStr, Color.black);

        if (jump)
        {
            Jump();
        }
    }


    public void SetCharData(string text, Color color)
    {
        gameHud.PanelLeft(name + " : ", text, color);
    }

    public void SetOtherData(string name, string value, Color color)
    {
        gameHud.PanelRight(name, value, color);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Obstacle")
        {
            return;
        }
        score = gm.score;
        SetCharData("Score " + score, Color.black);

        isRunning = false;
        this.gameObject.SetActive(false);

    }
}
