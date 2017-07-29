using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour {

    private float maxSpeed = 12f;

    private Transform trans;
    private Rigidbody2D rg2d;

    bool grounded = false;

    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    // Use this for initialization
    void Start () {
        trans = GetComponent<Transform>(); //pobieranie cech komponentu Transform
        rg2d = GetComponent<Rigidbody2D>(); //pobieranie cech komponentu Rigidbody2D
    }
	
	// Update is called once per frame
	void Update () {
        // Transformacje położenie w zależności od naciśniętego klawisza
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    trans.Translate(-0.25f, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    trans.Translate(0.25f, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    trans.Translate(0, 0.25f, 0);
        //}

        //rotacja postaci, jezeli jest przekrzywiona
        ////Do poprawy poniewaz obecnie postac drga -.- przy malych bardzo kątach, mniejszych niz kat obrotu (grrr)
        //if (trans.rotation.z > 0)
        //{
        //    trans.Rotate(0, 0, -2);
        //}
        //else if(trans.rotation.z < 0)
        //{
        //    trans.Rotate(0, 0, 2);
        //}
        if (grounded && Input.GetKeyDown(KeyCode.Space)) //skok przy użyciu fizyki
        {
            rg2d.AddForce(new Vector2(0, 550.0f));
        }

    }

    private void FixedUpdate() //używajmy fizyki do ruchu, jest bardziej naturalnie, w tej funkcji zamieszcza się większość rzeczy związanych z fizyką
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        float move = Input.GetAxis("Horizontal");
        rg2d.velocity = new Vector2(move * maxSpeed, rg2d.velocity.y);
    }


}
