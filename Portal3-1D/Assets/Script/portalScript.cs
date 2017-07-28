using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class portalScript : NetworkBehaviour {
    public enum FacingDirection { Left = 1, Up = 4, Right = 3, Down = 2 };

    [SyncVar]
    public GameObject wall;
    //public Collider2D wallOn;
    private GameObject oppositeWall;
    [HideInInspector] public GameObject secondPortal;
    private GameObject teleportedCopy;
    private playerScript teleportedScript;
    private ParticleSystem particles;
    [SyncVar]
    public FacingDirection facingDirection;
    public bool active;
    private FacingDirection secondPortalFacing;
    [HideInInspector] public bool secondPortalFound;
    public Sprite tempSprite;
    // Use this for initialization
    void Start() {
        particles = this.transform.GetChild(2).GetComponent<ParticleSystem>();
        searchForOtherPortal();
    }

    // Update is called once per frame
    void Update() {
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
            oppositeWall = secondPortal.GetComponent<portalScript>().wall;
            secondPortalFacing = secondPortal.GetComponent<portalScript>().facingDirection;
            active = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.gameObject.tag == "PlayerBlue" || collision.gameObject.tag == "PlayerOrange")
            {
                Debug.Log("Player exited");
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic) //Sprawdzam czy gracz nie ma na sobie znacznika "mimic", czyli czy nie jest swoją kopią w drugim portalu.
                {
                    Debug.Log("Player entered");
                    Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wall.GetComponent<Collider2D>());
                    if (secondPortalFacing == FacingDirection.Left)
                    {
                        if (this.facingDirection == FacingDirection.Up)
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + 0.5f, secondPortal.transform.position.y), Quaternion.identity);
                        else
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.x);
                        if (this.facingDirection == FacingDirection.Left)
                            teleportedScript.invertHorizontalMovement = true;
                        else if (this.facingDirection == FacingDirection.Up)
                            teleportedScript.thrown = true;

                    }
                    else if (secondPortalFacing == FacingDirection.Right)
                    {
                        if (this.facingDirection == FacingDirection.Up)
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x - 0.5f, secondPortal.transform.position.y), Quaternion.identity);
                        else
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x - 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y), Quaternion.identity);

                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Right);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.x);
                        if (this.facingDirection == FacingDirection.Right)
                            teleportedScript.invertHorizontalMovement = true;
                        else if (this.facingDirection == FacingDirection.Up)
                            teleportedScript.thrown = true;


                    }
                    else if (secondPortalFacing == FacingDirection.Up)
                    {
                        teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y - 1f), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.y);

                    }
                    else if (secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y + 1f), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.y);
                        //if (this.facingDirection == FacingDirection.Right || this.facingDirection == FacingDirection.Left)
                        //    teleportedScript.thrown = true;
                    }

                    Physics2D.IgnoreCollision(teleportedCopy.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>());
                    teleportedCopy.transform.parent = player.gameObject.transform;
                    teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
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
                        player.jumpEnabled = false;
                    }
                    collision.GetComponent<portalShooting>().enabled = false;
                    teleportedCopy.GetComponent<portalShooting>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active /*&& collision.GetComponent<NetworkIdentity>().isLocalPlayer*/)
        {
            if (collision.gameObject.tag == "PlayerBlue" || collision.gameObject.tag == "PlayerOrange")
            {
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic && teleportedCopy != null)
                {
                    if (this.facingDirection == FacingDirection.Up && secondPortalFacing == FacingDirection.Right)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x - player.transform.position.y + transform.position.y, secondPortal.transform.position.y);
                    }
                    else if ((this.facingDirection == FacingDirection.Left && secondPortalFacing == FacingDirection.Right) || (this.facingDirection == FacingDirection.Right && secondPortalFacing == FacingDirection.Left))
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y + player.transform.position.y - transform.position.y);
                    }
                    else if (this.facingDirection == FacingDirection.Up && secondPortalFacing == FacingDirection.Left)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.y - transform.position.y, secondPortal.transform.position.y);
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
                    else if (this.facingDirection == FacingDirection.Up && secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y + player.transform.position.y - transform.position.y);
                    }
                    else if (this.facingDirection == FacingDirection.Down && secondPortalFacing == FacingDirection.Right)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x + player.transform.position.y - transform.position.y, secondPortal.transform.position.y);
                    }
                    else if (this.facingDirection == FacingDirection.Down && secondPortalFacing == FacingDirection.Left)
                    {
                        teleportedCopy.transform.position = new Vector2(secondPortal.transform.position.x - player.transform.position.y + transform.position.y, secondPortal.transform.position.y);
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
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wall.GetComponent<Collider2D>(), false);
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>(), false);
                bool destroyPlayer = false;
                bool destroyCopy = false;
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic)
                {
                    Debug.Log("Player exited");
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
                                else
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
                            }
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
                                else
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
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
                            if (!teleportedScript.GetComponent<Rigidbody2D>().simulated)
                            {
                                teleportedScript.GetComponent<Rigidbody2D>().simulated = true;
                                if (secondPortalFacing == FacingDirection.Up)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, -player.GetComponent<Rigidbody2D>().velocity.y);
                                else if (secondPortalFacing == FacingDirection.Left)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, 0f);
                                else if (secondPortalFacing == FacingDirection.Right)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.GetComponent<Rigidbody2D>().velocity.y, 0f);
                                else
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
                            }
                        }
                        else if (collision.transform.position.y > this.transform.position.y)
                            destroyCopy = true;
                    }
                    else
                    {
                        if (collision.transform.position.y > this.transform.position.y)
                        {
                            destroyPlayer = true;
                            if (!teleportedScript.GetComponent<Rigidbody2D>().simulated)
                            {
                                teleportedScript.GetComponent<Rigidbody2D>().simulated = true;
                                if (secondPortalFacing == FacingDirection.Down)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, -player.GetComponent<Rigidbody2D>().velocity.y);
                                else if (secondPortalFacing == FacingDirection.Right)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.y, 0F);
                                else if (secondPortalFacing == FacingDirection.Left)
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(-player.GetComponent<Rigidbody2D>().velocity.y, 0F);
                                else
                                    teleportedScript.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
                            }

                        }
                        else if (collision.transform.position.y < this.transform.position.y)
                            destroyCopy = true;
                    }
                }
                if (destroyPlayer)
                {
                    teleportedCopy.transform.parent = null;
                    teleportedCopy.name = collision.gameObject.name;
                    teleportedCopy.GetComponent<playerScript>().mimic = false;
                    teleportedCopy.GetComponent<playerScript>().rend.material.name = "Material";
                    teleportedCopy.GetComponent<playerScript>().rend.material.SetInt("_CutDirection", 0);
                    teleportedCopy.GetComponent<portalShooting>().enabled = true;
                    //teleportedScript.pendHorizontalMovementChange();
                    teleportedCopy.GetComponent<playerScript>().networkConnection = collision.gameObject.GetComponent<playerScript>().networkConnection;
                    //teleportedCopy.GetComponent<Rigidbody2D>().velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
                    short playerID = collision.gameObject.GetComponent<NetworkIdentity>().playerControllerId;
                    //zamiana starego obiektu na nowy (jako postać gracza), nowy gameObject jest obsługiwany (ma networkConnection starego)
                    collision.gameObject.transform.position = teleportedCopy.transform.position;
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(teleportedCopy.GetComponent<Rigidbody2D>().velocity.x, teleportedCopy.GetComponent<Rigidbody2D>().velocity.y);
                    player.thrown = teleportedScript.thrown;
                    player.invertHorizontalMovement = teleportedScript.invertHorizontalMovement;
                    player.pendHorizontalMovementChange();
                    collision.GetComponent<portalShooting>().enabled = true;
                    player.jumpEnabled = true;
                    //if (isServer)
                    //{
                    //    NetworkServer.ReplacePlayerForConnection(teleportedCopy.GetComponent<playerScript>().networkConnection, teleportedCopy, playerID);
                    //    //Network.Destroy(collision.gameObject);
                    //    Destroy(collision.gameObject);
                    //}
                    //if (!isServer)
                    Destroy(teleportedCopy);
                }
                else if (destroyCopy)
                {
                    Destroy(teleportedCopy);
                    player.rend.material.name = "Material";
                    player.rend.material.SetInt("_CutDirection", 0);
                    collision.GetComponent<portalShooting>().enabled = true;
                    player.jumpEnabled = true;
                }

            }
        }
    }

}


