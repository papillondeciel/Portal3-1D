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
    private void searchForOtherPortal() //funkcja używana do szukania portalu x przez portal y i pobieranie od niego niezbędnych danych
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
                    //wyłączam kolizję gracza ze ścianą na której znajduje się portal
                    Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wall.GetComponent<Collider2D>());
                    if (secondPortalFacing == FacingDirection.Left)
                    {
                        //w następnych warunkach tworzę u ustawiam klon postaci która wychodzi z drugiego portalu,
                        //w zależności od skierowania portalu i położenia ustawiam shader maskujący na klon gracza i jego dzieci (portal gun)

                        if (this.facingDirection == FacingDirection.Up)
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + 0.5f, secondPortal.transform.position.y), Quaternion.identity);
                        else
                            teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + 0.5f, secondPortal.transform.position.y + player.transform.position.y - transform.position.y), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.x);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.x);

                        if (this.facingDirection == FacingDirection.Left)
                        {
                            teleportedScript.invertHorizontalMovement = true;
                            teleportedScript.Flip();
                        }
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
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Right);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.x);
                        if (this.facingDirection == FacingDirection.Right)
                        {
                            teleportedScript.invertHorizontalMovement = true;
                            teleportedScript.Flip();
                        }
                        else if (this.facingDirection == FacingDirection.Up)
                            teleportedScript.thrown = true;


                    }
                    else if (secondPortalFacing == FacingDirection.Up)
                    {
                        teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.y);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.y);

                    }
                    else if (secondPortalFacing == FacingDirection.Down)
                    {
                        teleportedCopy = Instantiate(collision.gameObject, new Vector3(secondPortal.transform.position.x + player.transform.position.x - transform.position.x, secondPortal.transform.position.y + 1f), Quaternion.identity);
                        teleportedScript = teleportedCopy.GetComponent<playerScript>();
                        teleportedScript.rend.material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        teleportedScript.rend.material.SetFloat("_CutPos", secondPortal.transform.position.y);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        teleportedCopy.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.y);
                    }

                    //wyłączam kolizje ze ściąną dla kopii gracza, wyłączam symulacje fizyki, i ustawiam klona jako dziecko gracza
                    Physics2D.IgnoreCollision(teleportedCopy.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>());
                    teleportedCopy.transform.parent = player.gameObject.transform;
                    teleportedScript.GetComponent<Rigidbody2D>().simulated = false;
                    teleportedScript.mimic = true;

                    //tutaj nakładam w zależności od skierowania portalu do którego wchodzi gracz shader maskujący na gracza i jego dzieci (portal gun)
                    if (this.facingDirection == FacingDirection.Right)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Right);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.x);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Right);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.x);
                    }

                    else if (this.facingDirection == FacingDirection.Left)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.x);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Left);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.x);
                    }

                    else if (this.facingDirection == FacingDirection.Up)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.y);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Up);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.y);
                        player.jumpEnabled = false;
                    }

                    else if (this.facingDirection == FacingDirection.Down)
                    {
                        player.rend.material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        player.rend.material.SetFloat("_CutPos", this.transform.position.y);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", (int)FacingDirection.Down);
                        player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetFloat("_CutPos", this.transform.position.y);
                        player.jumpEnabled = false;
                    }

                    //wyłączam możliwość strzelania kiedy postać jest w portalu
                    collision.GetComponent<portalShooting>().enabled = false;
                    teleportedCopy.GetComponent<portalShooting>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)//w tej funkcji pozycjonuje klona w zależności od ułożenia portali i postaci gracza, aby gracz miał wrażenie, że ruchy klona odpowiadają ruchowi postaci
    {
        if (active)
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

    private void OnTriggerExit2D(Collider2D collision)//w tej funkcji obsługuje opuszczenie przez gracza portalu, w zależności od tego czy przez niego przeszedł czy nie usuwana jest kopia lub oryginał
    {
        if (active)
        {
            if (collision.gameObject.tag == "PlayerBlue" || collision.gameObject.tag == "PlayerOrange")
            {
                //spowrotem włączenie kolizji dla gracza ze ścianami portali
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), wall.GetComponent<Collider2D>(), false);
                Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), oppositeWall.GetComponent<Collider2D>(), false);
                bool destroyPlayer = false;
                bool destroyCopy = false;
                playerScript player = collision.gameObject.GetComponent<playerScript>();
                if (!player.mimic)//kontrola czy gracz nie jest kopią
                {
                    //w następnych warunkach ustawiam w zależności od skierowania portali siły działające na kopie opuszczającą portal
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
                    //tutaj następuje podmiana gracza na kopię z przepisaniem zmiennych

                    teleportedCopy.transform.parent = null;
                    teleportedCopy.GetComponent<playerScript>().mimic = false;
                    collision.gameObject.transform.position = teleportedCopy.transform.position;
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(teleportedCopy.GetComponent<Rigidbody2D>().velocity.x, teleportedCopy.GetComponent<Rigidbody2D>().velocity.y);
                    player.thrown = teleportedScript.thrown;
                    player.invertHorizontalMovement = teleportedScript.invertHorizontalMovement;
                    if (player.invertHorizontalMovement && player.GetComponent<NetworkIdentity>().isLocalPlayer)
                        player.CmdFlipCharacter();
                    player.pendHorizontalMovementChange();
                    collision.GetComponent<portalShooting>().enabled = true;
                    player.jumpEnabled = true;
                    player.transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_CutDirection", 0);
                    player.rend.material.SetInt("_CutDirection", 0);
                    Destroy(teleportedCopy);
                }
                else if (destroyCopy)
                {
                    //jeżeli gracz nie przeszedł portalu to usuwana jest jego kopia

                    Destroy(teleportedCopy);
                    player.rend.material.SetInt("_CutDirection", 0);
                    collision.GetComponent<portalShooting>().enabled = true;
                    player.jumpEnabled = true;
                }

            }
        }
    }

}


