using UnityEngine;
using UnityEngine.Networking;

public class networkManager : NetworkManager {
    
    public GameObject playerBlue;
    public Transform playerBlueTransform;
    public GameObject playerOrange;
    public Transform playerOrangeTransform;
    private Transform playerPrefabTransform;
    private enum Player { Blue, Orange };
    private Player colorPlayer = Player.Blue;


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (colorPlayer == Player.Blue)
        {
            Debug.Log("OnServerAddPlayer BLUE!");
            playerPrefab = playerBlue;
            playerPrefabTransform = playerBlueTransform;
            colorPlayer = Player.Orange;
        }
        else if (colorPlayer == Player.Orange)
        {
            Debug.Log("OnServerAddPlayer ORANGE!");
            playerPrefab = playerOrange;
            playerPrefabTransform = playerOrangeTransform;
        }

        var player = (GameObject)GameObject.Instantiate(playerPrefab, playerPrefabTransform.position, Quaternion.identity);
        player.GetComponent<playerScript>().networkConnection = conn;

        if (GameObject.FindWithTag("OrangePortal"))
            player.GetComponent<portalShooting>().orangeOldPortal = GameObject.FindWithTag("OrangePortal");
        else
            player.GetComponent<portalShooting>().orangeOldPortal = null;

        if (GameObject.FindWithTag("BluePortal"))
            player.GetComponent<portalShooting>().blueOldPortal = GameObject.FindWithTag("BluePortal");
        else
            player.GetComponent<portalShooting>().blueOldPortal = null;

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnStopServer()
    {
        colorPlayer = Player.Blue;
    }
}
