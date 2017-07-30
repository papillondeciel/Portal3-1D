using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerScript : NetworkBehaviour {

    public float maxSpeed = 12f;
    private float maxJump = 12f;
    private float moveForce = 120f;
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
    bool facingRight = true;
    bool isPaused = false;
    //handgun
    Vector3 left = new Vector3(-0.21f, 0.5f, 0);
    Vector3 right = new Vector3(-0.21f, -0.5f, 0);

    // Use this for initialization
    void Start () {
        rg2d = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rg2d.freezeRotation = true;
        //wyłączenie menu dotyczącego Networkingu
        GameObject.Find("Network Manager").GetComponent<NetworkManagerHUD>().showGUI = false;
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
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.GetChild(1).transform.SetPositionAndRotation(this.transform.position + left, transform.rotation);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {

            transform.GetChild(1).transform.SetPositionAndRotation(this.transform.position - right, transform.rotation);

        }
        if (!isPaused)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.GetChild(1).transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
        }
    }
    private void Jump()
    {
        //animator.Play("idlejump");
        //animator.Play("idlejump1");
        Vector2 resolvedJump = new Vector2(rg2d.velocity.x, maxJump);
        rg2d.AddForce(resolvedJump, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) 
            return;

        //animator.enabled = false;
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        float move = Input.GetAxis("Horizontal");
        if (invertHorizontalMovement)
            move = -move;
        if (move > 0 && !facingRight)
        {
            CmdFlipCharacter();
        }
        else if (move < 0 && facingRight)
        {
            CmdFlipCharacter();
        }
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
        animator.SetFloat("movingSpeed", Mathf.Abs(move));
        animator.SetBool("grounded", grounded);
        
    }

    public void pendHorizontalMovementChange()
    {
        pendInvertHorizontal = true;
    }
    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }

    [Command]
    public void CmdFlipCharacter()
    {
        RpcFlipCharacter();
    }
    [ClientRpc]
    public void RpcFlipCharacter()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
