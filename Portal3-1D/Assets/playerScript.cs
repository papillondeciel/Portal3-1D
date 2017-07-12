using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour {

    private Transform trans;

    // Use this for initialization
    void Start () {
        trans = GetComponent<Transform>(); //pobieranie cech komponentu Transform
    }
	
	// Update is called once per frame
	void Update () {
        // Transformacje położenie w zależności od naciśniętego klawisza
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            trans.Translate(-0.25f, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            trans.Translate(0.25f, 0, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            trans.Translate(0, 0.25f, 0);
        }

        //rotacja postaci, jezeli jest przekrzywiona
        //Do poprawy poniewaz obecnie postac drga -.- przy malych bardzo kontach, mniejszych niz kat obrotu (grrr)
        if (trans.rotation.z > 0)
        {
            trans.Rotate(0, 0, -2);
        }
        else if(trans.rotation.z < 0)
        {
            trans.Rotate(0, 0, 2);
        }
    }
}
