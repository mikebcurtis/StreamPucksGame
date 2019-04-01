using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlockMovement : MonoBehaviour
{
    public Vector3 movementVector;
    public float minSpeed;
    public float maxSpeed;
    public float speed;

    // Use this for initialization
    void Start()
    {
        movementVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        speed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(movementVector * speed * Time.deltaTime);
    }
}
