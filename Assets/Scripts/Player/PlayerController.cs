using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float movementValue;
    public float movementSpeed;
    public float maxProjectiles;
    public GameObject projectileToSpawn;

    void OnFire() {
        GameObject projectile = Instantiate(projectileToSpawn, transform.position + new Vector3(0,0,1), Quaternion.Euler(0,0,0));
    }

    void OnMovement(InputValue value) {
        movementValue = value.Get<float>();
    }

    void OnPause() {
        Debug.Log("Game paused!");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        transform.Translate(new Vector3(movementValue, 0, 0) * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// When the player collides with an alien bullet or one of the aliens,
    /// the player must die.
    /// </summary>
    /// <param name="other">The object that corresponds to either an
    /// alien or one of its bullets.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AlienBullet"
            || other.tag == "Goei"
            || other.tag == "Stringer"
            || other.tag == "BossGalaga") {
            GameManager.Instance.PlayerDead = true;
            Destroy(this.gameObject);
        }

        if (other.tag == "Goei"
            || other.tag == "Stringer"
            || other.tag == "BossGalaga") {
            AlienController alienController = other.gameObject.GetComponent<AlienController>();
            alienController.DestoryAlien();
        }
    }
}
