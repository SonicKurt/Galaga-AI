/**********************************************************
 * Bullet Controller
 * 
 * Summary: Controls the behavior of a bullet.
 * 
 * Author: Kurt Campbell
 * Created: 04 April 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;

    // The boundaries to where the bullet can be destroyed
    public float topBounds;
    public float bottomBounds;

    public BulletType Type { get; set; }

    public GameObject Shooter { get; set; }
    
    // Update is called once per frame
    void Update()
    {
        if (Type == BulletType.Alien) {
            transform.position -= Vector3.forward * speed * Time.deltaTime; 
        } else {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }

        if (transform.position.z < bottomBounds || transform.position.z > topBounds) {
            Destroy(this.gameObject);
        }
    }
}
