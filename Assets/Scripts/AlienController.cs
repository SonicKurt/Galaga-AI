/**********************************************************
 * Alien Controller
 * 
 * Summary: Controls the behavior of the alien.
 * 
 * Author: Kurt Campbell
 * Created: 19 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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

    // Is the alien ready to attack.
    private bool attack;

    // Should the alien retreat and go back to its original position.
    private bool resetToPosition;

    // The grid spawner controller.
    private SpawnerController spawnerController;

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

    public bool Attack {
        get {
            return attack;
        }

        set {
            attack = value;
        }
    }

    public bool ResetToPosition {
        get {
            return resetToPosition;
        }

        set {
            resetToPosition = value;
        }
    }
    
    void Awake()
    {
        readyToLaunch = false;
        resetToPosition = false;
        GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
        spawnerController = spawner.GetComponent<SpawnerController>();
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

        // Attack
        if (attack) {
            if (transform.position.z < -9f) {
                Transform launchPad = spawnerController.loadSpawners[0].transform;

                transform.position = new Vector3(transform.position.x,
                    transform.position.y,
                    launchPad.position.z);

                attack = !resetToPosition;
            } else {
                transform.position -= Vector3.forward * speed * Time.deltaTime;
            }
        }

        // Reset the position if the alien is ready to not attack.
        if (!attack && resetToPosition) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, spawnPos, step);

            if (Vector3.Distance(transform.position, spawnPos) < 0.001f) {
                readyToLaunch = false;
                resetToPosition = false;
            }
        }
    }
}
