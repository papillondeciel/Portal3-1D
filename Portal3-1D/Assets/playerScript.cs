﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour {

    private float maxSpeed = 12f;
    private float maxJump = 12f;
    private float minJump = 5.5f;
    private float moveForce = 120f;
    public float maxFallingSpeed;
    public bool jumpEnabled = true;
    private Transform trans;
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

    // Use this for initialization
    void Start () {
<<<<<<< HEAD
        trans = GetComponent<Transform>();
        rg2d = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        rg2d.freezeRotation = true;
=======
        trans = GetComponent<Transform>(); //pobieranie cech komponentu Transform
        rg2d = GetComponent<Rigidbody2D>(); //pobieranie cech komponentu Rigidbody2D
>>>>>>> origin/master
    }
	
	void Update () {
        
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

    }
    private void Jump()
    {
        
        Vector2 resolvedJump = new Vector2(rg2d.velocity.x, maxJump);
        rg2d.AddForce(resolvedJump, ForceMode2D.Impulse);
    }
    private void FixedUpdate()
    {
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
    }
    public void pendHorizontalMovementChange()
    {
        pendInvertHorizontal = true;
    }


}
