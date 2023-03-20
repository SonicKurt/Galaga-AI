/**********************************************************
 * Alien Controller
 * 
 * Summary: Controls the behavior of the alien.
 * 
 * Author: Kurt Campbell
 * Date: 19 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    // The alien's grid position.
    private Vector3 spawnPos;

    // The type of alien.
    private EnemyType type;

    // Is the alien ready to launch.
    private bool readyToLaunch;

    // The movement of the alien.
    private float speed;

    // The current lanch pad to launch this alien.
    private int launchPad;

    public EnemyType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public Vector3 SpawnPos {
        get {
            return spawnPos;
        }

        set {
            spawnPos = value;
        }
    }

    public bool ReadyToLaunch
    {
        get
        {
            return readyToLaunch;
        }

        set
        {
            readyToLaunch = value;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public int LaunchPad
    {
        get
        {
            return launchPad;
        }

        set
        {
            launchPad = value;
        }
    }
    
    void Awake()
    {
        readyToLaunch = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Launches to its grid position.
        if (readyToLaunch)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, spawnPos, step);

            if (Vector3.Distance(transform.position, spawnPos) < 0.001f)
            {
                readyToLaunch = false;
            }
        }
    }
}
