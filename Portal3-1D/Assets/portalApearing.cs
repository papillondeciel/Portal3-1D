using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalApearing : MonoBehaviour {

    private Transform trans;
    public GameObject portal;
    public LayerMask isItWall;

    public portalShooting player;
    enum Color{BLUE, ORANGE};
    private Color color;
    
    private string portalName;
    private Vector2 startPosition;

   	// Use this for initialization
	void Start () {
        trans = GetComponent<Transform>();
        Vector2 startPosition = new Vector2(trans.position.x, trans.position.y); // zapisanie początkowego położenia kuli
        if (trans.gameObject.name == "blueProjectile") //w zaleznosci od koloru pocisku, jest zdeterminowana nazwa portalu
            {
                color = Color.BLUE;
                portalName = "bluePortal";
            }
        else if(trans.gameObject.name == "orangeProjectile")
            {
                color = Color.ORANGE;
                portalName = "orangePortal";
            }
   
    }
	
	// Update is called once per frame
	void Update () {
		
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
                    if (player.blueOldPortal != null)
                    {
                        Destroy(player.blueOldPortal);
                        if(player.orangeOldPortal != null)
                        player.orangeOldPortal.GetComponent<portalScript>().secondPortalFound = false;
                    }
                }
                else if (color == Color.ORANGE)
                {
                    if (player.orangeOldPortal != null)
                    {
                        Destroy(player.orangeOldPortal);
                        if (player.blueOldPortal != null)
                            player.blueOldPortal.GetComponent<portalScript>().secondPortalFound = false;
                    }
                }

                Quaternion rotation = col.gameObject.GetComponent<Transform>().rotation; //pobieranie rotacji obiektu z ktorym zaszla kolizjia (sciana)
                                                                                         //tworzenie kopii portalu, z uwzglednieniem pozycji w ktorej sie styknal ze scianal i rotacja w jakiej jest sciana (przyjmuje taką samą rotacje)
                GameObject newPortal = (GameObject)Instantiate(portal, trans.position, rotation);
                portalScript newPortalScript = newPortal.GetComponent<portalScript>();

                newPortalScript.wallOn = col;

                Vector3 position = col.gameObject.GetComponent<Transform>().position; //pobieranie pozycji obiektu z ktorym zaszla kolizjia (sciana)
                                                                                      //wykrywanie w jakim kierunku leciała kula względem ściany z którą się zderzyła
                if (rotation.z == 0)
                {
                    if (startPosition.x > position.x)
                    {
                        Debug.Log("Kula leci w lewo!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Right;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        newPortal.transform.position = new Vector2(col.transform.position.x + 0.11f, newPortal.transform.position.y);
                    }
                    else if (startPosition.x < position.x)
                    {
                        Debug.Log("Kuls leci w prawo!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Left;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                        newPortal.transform.position = new Vector2(col.transform.position.x - 0.11f, newPortal.transform.position.y);
                    }
                }
                else
                {
                    if (startPosition.y > position.y)
                    {
                        Debug.Log("Kula leci w dół!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Up;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                        newPortal.transform.position = new Vector2(newPortal.transform.position.x, col.transform.position.y + 0.11f);
                    }
                    else if (startPosition.y < position.y)
                    {
                        Debug.Log("Kula leci w górę!");
                        newPortalScript.facingDirection = portalScript.FacingDirection.Down;
                        newPortal.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
                        newPortal.transform.position = new Vector2(newPortal.transform.position.x, col.transform.position.y - 0.11f);
                    }
                }

                newPortal.name = portalName; // przypisanie nazwy skopiowanemu portalowi
                if (color == Color.BLUE) //przypisanie nowego portalu do posiadanej przez gracza referencji portalu
                    player.blueOldPortal = newPortal;
                else if (color == Color.ORANGE)
                    player.orangeOldPortal = newPortal;

                DestroyObject(trans.gameObject); // usuwanie pocisku
            }
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask) //sprawdzenie czy dany 'layer' znajduje się w tych podanych do skryptu ('layermask')
    {
        return layermask == (layermask | (1 << layer));
    }
}
