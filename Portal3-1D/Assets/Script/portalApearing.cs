using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class portalApearing : NetworkBehaviour {

    private Transform trans;
    public GameObject portal;
    public LayerMask isItWall;
    public float wallOffset = 0.105f;
    //public portalShooting player;
    enum Color{BLUE, ORANGE};
    private Color color;
    
    private string portalName;
    private Vector2 startPosition;

    public bool active;

   	// Use this for initialization
	void Start () {
        trans = GetComponent<Transform>();
        startPosition = new Vector2(trans.position.x, trans.position.y); // zapisanie początkowego położenia kuli
        if (trans.gameObject.tag == "BlueProjectile") //w zaleznosci od koloru pocisku, jest zdeterminowana nazwa portalu
        {
            color = Color.BLUE;
            portalName = "bluePortal";
        }
        else if(trans.gameObject.tag == "OrangeProjectile")
        {
            color = Color.ORANGE;
            portalName = "orangePortal";
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != "Portal collider" && col.tag != "Non portalable")
        {
            Vector2 velocity = GetComponent<Rigidbody2D>().velocity;

            if (IsInLayerMask(col.gameObject.layer, isItWall.value))
            {
                GameObject bluePortal = GameObject.FindWithTag("BluePortal");
                GameObject orangePortal = GameObject.FindWithTag("OrangePortal");
                if (color == Color.BLUE)//usuniecie starego portalu (w zaleznosci, jakiego koloru tworzony jest nowy)
                {
                    if (orangePortal && orangePortal.GetComponent<portalScript>().wall == col.gameObject)
                    {
                        float minDistance = orangePortal.GetComponent<BoxCollider2D>().size.y * orangePortal.transform.localScale.y;
                        if (col.transform.rotation.z == 0)
                        {
                            float distance = Mathf.Abs(trans.position.y - orangePortal.transform.position.y);
                            if (distance < minDistance)
                            {
                                DestroyObject(trans.gameObject);
                                return;
                            }
                        }
                        else
                        {
                            float distance = Mathf.Abs(trans.position.x - orangePortal.transform.position.x);
                            if (distance < minDistance)
                            {
                                DestroyObject(trans.gameObject);
                                return;
                            }
                        }
                    }
                    if (bluePortal)
                    {
                        Destroy(bluePortal);
                        if (orangePortal)
                        {
                            orangePortal.GetComponent<portalScript>().secondPortalFound = false;
                            orangePortal.GetComponent<portalScript>().active = false;
                        }
                    }
                }
                else if (color == Color.ORANGE)
                {
                    if (bluePortal != null && bluePortal.GetComponent<portalScript>().wall == col.gameObject)
                    {
                        float minDistance = bluePortal.GetComponent<BoxCollider2D>().size.y * bluePortal.transform.localScale.y;
                        if (col.transform.rotation.z == 0)
                        {
                            float distance = Mathf.Abs(trans.position.y - bluePortal.transform.position.y);
                            if (distance < minDistance)
                            {
                                DestroyObject(trans.gameObject);
                                return;
                            }
                        }
                        else
                        {
                            float distance = Mathf.Abs(trans.position.x - bluePortal.transform.position.x);
                            if (distance < minDistance)
                            {
                                DestroyObject(trans.gameObject);
                                return;
                            }
                        }
                    }
                    if (orangePortal)
                    {
                        Destroy(orangePortal);
                        if (bluePortal)
                        {
                            bluePortal.GetComponent<portalScript>().secondPortalFound = false;
                            bluePortal.GetComponent<portalScript>().active= false;
                        }
                    }
                }


                if (!isServer) //jeżeli skrypt jest wykonywany przez serwer, obsłuż pociski i wyślij odpowiednie informacje do klientów
                {
                    DestroyObject(trans.gameObject); // usuwanie pocisku
                    return;
                }
                
                if(!active)// jezeli kula nie jest aktywna nie wykonuj dalej, jezeli jest aktywna pozwol stworzyc portal, warunek eliminuje mozliwos stworzenia dwoch portali z jednej kuli
                    return;
                active = false;

                Quaternion rotation = col.gameObject.GetComponent<Transform>().rotation; //pobieranie rotacji obiektu z ktorym zaszla kolizjia (sciana)
                //tworzenie kopii portalu, z uwzglednieniem pozycji w ktorej sie styknal ze scianal i rotacja w jakiej jest sciana (przyjmuje taką samą rotacje)
                GameObject newPortal = (GameObject)Instantiate(portal, trans.position, rotation);
                
                portalScript newPortalScript = newPortal.GetComponent<portalScript>();

                newPortalScript.wall = col.gameObject;
                //newPortalScript.wallOn = col;

                Vector3 position = col.gameObject.GetComponent<Transform>().position; //pobieranie pozycji obiektu z ktorym zaszla kolizja (sciana)
                                                                                      //wykrywanie w jakim kierunku leciała kula względem ściany z którą się zderzyła
                if (rotation.z == 0)
                {
                    if (startPosition.x > position.x)
                    {
                        //Debug.Log("Kula leci w lewo!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Right;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        if (newPortal.transform.position.y < col.transform.GetChild(1).position.y)
                            newPortal.transform.position = new Vector2(col.transform.position.x + wallOffset, col.transform.GetChild(1).position.y);
                        else if (newPortal.transform.position.y > col.transform.GetChild(0).position.y)
                            newPortal.transform.position = new Vector2(col.transform.position.x + wallOffset, col.transform.GetChild(0).position.y);
                        else
                            newPortal.transform.position = new Vector2(col.transform.position.x + wallOffset, newPortal.transform.position.y);
                    }
                    else if (startPosition.x < position.x)
                    {
                        //Debug.Log("Kula leci w prawo!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Left;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                        if (newPortal.transform.position.y < col.transform.GetChild(1).position.y)
                            newPortal.transform.position = new Vector2(col.transform.position.x - wallOffset, col.transform.GetChild(1).position.y);
                        else if (newPortal.transform.position.y > col.transform.GetChild(0).position.y)
                            newPortal.transform.position = new Vector2(col.transform.position.x - wallOffset, col.transform.GetChild(0).position.y);
                        else
                            newPortal.transform.position = new Vector2(col.transform.position.x - wallOffset, newPortal.transform.position.y);
                    }
                }
                else
                {
                    if (startPosition.y > position.y)
                    {
                        //Debug.Log("Kula leci w dół!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Up;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                        if (newPortal.transform.position.x < col.transform.GetChild(0).position.x)
                            newPortal.transform.position = new Vector2(col.transform.GetChild(0).position.x, col.transform.position.y + wallOffset);
                        else if (newPortal.transform.position.x > col.transform.GetChild(1).position.x)
                            newPortal.transform.position = new Vector2(col.transform.GetChild(1).position.x, col.transform.position.y + wallOffset);
                        else
                            newPortal.transform.position = new Vector2(newPortal.transform.position.x, col.transform.position.y + wallOffset);
                    }
                    else if (startPosition.y < position.y)
                    {
                        //Debug.Log("Kula leci w górę!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Down;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
                        if (newPortal.transform.position.x < col.transform.GetChild(0).position.x)
                            newPortal.transform.position = new Vector2(col.transform.GetChild(0).position.x, col.transform.position.y - wallOffset);
                        else if (newPortal.transform.position.x > col.transform.GetChild(1).position.x)
                            newPortal.transform.position = new Vector2(col.transform.GetChild(1).position.x, col.transform.position.y - wallOffset);
                        else
                            newPortal.transform.position = new Vector2(newPortal.transform.position.x, col.transform.position.y - wallOffset);
                    }
                }

                newPortal.name = portalName; // przypisanie nazwy skopiowanemu portalowi
                if (color == Color.BLUE) //przypisanie nowego portalu do posiadanej przez gracza referencji portalu
                {
                    if (GameObject.FindWithTag("PlayerOrange"))
                        GameObject.FindWithTag("PlayerOrange").GetComponent<portalShooting>().blueOldPortal = newPortal;

                    if (GameObject.FindWithTag("PlayerBlue"))
                        GameObject.FindWithTag("PlayerBlue").GetComponent<portalShooting>().blueOldPortal = newPortal;
                }
                else if (color == Color.ORANGE)
                {
                    if (GameObject.FindWithTag("PlayerOrange"))
                        GameObject.FindWithTag("PlayerOrange").GetComponent<portalShooting>().orangeOldPortal = newPortal;

                    if (GameObject.FindWithTag("PlayerBlue"))
                        GameObject.FindWithTag("PlayerBlue").GetComponent<portalShooting>().orangeOldPortal = newPortal;
                }

                NetworkServer.Spawn(newPortal); 
                DestroyObject(trans.gameObject); // usuwanie pocisku
            }
        }
        else if(col.tag == "Non portalable")
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask) //sprawdzenie czy dany 'layer' znajduje się w tych podanych do skryptu ('layermask')
    {
        return layermask == (layermask | (1 << layer));
    }
}
