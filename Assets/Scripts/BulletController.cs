using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;

    public float topBounds;
    public float bottomBounds;

    public BulletType Type { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Type == BulletType.Alien) {
            transform.position -= Vector3.forward * speed * Time.deltaTime; 
        } else {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }

        if (transform.position.z < bottomBounds || transform.position.z > topBounds) {
            Destroy(this.gameObject);
        }
    }
}
