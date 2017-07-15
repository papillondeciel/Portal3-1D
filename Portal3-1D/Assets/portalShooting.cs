﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalShooting : MonoBehaviour {

    private Transform playerTrans;

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
        projectileVelovity = 1000f;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0)) //lewy przycisk myszy
        {
            cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //pobranie pozycji kursora myszki
            //tworzenie kopii pocisku, o początku w obecnej pozycji gracza, i rotacji identycznej do niego (rotacja przy pocisku jest nieistotna jeżeli doccelowo pocisk będzie kulą a nie kwadratem)
            GameObject bullet = (GameObject)Instantiate(blueProjectilePrefab, playerTrans.position, Quaternion.identity);        
            bullet.name = "blueProjectile"; //nazwa pocisku  
            BlueProjectiles.Add(bullet); //dodanie pocisku do listy pociskow konkretnego koloru
            //forth to długość wektora jaki został wyznaczony przez naciśnięcie myszki (od srodka gracza - do miejsca naciśnięcia myszki)
            float forth = Mathf.Sqrt((cursorPosition.y - playerTrans.position.y) * (cursorPosition.y - playerTrans.position.y) + (cursorPosition.x - playerTrans.position.x) * (cursorPosition.x - playerTrans.position.x));
            //nadanie kuli jednostajnej prędkości
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2((cursorPosition.x - playerTrans.position.x), (cursorPosition.y - playerTrans.position.y)) * Time.deltaTime * projectileVelovity / forth;
            
        }


        else if (Input.GetMouseButtonDown(1))//prawy przycisk myszy
        {
            cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//pobranie pozycji kursora myszki
            //tworzenie kopii pocisku, o początku w obecnej pozycji gracza, i rotacji identycznej do niego (rotacja przy pocisku jest nieistotna jeżeli doccelowo pocisk będzie kulą a nie kwadratem)
            GameObject bullet = (GameObject)Instantiate(orangeProjectilePrefab, playerTrans.position, Quaternion.identity);
            bullet.name = "orangeProjectile";//nazwa pocisku  
            OrangeProjectiles.Add(bullet);//dodanie pocisku do listy pociskow konkretnego koloru
            //forth to długość wektora jaki został wyznaczony przez naciśnięcie myszki (od srodka gracza - do miejsca naciśnięcia myszki)
            float forth = Mathf.Sqrt((cursorPosition.y - playerTrans.position.y) * (cursorPosition.y - playerTrans.position.y) + (cursorPosition.x - playerTrans.position.x) * (cursorPosition.x - playerTrans.position.x));
            //nadanie kuli jednostajnej prędkości
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2((cursorPosition.x - playerTrans.position.x), (cursorPosition.y - playerTrans.position.y)) * Time.deltaTime * projectileVelovity / forth;
        }
                

        for (int i = 0; i < BlueProjectiles.Count; i++)//dla każdej kuli wykonaj
        {
            GameObject goBullet = BlueProjectiles[i]; 
            if (goBullet != null) //jeżeli kula jest na scenie
            {
                Vector2 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet.transform.position); //pobranie pozycji kuli względem ekranu
                //jeżeli kula wyleciała za ekran, usuń ją ze sceny oraz z listy
                if (bulletScreenPos.y >= Screen.height || bulletScreenPos.y <= 0 || bulletScreenPos.x >= Screen.width || bulletScreenPos.x <= 0)
                {
                    DestroyObject(goBullet);
                    BlueProjectiles.Remove(goBullet);
                }
            }
            else //jeżeli nie ma już tej kuli na scenie, usuń ją z listy
            {
                BlueProjectiles.Remove(goBullet);
            }
        }

        for (int i = 0; i < OrangeProjectiles.Count; i++)//dla każdej kuli wykonaj
        {
            GameObject goBullet = OrangeProjectiles[i];
            if (goBullet != null) //jeżeli kula jest na scenie
            {
                Vector2 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet.transform.position); //pobranie pozycji kuli względem ekranu
                //jeżeli kula wyleciała za ekran, usuń ją ze sceny oraz z listy
                if (bulletScreenPos.y >= Screen.height || bulletScreenPos.y <= 0 || bulletScreenPos.x >= Screen.width || bulletScreenPos.x <= 0)
                {
                    DestroyObject(goBullet);
                    OrangeProjectiles.Remove(goBullet);
                }
            }
            else //jeżeli nie ma już tej kuli na scenie, usuń ją z listy
            {
                OrangeProjectiles.Remove(goBullet);
            }
        }
    }
}