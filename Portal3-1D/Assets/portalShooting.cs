using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalShooting : MonoBehaviour {

    private Transform playerTrans;
    ///public GameObject newPortal;
    //public Transform portalTrans;

    public GameObject blueProjectilePrefab;
    public GameObject orangeProjectilePrefab;
    
    private List<GameObject> BlueProjectiles = new List<GameObject>();
    private List<GameObject> OrangeProjectiles = new List<GameObject>();


    private float projectileVelovity;
    Vector2 cursorPosition = new Vector2(1, 1);

    public GameObject blueOldPortal;
    public GameObject orangeOldPortal;


    // Use this for initialization
    void Start () {
        playerTrans = GetComponent<Transform>();
        projectileVelovity = 750f;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject bullet = (GameObject)Instantiate(blueProjectilePrefab, playerTrans.position, Quaternion.identity);        
            bullet.name = "blueProjectile";       
            BlueProjectiles.Add(bullet);
            float forth = Mathf.Sqrt((cursorPosition.y - playerTrans.position.y) * (cursorPosition.y - playerTrans.position.y) + (cursorPosition.x - playerTrans.position.x) * (cursorPosition.x - playerTrans.position.x));
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2((cursorPosition.x - playerTrans.position.x), (cursorPosition.y - playerTrans.position.y)) * Time.deltaTime * projectileVelovity / forth;
        }


        else if (Input.GetMouseButtonDown(1))
        {
            cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject bullet = (GameObject)Instantiate(orangeProjectilePrefab, playerTrans.position, Quaternion.identity);
            bullet.name = "orangeProjectile";
            OrangeProjectiles.Add(bullet);
            float forth = Mathf.Sqrt((cursorPosition.y - playerTrans.position.y) * (cursorPosition.y - playerTrans.position.y) + (cursorPosition.x - playerTrans.position.x) * (cursorPosition.x - playerTrans.position.x));
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2((cursorPosition.x - playerTrans.position.x), (cursorPosition.y - playerTrans.position.y)) * Time.deltaTime * projectileVelovity / forth;
        }
                

        for (int i = 0; i < BlueProjectiles.Count; i++)
        {
            GameObject goBullet = BlueProjectiles[i];
            if (goBullet != null)
            {
                Vector2 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet.transform.position);
                if (bulletScreenPos.y >= Screen.height || bulletScreenPos.y <= 0 || bulletScreenPos.x >= Screen.width || bulletScreenPos.x <= 0)
                {
                    DestroyObject(goBullet);
                    BlueProjectiles.Remove(goBullet);
                }
            }
            else
            {
                BlueProjectiles.Remove(goBullet);
            }
        }

        for (int i = 0; i < OrangeProjectiles.Count; i++)
        {
            GameObject goBullet = OrangeProjectiles[i];
            if (goBullet != null)
            {
                Vector2 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet.transform.position);
                if (bulletScreenPos.y >= Screen.height || bulletScreenPos.y <= 0 || bulletScreenPos.x >= Screen.width || bulletScreenPos.x <= 0)
                {
                    DestroyObject(goBullet);
                    OrangeProjectiles.Remove(goBullet);
                }
            }
            else
            {
                OrangeProjectiles.Remove(goBullet);
            }
        }
    }
}
