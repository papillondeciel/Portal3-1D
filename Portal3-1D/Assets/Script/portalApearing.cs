using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class portalApearing : NetworkBehaviour {

    private Transform trans;
    public GameObject portal;
    public LayerMask isItWall;

    //public portalShooting player;
    enum Color{BLUE, ORANGE};
    private Color color;
    
    private string portalName;
    private Vector2 startPosition;

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
        if (col.tag != "Portal collider")
        {
            Vector2 velocity = GetComponent<Rigidbody2D>().velocity;

            if (IsInLayerMask(col.gameObject.layer, isItWall.value))
            {
                if (color == Color.BLUE)//usuniecie starego portalu (w zaleznosci, jakiego koloru tworzony jest nowy)
                {
                    if (GameObject.FindWithTag("BluePortal"))
                    {
                        Destroy(GameObject.FindWithTag("BluePortal"));
                        if (GameObject.FindWithTag("OrangePortal"))
                            GameObject.FindWithTag("OrangePortal").GetComponent<portalScript>().secondPortalFound = false;
                    }
                }
                else if (color == Color.ORANGE)
                {
                    if (GameObject.FindWithTag("OrangePortal"))
                    {
                        Destroy(GameObject.FindWithTag("OrangePortal"));
                        if (GameObject.FindWithTag("BluePortal"))
                            GameObject.FindWithTag("BluePortal").GetComponent<portalScript>().secondPortalFound = false;
                    }
                }


                if (!isServer) //jeżeli skrypt jest wykonywany przez serwer, obsłuż pociski i wyślij odpowiednie informacje do klientów
                {
                    DestroyObject(trans.gameObject); // usuwanie pocisku
                    return;
                }


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
                        //Kula leci w lewo!
                        newPortalScript.facingDirection = portalScript.FacingDirection.Right;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        newPortal.transform.position = new Vector2(col.transform.position.x + 0.11f, newPortal.transform.position.y);
                    }
                    else if (startPosition.x < position.x)
                    {
                        //Kula leci w prawo!
                        newPortalScript.facingDirection = portalScript.FacingDirection.Left;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                        newPortal.transform.position = new Vector2(col.transform.position.x - 0.11f, newPortal.transform.position.y);
                    }
                }
                else
                {
                    if (startPosition.y > position.y)
                    {
                        //Kula leci w dół!
                        newPortalScript.facingDirection = portalScript.FacingDirection.Up;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                        newPortal.transform.position = new Vector2(newPortal.transform.position.x, col.transform.position.y + 0.11f);
                    }
                    else if (startPosition.y < position.y)
                    {
                        //Kula leci w górę!
                        newPortalScript.facingDirection = portalScript.FacingDirection.Down;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
                        newPortal.transform.position = new Vector2(newPortal.transform.position.x, col.transform.position.y - 0.11f);
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
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask) //sprawdzenie czy dany 'layer' znajduje się w tych podanych do skryptu ('layermask')
    {
        return layermask == (layermask | (1 << layer));
    }
}
