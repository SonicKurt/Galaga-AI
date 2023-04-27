/**********************************************************
 * Spawn Controller
 * 
 * Summary: This is the controller to load and launch the aliens
 * into their proper positions.
 * 
 * Author: Kurt Campbell
 * Created: 19 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SpawnerController : MonoBehaviour
{
    // The grid size
    public int gridX;
    public int gridZ;

    // The grid cells' padding offset.
    public float gapSize;

    // Alien properties
    public float alienSpeed;
    public float alienBulletSpeed;
    public float alienSpeedIncrements;
    public float alienShootDelay;

    private float timeDecrement;

    public GameObject stringerObject;
    public GameObject goeiObject;
    public GameObject bossGalagaObject;

    private Transform gridTransform;

    public Transform[] loadSpawners;

    // The alien position grid.
    private Vector3[,] grid;

    private List<GameObject> aliens;
    
    public List<GameObject> Aliens {
        get {
            return aliens;
        }

        set {
            aliens = value;
        }
    }

    private void Awake()
    {
        grid = new Vector3[gridX, gridZ];
        aliens = new List<GameObject>();
        gridTransform = transform;
        timeDecrement = 0;
    }


    public void SpawnAliens()
    {
        GameManager.Instance.Spawning = true;

        // The first position of the grid.
        float startingPosX = gapSize * -4;
        
        // Create the grid to fill all of the final positions for the aliens
        // to go.
        for (int i = 0; i < gridX; i++)
        {
            // Steps into the next vertical position.
            startingPosX = gapSize * -5;
            for (int j = 0; j < gridZ; j++)
            {
                // Creates the spawn position for the alien to be placed within the grid.
                Vector3 spawnPos = new Vector3(startingPosX, 0, i * gapSize) + transform.localPosition;

                grid[i, j] = spawnPos;

                // Steps into the next horizontal position.
                startingPosX += gapSize;
            }
        }

        // Perform the phases to load in the enemies ordely.
        StartCoroutine(loadAliens());
    }

    /// <summary>
    /// Increases the alien's speed and bullet speed.
    /// </summary>
    private void IncreaseAlienSpeed() {
        alienSpeed += alienSpeedIncrements;
        alienBulletSpeed += 1f;
        timeDecrement += 0.25f;
    }

    /// <summary>
    /// Loads the aliens within five phases.
    /// </summary>
    /// <returns></returns>
    private IEnumerator loadAliens()
    {
        // Start in the first phase of loading.
        LoadEnemyState enemyState = LoadEnemyState.Phase1;

        int stage = GameManager.Instance.getCurrentStage();
        bool attackOnLoad = false;
        AgentType alienAgentType = AgentType.Beginner; 

        // For every three stages, increase the alien's speed.
        if (stage % 3 == 0) {
            IncreaseAlienSpeed();
            attackOnLoad = true;
        }

        if (stage >= 3) {
            alienAgentType = AgentType.Average;
        }

        if (stage >= 10) {
            alienAgentType = AgentType.Advanced;
        }

        while (enemyState != LoadEnemyState.Done)
        {
            if (GameManager.Instance.PlayerDead) {
                yield break;
            }

            switch (enemyState)
            {
                // Loads in Goeis and Stringers.
                case LoadEnemyState.Phase1:
                    List<GameObject> aliensToLoad = new List<GameObject>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < 2)
                        {
                            aliensToLoad.Add(SpawnStringer(grid[i, 4]));
                            aliensToLoad.Add(SpawnStringer(grid[i, 5]));
                        } else
                        {
                            aliensToLoad.Add(SpawnGoei(grid[i, 4]));
                            aliensToLoad.Add(SpawnGoei(grid[i, 5]));
                        }
                    }

                    foreach (GameObject alien in aliensToLoad)
                    {
                        AlienController alienController = alien.GetComponent<AlienController>();
                        EnemyType alienType = alienController.Type;

                        if (alienType == EnemyType.Goei)
                        {
                            alien.transform.position = loadSpawners[0].position;
                            alienController.LaunchPad = 0;
                        } else
                        {
                            alien.transform.position = loadSpawners[1].position;
                            alienController.LaunchPad = 1;
                        }

                        // Store alien for the randomizer to pick an alien.
                        aliens.Add(alien);
                    }

                    StartCoroutine(launchAliens(aliensToLoad, EnemyType.Goei, 0, attackOnLoad, alienAgentType));
                    StartCoroutine(launchAliens(aliensToLoad, EnemyType.Stringer, 1, attackOnLoad, alienAgentType));

                    yield return new WaitForSeconds(7f - timeDecrement);
                    enemyState = LoadEnemyState.Phase2;
                    break;

                // Loads in Goeis and Boss Galagas.
                case LoadEnemyState.Phase2:
                    List<GameObject> aliensToLoad2 = new List<GameObject>();
                    for (int i = 4; i > 1; i--)
                    {
                        if (i == 4)
                        {
                            for (int j = 3; j < 7; j++)
                            {
                                aliensToLoad2.Add(SpawnBossGalaga(grid[i, j]));
                            }
                        } else
                        {
                            aliensToLoad2.Add(SpawnGoei(grid[i, 3]));
                            aliensToLoad2.Add(SpawnGoei(grid[i, 6]));
                        }
                    }

                    foreach (GameObject alien in aliensToLoad2)
                    {
                        AlienController alienController = alien.GetComponent<AlienController>();
                        EnemyType alienType = alienController.Type;

                        if (alienType == EnemyType.Goei)
                        {
                            alien.transform.position = loadSpawners[0].position;
                            alienController.LaunchPad = 0;
                        } else
                        {
                            alien.transform.position = loadSpawners[1].position;
                            alienController.LaunchPad = 1;
                        }

                        // Store alien for the randomizer to pick an alien.
                        aliens.Add(alien);
                    }

                    StartCoroutine(launchAliens(aliensToLoad2, EnemyType.Goei, 0, attackOnLoad, alienAgentType));
                    StartCoroutine(launchAliens(aliensToLoad2, EnemyType.BossGalaga, 1, attackOnLoad, alienAgentType));

                    yield return new WaitForSeconds(7f - timeDecrement);
                    enemyState = LoadEnemyState.Phase3;
                    break;

                // Spawns in Goeis.
                case LoadEnemyState.Phase3:
                    List<GameObject> aliensToLoad3 = new List<GameObject>();
                    for (int i = 3; i > 1; i--)
                    {
                        aliensToLoad3.Add(SpawnGoei(grid[i, 1]));
                        aliensToLoad3.Add(SpawnGoei(grid[i, 2]));
                        aliensToLoad3.Add(SpawnGoei(grid[i, 7]));
                        aliensToLoad3.Add(SpawnGoei(grid[i, 8]));
                    }

                    int k = 0;
                    foreach (GameObject alien in aliensToLoad3)
                    {
                        AlienController alienController = alien.GetComponent<AlienController>();
                        EnemyType alienType = alienController.Type;

                        if (k < 4)
                        {
                            alien.transform.position = loadSpawners[0].position;
                            alienController.LaunchPad = 0;
                        } else
                        {
                            alien.transform.position = loadSpawners[1].position;
                            alienController.LaunchPad = 1;
                        }

                        k++;

                        // Store alien for the randomizer to pick an alien.
                        aliens.Add(alien);
                    }

                    StartCoroutine(launchAliens(aliensToLoad3, EnemyType.Goei, 0, attackOnLoad, alienAgentType));
                    StartCoroutine(launchAliens(aliensToLoad3, EnemyType.Goei, 1, attackOnLoad, alienAgentType));

                    yield return new WaitForSeconds(8f - timeDecrement);

                    enemyState = LoadEnemyState.Phase4;
                    break;

                // Spawns in Stringers.
                case LoadEnemyState.Phase4:
                    List<GameObject> aliensToLoad4 = new List<GameObject>();
                    for (int i = 1; i > -1; i--)
                    {
                        aliensToLoad4.Add(SpawnStringer(grid[i, 2]));
                        aliensToLoad4.Add(SpawnStringer(grid[i, 3]));
                        aliensToLoad4.Add(SpawnStringer(grid[i, 6]));
                        aliensToLoad4.Add(SpawnStringer(grid[i, 7]));
                    }

                    int l = 0;
                    foreach (GameObject alien in aliensToLoad4)
                    {
                        AlienController alienController = alien.GetComponent<AlienController>();
                        EnemyType alienType = alienController.Type;

                        if (l < 4)
                        {
                            alien.transform.position = loadSpawners[0].position;
                            alienController.LaunchPad = 0;
                        } else
                        {
                            alien.transform.position = loadSpawners[1].position;
                            alienController.LaunchPad = 1;
                        }

                        l++;
                        // Store alien for the randomizer to pick an alien.
                        aliens.Add(alien);
                    }

                    StartCoroutine(launchAliens(aliensToLoad4, EnemyType.Stringer, 0, attackOnLoad, alienAgentType));
                    StartCoroutine(launchAliens(aliensToLoad4, EnemyType.Stringer, 1, attackOnLoad, alienAgentType));

                    yield return new WaitForSeconds(8f - timeDecrement);

                    enemyState = LoadEnemyState.Phase5;
                    break;

                // Spawns in Stringers again.
                case LoadEnemyState.Phase5:
                    List<GameObject> aliensToLoad5 = new List<GameObject>();
                    for (int i = 1; i > -1; i--)
                    {
                        aliensToLoad5.Add(SpawnStringer(grid[i, 0]));
                        aliensToLoad5.Add(SpawnStringer(grid[i, 1]));
                        aliensToLoad5.Add(SpawnStringer(grid[i, 8]));
                        aliensToLoad5.Add(SpawnStringer(grid[i, 9]));
                    }

                    int m = 0;
                    foreach (GameObject alien in aliensToLoad5)
                    {
                        AlienController alienController = alien.GetComponent<AlienController>();
                        EnemyType alienType = alienController.Type;

                        if (m < 4)
                        {
                            alien.transform.position = loadSpawners[0].position;
                            alienController.LaunchPad = 0;
                        } else
                        {
                            alien.transform.position = loadSpawners[1].position;
                            alienController.LaunchPad = 1;
                        }

                        m++;

                        // Store alien for the randomizer to pick an alien.
                        aliens.Add(alien);
                    }

                    StartCoroutine(launchAliens(aliensToLoad5, EnemyType.Stringer, 0, attackOnLoad, alienAgentType));
                    StartCoroutine(launchAliens(aliensToLoad5, EnemyType.Stringer, 1, attackOnLoad, alienAgentType));

                    yield return new WaitForSeconds(9f - timeDecrement);

                    enemyState = LoadEnemyState.Done;
                    break;
            }
        }

        GameManager.Instance.Spawning = false;
        GameManager.Instance.UpdateGameState(GameState.EnemiesAttack);
    }

    /// <summary>
    /// It is time to launch the aliens to be loaded to their targeted grid position.
    /// </summary>
    /// <param name="alienLoadDeck">The current loading deck of aliens to launch.</param>
    /// <param name="enemyType">The enemy type of the aliens to launch.</param>
    /// <param name="launchPad">The launch pad number to launch from.</param>
    /// <returns>The amount of time to sleep before launching the next alien.</returns>
    IEnumerator launchAliens(List<GameObject> alienLoadDeck, EnemyType enemyType, int launchPad,
        bool attackOnLoad, AgentType modelType)
    {
        Random randomizer = new Random();

        int alienAttack = randomizer.Next(0, alienLoadDeck.Count);

        int i = 0;
        foreach (GameObject alien in alienLoadDeck)
        {
            if (GameManager.Instance.PlayerDead) {
                yield break;
            }

            if (alien != null) {
                AlienController alienController = alien.GetComponent<AlienController>();
                AlienAgent alienAgent = alien.GetComponentInChildren<AlienAgent>();

                if (alienController.Type == enemyType && alienController.LaunchPad == launchPad) {
                    alienController.ReadyToLaunch = true;

                    if (alienAgent != null && modelType != AgentType.Beginner) {
                        alienAgent.ChangeModel(modelType);
                    }

                    yield return new WaitForSeconds(1.5f);

                    if (attackOnLoad && alienController != null) {
                        if (alienAttack == i) {
                            alienController.ShootBullet();
                        }
                    }                   
                }
            }

            i++;
        }
    }

    /// <summary>
    /// Spawn a Goei alien.
    /// </summary>
    /// <param name="spawnPos">The grid spawn position for the alien.</param>
    /// <returns>The newly created Goei alien.</returns>
    GameObject SpawnGoei(Vector3 spawnPos)
    {
        GameObject goei = Instantiate(goeiObject, spawnPos, Quaternion.Euler(-90, 90, 0));
        goei.transform.SetParent(transform);
        AlienController alienController = goei.GetComponent<AlienController>();
        alienController.SpawnPos = spawnPos;
        alienController.Type = EnemyType.Goei;
        alienController.Speed = alienSpeed;
        alienController.BulletSpeed = alienBulletSpeed;
        alienController.ShootDelay = alienShootDelay;
        return goei;
    }

    /// <summary>
    /// Spawn a Stringer alien.
    /// </summary>
    /// <param name="spawnPos">The grid spawn position for the alien.</param>
    /// <returns>The newly created Stringer alien,</returns>
    GameObject SpawnStringer(Vector3 spawnPos)
    {
        GameObject stringer = Instantiate(stringerObject, spawnPos, Quaternion.Euler(-90, 90, 0));
        stringer.transform.SetParent(transform);
        AlienController alienController = stringer.GetComponent<AlienController>();
        alienController.SpawnPos = spawnPos;
        alienController.Type = EnemyType.Stringer;
        alienController.Speed = alienSpeed;
        alienController.BulletSpeed = alienBulletSpeed;
        alienController.ShootDelay = alienShootDelay;
        return stringer;
    }

    /// <summary>
    /// Spawn a Boss Galaga alien.
    /// </summary>
    /// <param name="spawnPos">The grid spawn position for the alien.</param>
    /// <returns>The newly created Boss Galaga alien.</returns>
    GameObject SpawnBossGalaga(Vector3 spawnPos)
    {
        GameObject bossGalaga = Instantiate(bossGalagaObject, spawnPos, Quaternion.Euler(-90, 90, 0));
        bossGalaga.transform.SetParent(transform);
        AlienController alienController = bossGalaga.GetComponent<AlienController>();
        alienController.SpawnPos = spawnPos;
        alienController.Type = EnemyType.BossGalaga;
        alienController.Speed = alienSpeed;
        alienController.BulletSpeed = alienBulletSpeed;
        alienController.ShootDelay = alienShootDelay;
        return bossGalaga;
    }

    /*
    public void RemoveAlien(GameObject alien) {
        aliens.Remove(alien);
    }
    */

    /// <summary>
    /// Clears the entire alien grid.
    /// </summary>
    public void ClearGrid() {
        foreach (GameObject alien in aliens) {
            AlienController alienController = alien.GetComponent<AlienController>();
            alienController.ClearBullets();
            Destroy(alien);
        }

        aliens.Clear();
    }
}
