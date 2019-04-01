using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour
{
    public Vector3 axis;
    public float speed;
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.Rotate(axis, speed * Time.deltaTime);
	}
}
