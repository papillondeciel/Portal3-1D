using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalScript : MonoBehaviour {
    private GameObject wallOn;
    private GameObject oppositeWall;
    private Transform secondPortal;
    private GameObject teleportedCopy;
    private BoxCollider2D portalCollider;
    // Use this for initialization
    void Start () {
        if (this.name == "bluePortal")
        {
            wallOn = GameObject.Find("wallLeft");
            oppositeWall = GameObject.Find("wallRight");
            secondPortal = GameObject.Find("orangePortal").transform;
            portalCollider = GameObject.Find("portalColliderBlue").GetComponent<BoxCollider2D>();
        }
        else
        {
            wallOn = GameObject.Find("wallRight");
            oppositeWall = GameObject.Find("wallLeft");
            secondPortal = GameObject.Find("bluePortal").transform;
            portalCollider = GameObject.Find("portalColliderOrange").GetComponent<BoxCollider2D>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("Trigger entered");
            //coll = collision.gameObject.GetComponent<BoxCollider2D>();
            //coll.enabled = false;
            playerScript player = collision.gameObject.GetComponent<playerScript>();
            
            if (!player.mimic)
            {
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wallOn.GetComponent<Collider2D>());
                float height = player.transform.position.y - secondPortal.transform.position.y;
                if(this.name == "bluePortal")
                    teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.position.x + 0.5f, secondPortal.transform.position.y +  player.transform.position.y - transform.position.y), Quaternion.identity);
                else
                    teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.position.x - 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y), Quaternion.identity);
                Physics2D.IgnoreCollision(teleportedCopy.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>());
                teleportedCopy.transform.parent = player.gameObject.transform;
                
                teleportedCopy.gameObject.GetComponent<playerScript>().mimic = true;
                teleportedCopy.gameObject.GetComponent<playerScript>().GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;
            }

            if (this.name == "bluePortal")
            {
                player.rend.material.SetInt("_CutDirection", 3);
            }
            else
                player.rend.material.SetInt("_CutDirection", 1);
            player.rend.material.SetFloat("_CutPos", this.transform.position.x);
            


        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
       
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("Trigger exited");
            //coll.enabled = true;
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wallOn.GetComponent<Collider2D>(), false);
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>(), false);
            playerScript player = collision.gameObject.GetComponent<playerScript>();
            player.rend.material.SetInt("_CutDirection", 0);
            if(this.name == "bluePortal")
            {
                if (!player.mimic && collision.transform.position.x < this.transform.position.x)
                {
                    teleportedCopy.transform.parent = null;
                    Destroy(collision.gameObject);
                    teleportedCopy.gameObject.GetComponent<playerScript>().mimic = false;
                    teleportedCopy.name = "player";
                    teleportedCopy.gameObject.GetComponent<playerScript>().rend.material.name = "Material";
                }
                else if (!player.mimic && collision.transform.position.x > this.transform.position.x)
                {
                    Destroy(teleportedCopy);
                }
            }
            else
            {
                if (!player.mimic && collision.transform.position.x > this.transform.position.x)
                {
                    teleportedCopy.transform.parent = null;
                    Destroy(collision.gameObject);
                    teleportedCopy.gameObject.GetComponent<playerScript>().mimic = false;
                    teleportedCopy.name = "player";
                    teleportedCopy.gameObject.GetComponent<playerScript>().rend.material.name = "Material";
                }
                else if (!player.mimic && collision.transform.position.x < this.transform.position.x)
                {
                    Destroy(teleportedCopy);
                }
            }

        }
    }
}
