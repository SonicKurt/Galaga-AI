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
}
