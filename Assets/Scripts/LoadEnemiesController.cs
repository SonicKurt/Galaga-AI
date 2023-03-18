using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class LoadEnemiesController : MonoBehaviour
{
    private SplineAnimate animate;
    private AlienController alienController;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        animate = GetComponent<SplineAnimate>();
        alienController = GetComponent<AlienController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!animate.isPlaying) {

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, alienController.SpawnPos, step);
        }

    }
}
