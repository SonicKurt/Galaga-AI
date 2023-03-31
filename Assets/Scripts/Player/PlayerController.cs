using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float movementValue;
    public float movementSpeed;

    void OnFire() {
        Debug.Log("Pew!");
    }

    void OnMovement(InputValue value) {
        movementValue = value.Get<float>();
        Debug.Log(movementValue);
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
    }
}
