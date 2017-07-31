using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cameraOperator : NetworkBehaviour {

    private GameObject camera;// = null;
    private Transform topMaker;
    private Transform rightMaker;
    private Transform downMaker;
    private Transform leftMaker;

    private Transform player;
    private float maxSpeed;

    void Start ()
    {
        camera = GameObject.FindWithTag("MainCamera");
        AssignMaker(camera.transform.GetChild(0));
        AssignMaker(camera.transform.GetChild(1));
        AssignMaker(camera.transform.GetChild(2));
        AssignMaker(camera.transform.GetChild(3));
        player = this.gameObject.transform;
        maxSpeed = Mathf.Abs(GetComponent<playerScript>().maxSpeed)*0.8f;
    }


    void Update()
    {
        SetTheCamera();
    }


    public void SetTheCamera()
    {
        if(isLocalPlayer)
        {
            if(player.position.x >= rightMaker.position.x) 
                camera.transform.Translate(Vector3.right * Time.deltaTime * maxSpeed, Space.World); 
            else if(player.position.x <= leftMaker.position.x) 
                camera.transform.Translate(Vector3.left * Time.deltaTime * maxSpeed, Space.World); 

            if(player.position.y >= topMaker.position.y)
                camera.transform.Translate(Vector3.up * Time.deltaTime * maxSpeed, Space.World); 
            else if(player.position.y <= downMaker.position.y) 
                camera.transform.Translate(Vector3.down * Time.deltaTime * maxSpeed, Space.World); 
        }
    }


    private void AssignMaker(Transform child)
    {
        switch (child.gameObject.tag)
        {
            case "TopMaker":
                topMaker = child;
                break;
            case "RightMaker":
                rightMaker = child;
                break;
            case "DownMaker":
                downMaker = child;
                break;
            case "LeftMaker":
                leftMaker = child;
                break;
        }
    }

}
