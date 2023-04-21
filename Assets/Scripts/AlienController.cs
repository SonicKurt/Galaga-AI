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
using UnityEngine.InputSystem;
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

    private float bulletSpeed;

    // The current lanch pad to launch this alien.
    private int launchPad;

    // Is the alien ready to attack.
    private bool attack;

    // Should the alien retreat and go back to its original position.
    private bool resetToPosition;

    // The grid spawner controller.
    private SpawnerController spawnerController;

    // Bullet prefab for the alien.
    public GameObject bulletPrefab;

    // Randomizer instance.
    private Random randomizer;

    // The times the alien shot bullets.
    private int timesShot;

    // The collection of bullets that the alien instantiated.
    private List<GameObject> bullets;

    private AudioSource dieSoundEffect;

    // The attack start time counter.
    private float startTime;

    // A counter for how many hits that the alien has been taken.
    // NOTE: This is only used for Boss Galagas.
    private float hitCounter;

    // The value for the horizonal movement.
    public float HorizontalInput { get; set; }

    // Alien shooting delay.
    public float ShootDelay { get; set; }

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

    public float BulletSpeed {
        get {
            return bulletSpeed;
        }

        set {
            bulletSpeed = value;
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
        randomizer = new Random();
        timesShot = 0;
        dieSoundEffect = GetComponent<AudioSource>();
        startTime = Time.time;
        bullets = new List<GameObject>();
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

                //ClearBullets();
                timesShot = 0;
                
            } else {
                Vector3 pos = transform.position;

                pos -= new Vector3(Mathf.Clamp(HorizontalInput, -12f, 12f), 0, 1) * speed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, -12f, 12f);

                transform.position = pos;

                if (!GameManager.Instance.training) {
                    int timeToShoot = randomizer.Next(0, 2);
                    if (timeToShoot == 1 && timesShot == 0) {
                        StartCoroutine(Shoot());
                        timesShot++;
                    }
                }
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

    /// <summary>
    /// Shoots an alien bullet if we are training the alien AIs.
    /// </summary>
    public void ShootBullet() {
        if (Time.time > startTime && timesShot < 3) {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.Shooter = this.gameObject;
            bulletController.Type = BulletType.Alien;
            startTime = Time.time + ShootDelay;
            bullets.Add(bullet);
            timesShot++;
        }
    }

    /// <summary>
    /// Instantiate new bullet objects for the alien.
    /// </summary>
    /// <returns>A time to timeout to instantiate the next bullet</returns>
    private IEnumerator Shoot() {
        yield return new WaitForSeconds(0.7f);
        int amountOfBullets = randomizer.Next(1, 4);

        for (int i = 0; i < amountOfBullets; i++) {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.Shooter = this.gameObject;
            bulletController.Type = BulletType.Alien;
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Clears the bullets from the scene if the player had died.
    /// </summary>
    public void ClearBullets() {
        if (bullets != null) {
            foreach (GameObject bullet in bullets) {
                Destroy(bullet);
            }

            bullets.Clear();
        }
    }

    public void DestoryAlien() {
        if (attack) {
            GameManager.Instance.RemoveAlienAttacking(this.gameObject);
        }

        GameManager.Instance.RemoveAlien(this.gameObject);
        
        dieSoundEffect.Play();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet") {
            BulletController bullet = other.GetComponent<BulletController>();

            if (bullet.Type != BulletType.Alien) {
                GameObject player = bullet.Shooter;

                if (player != null && GameManager.Instance.training) {
                    PlayerAgent playerAgent = player.GetComponentInChildren<PlayerAgent>();
                    playerAgent.AddReward(1f / GameManager.Instance.getAlienCount());
                }

                // Boss Galagas take two hits to be destroyed.
                // Other aliens (i.e., Goeis or Stringers) only take one hit.
                if (Type == EnemyType.BossGalaga && hitCounter != 1) {
                    hitCounter++;
                } else {
                    GameManager.Instance.UpdateScore(type, attack);
                    DestoryAlien();
                }

                Destroy(other.gameObject);
            }
        }
    }
}
