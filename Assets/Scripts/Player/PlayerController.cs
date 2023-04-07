using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float movementValue;
    public float movementSpeed;
    public float maxProjectiles;
    public GameObject projectileToSpawn;
    public float shootDelay;

    private AudioSource shootSoundEffect;

    private float startTime;
    private bool reload;


    void OnFire() {
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
        movementValue = value.Get<float>();
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
    }

    // Update is called once per frame
    void Update(){
        float horizontalMovement = movementValue * movementSpeed * Time.deltaTime;
        Vector3 pos = transform.position;

        // Clamps the horizontal movement to the left and right boundaries from the origin.
        pos.x = Mathf.Clamp(pos.x + horizontalMovement, -12, 12);
        transform.position = pos;

        if (Time.time > startTime) {
            reload = false;
        }

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
                Destroy(other.gameObject);
                GameManager.Instance.PlayerDead = true;
                Destroy(this.gameObject);
            }   
        }

        if (other.tag == "Goei"
            || other.tag == "Stringer"
            || other.tag == "BossGalaga") {
            AlienController alienController = other.gameObject.GetComponent<AlienController>();
            alienController.DestoryAlien();
            GameManager.Instance.PlayerDead = true;
            Destroy(this.gameObject);
        }
    }
}
