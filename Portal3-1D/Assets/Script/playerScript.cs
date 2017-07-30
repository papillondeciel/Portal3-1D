using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerScript : NetworkBehaviour {

    public float maxSpeed = 12f;
    private float maxJump = 12f;
    private float moveForce = 120f;
    public float velocityX = 0;
    public float velocityY = 0;
    public float maxFallingSpeed=-35;
    public bool jumpEnabled = true;
    private Rigidbody2D rg2d;
    [HideInInspector] public SpriteRenderer rend;
    [HideInInspector] public bool grounded = false;
    public bool mimic = false;
    public bool thrown = false;
    public bool invertHorizontalMovement = false;
    private bool pendInvertHorizontal = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public NetworkConnection networkConnection;
    public Animator animator;
    bool moving = false;

    // Use this for initialization
    void Start () {
        rg2d = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rg2d.freezeRotation = true;
    }
	
	void Update () {
        
        if (!isLocalPlayer) 
            return;

        if (grounded && Input.GetKeyDown(KeyCode.Space) && jumpEnabled)
            Jump();

        if(pendInvertHorizontal)
        {
            if(Input.GetAxis("Horizontal") == 0)
            {
                invertHorizontalMovement = false;
                pendInvertHorizontal = false;
            }
        }
        if (Input.GetAxis("Horizontal") == 0 || !grounded)
        {
            moving = false;
            
        }
        if(Input.GetAxis("Horizontal") > 0 && grounded )
        {
            //animator.Play("walking");
            moving = true;
            rend.flipX = false;
        }
        else if (Input.GetAxis("Horizontal") < 0 && grounded)
        {
            //animator.Play("walking");
            moving = true;
            rend.flipX = true;
        }
        velocityX = rg2d.velocity.x;
        velocityY = rg2d.velocity.y;
    }
    private void Jump()
    {
        animator.Play("idlejump");
        animator.Play("idlejump1");
        Vector2 resolvedJump = new Vector2(rg2d.velocity.x, maxJump);
        rg2d.AddForce(resolvedJump, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) 
            return;

        animator.enabled = false;
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        float move = Input.GetAxis("Horizontal");
        if (invertHorizontalMovement)
            move = -move;
        if (rg2d.velocity.y < maxFallingSpeed)
            rg2d.velocity = new Vector2(rg2d.velocity.x, maxFallingSpeed);
        if (grounded)
        {
            rg2d.velocity = new Vector2(move * maxSpeed, rg2d.velocity.y);
            thrown = false;
        }
        else if (!thrown)
        {
            if (move * rg2d.velocity.x < maxSpeed)
                rg2d.AddForce(Vector2.right * move * moveForce);

            if (Mathf.Abs(rg2d.velocity.x) > maxSpeed)
                rg2d.velocity = new Vector2(Mathf.Sign(rg2d.velocity.x) * maxSpeed, rg2d.velocity.y);
        }
        if (moving)
        {
            animator.enabled = true;
            //animator.Play("walking");
            //animator.Play("walking2");
        }
        //else
        //{
        //    animator.Play("idle");
        //}
    }

    public void pendHorizontalMovementChange()
    {
        pendInvertHorizontal = true;
    }


}
