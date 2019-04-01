using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperScript : MonoBehaviour
{
    public static string DROPPER_TAG = "Dropper";

    public GameObject ballPrefab;
    public int numBallsToLaunch = -1;
    public float launchFrequency = 0.5f;
    public Vector3 velocityVector;

    private void Start()
    {
        StartCoroutine("Launch");
    }

    private void Update()
    {
        transform.Translate(new Vector3(velocityVector.x * Time.deltaTime,
                                        velocityVector.y * Time.deltaTime,
                                        velocityVector.z * Time.deltaTime));
    }

    private IEnumerator Launch()
    {
        //Debug.Log("entering Launch");
        while (true)
        {
            yield return new WaitForSeconds(launchFrequency);

            if (numBallsToLaunch > 0)
            {
                numBallsToLaunch--;
            }
            else if (numBallsToLaunch == 0)
            {
                yield return null;
            }

            //Debug.Log("creating new ball");
            GameObject ball = Instantiate<GameObject>(ballPrefab);
            ball.transform.position = transform.position;
            //float force = Random.Range(minLaunchForce, maxLaunchForce);
            //Vector3 forceVector = transform.up * force;
            //forceVector.z = 0;
            //ball.GetComponent<Rigidbody>().AddForce(forceVector);
        }
    }
}
