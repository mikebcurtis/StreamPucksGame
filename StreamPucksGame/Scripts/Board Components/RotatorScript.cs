using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorScript : MonoBehaviour
{
    public float RotateSpeed = 1f;
    public Vector3 axis;

    private void Start()
    {
        if (axis == null || (axis.x == 0 && axis.y == 0 && axis.z == 0))
        {
            axis = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
        //this.transform.Rotate(axis, Time.deltaTime * RotateSpeed);
	}
}
