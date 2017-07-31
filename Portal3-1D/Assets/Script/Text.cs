using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text : MonoBehaviour {
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 100;
        GUI.Label(new Rect( 400, 300, 100, 200), "You won!", style);
    }
}
