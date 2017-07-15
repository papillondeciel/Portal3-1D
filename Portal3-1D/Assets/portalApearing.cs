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

   	// Use this for initialization
	void Start () {
        trans = GetComponent<Transform>();
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
        if (IsInLayerMask(col.gameObject.layer, isItWall.value)) //
        {
            if (color == Color.BLUE)//usuniecie starego portalu (w zaleznosci, jakiego koloru tworzony jest nowy)
            {
                if (player.blueOldPortal != null)
                    Destroy(player.blueOldPortal);
            }
            else if (color == Color.ORANGE)
            {
                if (player.orangeOldPortal != null)
                    Destroy(player.orangeOldPortal);
            }

            //tworzenie kopii portalu, z uwzglednieniem pozycji w ktorej sie styknal ze scianal i rotacja w jakiej jest sciana (przyjmuje taką samą rotacje)
            GameObject newPortal = (GameObject)Instantiate(portal, trans.position, col.gameObject.GetComponent<Transform>().rotation);
            newPortal.name = portalName; // przypisanie nazwy skopiowanemu portalowi
            if (color == Color.BLUE) //przypisanie nowego portalu do posiadanej przez gracza referencji portalu
                player.blueOldPortal = newPortal;
            else if (color == Color.ORANGE)
                player.orangeOldPortal = newPortal;
            
            DestroyObject(trans.gameObject); // usuwanie pocisku
        }        
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask) //sprawdzenie czy dany 'layer' znajduje się w tych podanych do skryptu ('layermask')
    {
        return layermask == (layermask | (1 << layer));
    }
}
