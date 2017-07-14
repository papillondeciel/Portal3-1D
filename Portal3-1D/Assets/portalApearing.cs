using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalApearing : MonoBehaviour {

    private Transform trans;
    //private Rigidbody2D rb2d;
    public GameObject portal;
    public LayerMask isItWall;

    public portalShooting player;
    enum Color{BLUE, ORANGE};
    private Color color;
    
    private string portalName;

   	// Use this for initialization
	void Start () {
        trans = GetComponent<Transform>();
        //rb2d = GetComponent<Rigidbody2D>();
        if (trans.gameObject.name == "blueProjectile")
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
        if (IsInLayerMask(col.gameObject.layer, isItWall.value))
        {
            //Destroy(GameObject.Find(portalName));
            if (color == Color.BLUE)
            {
                if (player.blueOldPortal != null)
                    Destroy(player.blueOldPortal);
            }
            else if (color == Color.ORANGE)
            {
                if (player.orangeOldPortal != null)
                    Destroy(player.orangeOldPortal);
            }
            
            GameObject newPortal = (GameObject)Instantiate(portal, trans.position, Quaternion.identity);
            if (color == Color.BLUE)
                player.blueOldPortal = newPortal;
            else if (color == Color.ORANGE)
                player.orangeOldPortal = newPortal;
            newPortal.name = portalName;
            DestroyObject(trans.gameObject);
        }        
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
