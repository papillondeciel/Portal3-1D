using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handgun : MonoBehaviour {

    public GameObject Player;
    Vector3 left = new Vector3(-0.21f, 0.5f, 0);
    Vector3 right = new Vector3(-0.21f, -0.5f, 0);
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.SetPositionAndRotation(Player.transform.position + left, transform.rotation);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {

            transform.SetPositionAndRotation(Player.transform.position - right , transform.rotation);

        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
	}
}
