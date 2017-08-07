using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(AudioSource))]
public class portalShooting : NetworkBehaviour {

    enum PlayerColor {Blue, Orange, None};
    private PlayerColor playerColor = PlayerColor.None;
    private audioSync audioS;
    public Transform handgunMarker;
    public GameObject blueProjectilePrefab;
    public GameObject orangeProjectilePrefab;
    private float projectileVelovity;
    //Vector2 cursorPosition = new Vector2(1, 1);
    private AudioSource audioSource;
    [SyncVar]
    public GameObject blueOldPortal;
    [SyncVar]
    public GameObject orangeOldPortal;
    

    // Use this for initialization
    void Start () {
        projectileVelovity = 3000f;
        if(tag == "PlayerBlue")//przypisanie koloru do gracza
            playerColor = PlayerColor.Blue;
        else if(tag == "PlayerOrange")
            playerColor = PlayerColor.Orange;
        audioS = GetComponent<audioSync>();
        
    }
	
	// Update is called once per frame
	void Update () {
        if(isLocalPlayer) // jezeli skrypt jest wykonywany przez lokalnego gracza, sprawdz czy gracz nacisnał klawisz myszki by wystrzelić pocisk
        {
            if (Input.GetMouseButtonDown(0))//prawy przycisk myszy 
            {
                Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pobranie pozycji kursora myszki
                CmdShoot(playerColor, cursorPosition);
                if (playerColor == PlayerColor.Blue)
                    audioS.playSound(0);
                else
                    audioS.playSound(1);
                //Debug.Log("Work Or Not?");
            }
        }
    }



    [Command] //client wysyła żadanie na serwer by ten wykonał następującą motodę -> wystrzelił pocisk
    void CmdShoot(PlayerColor playerColor, Vector2 cursorPosition) //prefiks "Cmd" świadczy o tym że dana metoda może być wykonana tylko przez serwer
    {
        GameObject bullet = null;
        if(playerColor == PlayerColor.Blue)//jeżeli gracz jest niebieski
        {
            //tworzenie kopii pocisku, o początku w obecnej pozycji gracza, i rotacji identycznej do niego (rotacja przy pocisku jest nieistotna jeżeli docelowo pocisk będzie kulą a nie kwadratem)
            bullet = (GameObject)Instantiate(blueProjectilePrefab, handgunMarker.position, Quaternion.identity);
            bullet.name = "blueProjectile"; //nazwa pocisku
        }
        else if(playerColor == PlayerColor.Orange)//jeżeli gracz jest pomaranczowy
        {
            //tworzenie kopii pocisku, o początku w obecnej pozycji gracza, i rotacji identycznej do niego (rotacja przy pocisku jest nieistotna jeżeli doccelowo pocisk będzie kulą a nie kwadratem)
            bullet = (GameObject)Instantiate(orangeProjectilePrefab, handgunMarker.position, Quaternion.identity);
            bullet.name = "orangeProjectile";//nazwa pocisku
        }
        bullet.GetComponent<portalApearing>().active = true;
        //forth to długość wektora jaki został wyznaczony przez naciśnięcie myszki (od srodka gracza - do miejsca naciśnięcia myszki)
        float forth = Mathf.Sqrt((cursorPosition.y - handgunMarker.position.y) * (cursorPosition.y - handgunMarker.position.y) + (cursorPosition.x - handgunMarker.position.x) * (cursorPosition.x - handgunMarker.position.x));
        //nadanie kuli jednostajnej prędkości
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2((cursorPosition.x - handgunMarker.position.x), (cursorPosition.y - handgunMarker.position.y)) * Time.deltaTime * projectileVelovity / forth;
        NetworkServer.Spawn(bullet); // Spawn the bullet on the Clients
                       
    }
}
