using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSegmentFollowerScript : MonoBehaviour
{
    public GameObject followObj;
    private float minDistance = 5f;
    private float distanceDamp = 5f;
    private Vector3 prevPos;
	
	// Update is called once per frame
	void Update ()
    {
        float distance = Vector3.Distance(this.transform.position, followObj.transform.position);

        //float step = followSpeed * Time.deltaTime * distance;
        this.transform.position = Vector3.Lerp(this.transform.position, prevPos, Time.deltaTime * distanceDamp); // Vector3.MoveTowards(this.transform.position, followObj.transform.position, step);
        prevPos = followObj.transform.position;
	}
}
