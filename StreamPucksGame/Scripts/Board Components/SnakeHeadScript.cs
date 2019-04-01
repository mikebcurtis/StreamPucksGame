using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHeadScript : MonoBehaviour
{
    public Vector3 movementVector;
    public float speed;
    public float minDist;
    public GameObject[] BodySegments;

	// Use this for initialization
	void Start ()
    {
        movementVector = new Vector3(Random.Range(0.1f, 1f), Random.Range(0.1f, 1f), 0);	
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.Translate(movementVector * speed * Time.deltaTime);
        GameObject prevObject = this.gameObject;
        for (int i = 0; i < BodySegments.Length; i++)
        {
            float distance = Vector3.Distance(BodySegments[i].transform.position, prevObject.transform.position);
            BodySegments[i].transform.position = Vector3.Lerp(BodySegments[i].transform.position, prevObject.transform.position, distance / minDist);
            prevObject = BodySegments[i];
        }
	}
}
