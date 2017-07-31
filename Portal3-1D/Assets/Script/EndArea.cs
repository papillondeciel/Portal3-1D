using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndArea : NetworkBehaviour {
    [SyncVar]
    public bool orangeEntered = false;
    [SyncVar]
    public bool blueEntered = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (orangeEntered && blueEntered)
            CmdEndLevel();
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBlue")
        {
            blueEntered = true;
        }
        else if (collision.gameObject.tag == "PlayerOrange")
            orangeEntered = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBlue")
        {
            blueEntered = false;
        }
        else if (collision.gameObject.tag == "PlayerOrange")
            orangeEntered = false;
    }

    [Command]

    void CmdEndLevel()
    {
        NetworkManager.singleton.ServerChangeScene("funBox");
    }
}
