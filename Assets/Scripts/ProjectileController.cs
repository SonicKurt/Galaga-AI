using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    private float speed = 20.0f;
    private String targetTag = "";
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (transform.position.z >= 25.0f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.compareTag(targetTag)) {
            Destroy(other.gameObject);
        }
    }
}