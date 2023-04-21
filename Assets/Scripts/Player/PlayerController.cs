/**********************************************************
 * Player Controller
 * 
 * Summary: Controls the behavior of the player.
 * 
 * Authors: Jackson Isenhower, Kurt Campbell
 * Created: 21 March 2023
 * 
 * Copyright Cedarville University, Kurt Campbell, Jackson Isenhower,
 * Donald Osborn.
 * All rights reserved.
 *********************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    public float maxProjectiles;
    public GameObject projectileToSpawn;
    public float shootDelay;

    private AudioSource shootSoundEffect;

    private float startTime;
    private bool reload;

    private PlayerAgent agent;

    // Indicate that the player is in training mode.
    public bool Training { get; set; }

    public float HorizontalInput { get; set; }

    public void OnFire() {
        if (!reload) {
            shootSoundEffect.Play();
            GameObject projectile = Instantiate(projectileToSpawn, transform.position + new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0));
            BulletController bulletController = projectile.GetComponent<BulletController>();
            bulletController.Type = BulletType.Player;
            reload = true;
            startTime = Time.time + shootDelay;
        }
    }

    void OnMovement(InputValue value) {
        HorizontalInput = value.Get<float>();
    }

    void OnPause() {
        Debug.Log("Game paused!");
    }

    void OnExit() {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        shootSoundEffect = GetComponent<AudioSource>();
        startTime = Time.time + shootDelay;
        reload = true;
        agent = GetComponentInChildren<PlayerAgent>();
    }

    // Update is called once per frame
    void Update() {
        
        float horizontalMovement = HorizontalInput * movementSpeed * Time.deltaTime;
        Vector3 pos = transform.position;

        // Clamps the horizontal movement to the left and right boundaries from the origin.
        pos.x = Mathf.Clamp(pos.x + horizontalMovement, -12, 12);
        transform.position = pos;

        if (Time.time > startTime) {
            reload = false;
        }

        agent.AddReward(-1f / agent.MaxStep);
    }

    /// <summary>
    /// When the player collides with an alien bullet or one of the aliens,
    /// the player must die.
    /// </summary>
    /// <param name="other">The object that corresponds to either an
    /// alien or one of its bullets.</param>
    private void OnTriggerEnter(Collider other)
    {
         if (other.tag == "Bullet") {
            BulletController bulletController = other.GetComponent<BulletController>();
            if (bulletController.Type == BulletType.Alien) {
                GameObject alien = bulletController.Shooter;

                // If the alien's bullet attacked the player in training mode,
                // add a reward to the alien agent.
                if (GameManager.Instance.training) {
                    if (alien != null) {
                        AlienAgent alienAgent = alien.GetComponentInChildren<AlienAgent>();
                        alienAgent.AddReward(1f);
                    }

                    GameManager.Instance.removeAllBulletsFromScene();
                    Destroy(other.gameObject);
                    GameManager.Instance.PlayerDead = true;

                    PlayerAgent playerAgent = GetComponentInChildren<PlayerAgent>();
                    GameManager.Instance.StopAllCoroutines();
                    GameManager.Instance.UpdateGameState(GameState.ResetEpisode);
                    playerAgent.AddReward(-1f);
                    playerAgent.EndEpisode();
                } else {
                    Destroy(other.gameObject);
                    GameManager.Instance.PlayerDead = true;
                    Destroy(this.gameObject);
                }
                

                return;
            }   
        }

        if (other.tag == "Goei"
            || other.tag == "Stringer"
            || other.tag == "BossGalaga") {
            AlienController alienController = other.gameObject.GetComponent<AlienController>();

            // Gets reward if the alien crashes into player.
            AlienAgent alienAgent = other.gameObject.GetComponentInChildren<AlienAgent>();
            alienAgent.AddReward(1f);

            alienController.DestoryAlien();
            GameManager.Instance.PlayerDead = true;

            // The player agent needs to be present at all times.
            if (GameManager.Instance.training) {
                PlayerAgent playerAgent = GetComponentInChildren<PlayerAgent>();
                GameManager.Instance.StopAllCoroutines();
                GameManager.Instance.UpdateGameState(GameState.ResetEpisode);
                GameManager.Instance.removeAllBulletsFromScene();
                playerAgent.AddReward(-1f / agent.MaxStep);
                playerAgent.EndEpisode();
            } else {
                Destroy(this.gameObject);
            }

            return;
        }
    }
}
