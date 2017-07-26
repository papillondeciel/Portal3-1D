using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class portalScript : NetworkBehaviour {
    public enum FacingDirection { Left = 1, Up=4, Right=3, Down=2};

    public Collider2D wallOn;
    private Collider2D oppositeWall;
    [HideInInspector] public GameObject secondPortal;
    private GameObject teleportedCopy;
    private playerScript teleportedScript;
    private ParticleSystem particles;
    [SyncVar]
    public FacingDirection facingDirection;
    private bool active;
    private FacingDirection secondPortalFacing;
    [HideInInspector] public bool secondPortalFound;
    // Use this for initialization
    void Start () {
        particles = this.transform.GetChild(2).GetComponent<ParticleSystem>();
        searchForOtherPortal();
    }
	
	// Update is called once per frame
	void Update () {
        if (!secondPortalFound)
        {
            searchForOtherPortal();
        }
    }
    private void searchForOtherPortal()
    {
        if (this.tag == "BluePortal")
        {
            if (GameObject.FindWithTag("OrangePortal"))
            {
                secondPortal = GameObject.FindWithTag("OrangePortal");
                secondPortalFound = true;
                particles.Play();
            }
            else
            {
                secondPortalFound = false;
                active = false;
                particles.Stop();
            }
        }
        else if (this.tag == "OrangePortal")
        {
            if (GameObject.FindWithTag("BluePortal"))
            {
                secondPortal = GameObject.FindWithTag("BluePortal");
                secondPortalFound = true;
                particles.Play();
            }
            else
            {
                secondPortalFound = false;
                active = false;
                particles.Stop();
            }
        }
        if (secondPortalFound)
        {
            oppositeWall = secondPortal.GetComponent<portalScript>().wallOn;
            secondPortalFacing = secondPortal.GetComponent<portalScript>().facingDirection;
            active = true;
        }
    }
    
    /*
    [ClientRpc]
    public void RpcIgnoreWallCollision(GameObject collision, GameObject portal)
    {
        Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), portal.GetComponent<portalScript>().wallOn.GetComponent<Collider2D>());
        Debug.Log("elo!");
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.gameObject.tag == "PlayerBlue" || collision.gameObject.tag == "PlayerOrange")
            {
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic)
                {
                    //RpcIgnoreWallCollision(collision.gameObject, this.gameObject);
                    Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wallOn);
                    if (secondPortalFacing == FacingDirection.Left)
                    {
                        if (this.facingDirection == FacingDirection.Up)
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y - 1f), Quaternion.identity);
                        else
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.x);
                        if (this.facingDirection == FacingDirection.Left)
                        {
                            teleportedScript.invertHorizontalMovement = true;
                            teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
                        }
                        else if (this.facingDirection == FacingDirection.Up)
                            teleportedScript.thrown = true;
                        else
                            teleportedScript.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;
                    }
                    else if (secondPortalFacing == FacingDirection.Right)
                    {
                        if (this.facingDirection == FacingDirection.Up)
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x - 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y - 1f), Quaternion.identity);
                        else
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x - 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y), Quaternion.identity);

                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Right);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.x);
                        if (this.facingDirection == FacingDirection.Right)
                        {
                            teleportedScript.invertHorizontalMovement = true;
                            teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
                        }
                        else if (this.facingDirection == FacingDirection.Up)
                            teleportedScript.thrown = true;
                        else
                            teleportedScript.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;
                    }
                    else if (secondPortalFacing == FacingDirection.Up)
                    {
                        teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y - 1f), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.y);

                        if (this.facingDirection == FacingDirection.Right || this.facingDirection == FacingDirection.Left || this.facingDirection == FacingDirection.Up)
                            teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
                        else
                            teleportedScript.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;

                    }
                    else if (secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y + 1f), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.y);
                        if (this.facingDirection == FacingDirection.Down)
                        {
                            teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
                        }
                        else if (this.facingDirection == FacingDirection.Right || this.facingDirection == FacingDirection.Left)
                            teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
                        else
                            teleportedScript.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;
                    }

                    Physics2D.IgnoreCollision(teleportedCopy.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>());
                    teleportedCopy.transform.parent = player.gameObject.transform;

                    teleportedScript.mimic = true;

                    if (this.facingDirection == FacingDirection.Right)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Right);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.x);
                    }

                    else if (this.facingDirection == FacingDirection.Left)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.x);
                    }

                    else if (this.facingDirection == FacingDirection.Up)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.y);
                        player.jumpEnabled = false;
                    }

                    else if (this.facingDirection == FacingDirection.Down)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.y);
                    }
                    collision.GetComponent<portalShooting>().enabled = false;
                    teleportedCopy.GetComponent<portalShooting>().enabled = false;
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.gameObject.tag == "PlayerBlue" || collision.gameObject.tag == "PlayerOrange")
            {
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic)
                {
                    if (this.facingDirection == FacingDirection.Up && secondPortalFacing == FacingDirection.Right)
                    {
                        teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.GetComponent<Rigidbody2D>().velocity.y, 0f);
                    }
                    else if (this.facingDirection == FacingDirection.Up && secondPortalFacing == FacingDirection.Left)
                    {
                        teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, 0f);
                    }
                    else if (this.facingDirection == FacingDirection.Right && secondPortalFacing == FacingDirection.Up)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.y - transform.position.y, secondPortal.transform.position.y - player.transform.position.x + transform.position.x);
                    }
                    else if (this.facingDirection == FacingDirection.Right && secondPortalFacing == FacingDirection.Right)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x - player.transform.position.x + transform.position.x, secondPortal.transform.position.y + player.transform.position.y - transform.position.y);
                    }
                    else if (this.facingDirection == FacingDirection.Right && secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.y - transform.position.y, secondPortal.transform.position.y + player.transform.position.x - transform.position.x);
                    }
                    else if (this.facingDirection == FacingDirection.Left && secondPortalFacing == FacingDirection.Up)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.y - transform.position.y, secondPortal.transform.position.y + player.transform.position.x - transform.position.x);
                    }
                    else if (this.facingDirection == FacingDirection.Left && secondPortalFacing == FacingDirection.Left)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x - player.transform.position.x + transform.position.x, secondPortal.transform.position.y + player.transform.position.y - transform.position.y);
                    }
                    else if (this.facingDirection == FacingDirection.Left && secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.y - transform.position.y, secondPortal.transform.position.y - player.transform.position.x + transform.position.x);
                    }
                    else if (this.facingDirection == FacingDirection.Down && secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y - player.transform.position.y + transform.position.y);
                    }
                    else if (this.facingDirection == FacingDirection.Up && secondPortalFacing == FacingDirection.Up)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y - player.transform.position.y + transform.position.y);
                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.gameObject.tag == "PlayerBlue" || collision.gameObject.tag == "PlayerOrange")
            {
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wallOn, false);
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), oppositeWall, false);
                bool destroyPlayer = false;
                bool destroyCopy = false;
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic)
                {
                    if (this.facingDirection == FacingDirection.Right)
                    {
                        if (collision.transform.position.x < this.transform.position.x)
                        {
                            destroyPlayer = true;
                            if (!teleportedScript.GetComponent<Rigidbody2D>().simulated)
                            {
                                teleportedScript.GetComponent<Rigidbody2D>().simulated = true;
                                if (secondPortalFacing == FacingDirection.Up)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, -player.GetComponent<Rigidbody2D>().velocity.x);
                                else if (secondPortalFacing == FacingDirection.Down)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, player.GetComponent<Rigidbody2D>().velocity.x);
                                else if (secondPortalFacing == FacingDirection.Right)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
                            }
                            Physics2D.IgnoreCollision(teleportedScript.GetComponent<Collider2D>(), wallOn, false);
                            Physics2D.IgnoreCollision(teleportedScript.GetComponent<Collider2D>(), oppositeWall, false);
                        }
                        else if (collision.transform.position.x > this.transform.position.x)
                            destroyCopy = true;
                    }
                    else if (this.facingDirection == FacingDirection.Left)
                    {
                        if (collision.transform.position.x > this.transform.position.x)
                        {
                            destroyPlayer = true;
                            if (!teleportedScript.GetComponent<Rigidbody2D>().simulated)
                            {
                                teleportedScript.GetComponent<Rigidbody2D>().simulated = true;
                                if (secondPortalFacing == FacingDirection.Up)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, player.GetComponent<Rigidbody2D>().velocity.x);
                                else if (secondPortalFacing == FacingDirection.Down)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, -player.GetComponent<Rigidbody2D>().velocity.x);
                                else if (secondPortalFacing == FacingDirection.Left)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
                            }

                        }
                        else if (collision.transform.position.x < this.transform.position.x)
                        {
                            destroyCopy = true;
                        }
                    }
                    else if (this.facingDirection == FacingDirection.Up)
                    {
                        if (collision.transform.position.y < this.transform.position.y)
                        {
                            destroyPlayer = true;
                            Physics2D.IgnoreCollision(teleportedScript.GetComponent<Collider2D>(), wallOn, false);
                            Physics2D.IgnoreCollision(teleportedScript.GetComponent<Collider2D>(), oppositeWall, false);
                            if (!teleportedScript.GetComponent<Rigidbody2D>().simulated)
                            {
                                teleportedScript.GetComponent<Rigidbody2D>().simulated = true;
                                if (secondPortalFacing == FacingDirection.Up)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, -player.GetComponent<Rigidbody2D>().velocity.y);
                            }
                        }
                        else if (collision.transform.position.y > this.transform.position.y)
                            destroyCopy = true;
                    }
                    else if (this.facingDirection == FacingDirection.Down)
                    {
                        if (collision.transform.position.y > this.transform.position.y)
                        {
                            destroyPlayer = true;
                            if (!teleportedScript.GetComponent<Rigidbody2D>().simulated)
                            {
                                teleportedScript.GetComponent<Rigidbody2D>().simulated = true;
                                if (secondPortalFacing == FacingDirection.Down)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, -player.GetComponent<Rigidbody2D>().velocity.y);
                            }
                        }
                        else if (collision.transform.position.y < this.transform.position.y)
                            destroyCopy = true;
                    }
                }
                player.jumpEnabled = true;
                if (destroyPlayer)
                {
                    teleportedCopy.transform.parent = null;
                    teleportedCopy.name = collision.gameObject.name;
                    //pytanie: czy przypisywac nowemu graczowi referencje na portal, ktore wczesniej przechowywal jego oryginal?
                    //
                    Destroy(collision.gameObject); 
                    teleportedCopy.gameObject.GetComponent<playerScript>().mimic = false;
                    teleportedCopy.gameObject.GetComponent<playerScript>().rend.material.name = "Material";
                    teleportedCopy.gameObject.GetComponent<playerScript>().rend.material.SetInt("_CutDirection", 0);
                    teleportedScript.pendHorizontalMovementChange();
                    teleportedCopy.gameObject.GetComponent<playerScript>().networkConnection = collision.gameObject.GetComponent<playerScript>().networkConnection;
                    //zamiana starego obiektu na nowy (jako postać gracza), nowy gameObject jest obsługiwany (ma networkConnection starego)
                    NetworkServer.ReplacePlayerForConnection(
                        collision.gameObject.GetComponent<playerScript>().networkConnection, teleportedCopy, 
                        collision.gameObject.GetComponent<NetworkIdentity>().playerControllerId);
                    
                }
                else if (destroyCopy)
                {
                    Destroy(teleportedCopy);
                    player.rend.material.name = "Material";
                    player.rend.material.SetInt("_CutDirection", 0);
                }
                collision.GetComponent<portalShooting>().enabled = true;
            }
        }
    }
}
