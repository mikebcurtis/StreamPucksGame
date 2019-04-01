using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiggleScript : MonoBehaviour
{
    public float Tolerance = 0.01f;
    public float CheckFrequencySeconds = 5f;
    public float MaxForce = 1f;
    private Vector3 prevLocation;

	// Use this for initialization
	void Start ()
    {
        prevLocation = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        StartCoroutine("DoCheck");
	}

    private IEnumerator DoCheck()
    {
        while (true)
        {
            if (HasNotMoved())
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb)
                {
                    //rb.AddForce(new Vector3(Random.Range(0, MaxForce), Random.Range(0, MaxForce), 0));
                    Debug.Log("has not moved");
                }
                else
                {
                    yield return null;
                }
            }

            prevLocation = transform.position;

            Debug.Log("checking");
            yield return new WaitForSeconds(CheckFrequencySeconds);
        }
    }

    private bool HasNotMoved()
    {
        return Mathf.Abs(prevLocation.x - transform.position.x) < Tolerance &&
            Mathf.Abs(prevLocation.y - transform.position.y) < Tolerance &&
            Mathf.Abs(prevLocation.z - transform.position.z) < Tolerance;
    }
}
