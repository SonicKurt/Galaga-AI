using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour {
    public float movementValue;
    public float movementSpeed;

    void OnFire() {
        Debug.Log("Pew!");
    }

    void OnMovement(InputValue value) {
        movementValue = value.Get<float>();
        Debug.Log(movementValue);
    }

    void OnOnePlayerGame() {
        Debug.Log("One player game!");
    }

    void OnTwoPlayerGame() {
        Debug.Log("Two player game!");
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
