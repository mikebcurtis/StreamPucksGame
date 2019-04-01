using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchTubeTriggerScript : MonoBehaviour
{
    public static string PUCK_TAG = Constants.PUCK_TAG;
    public GameObject Launcher;
    public Vector3 startForwardVector;
    public float angleTolerance = 0.8f;
    public float LauncherTurnSpeed = 80f;
    public int Direction = 1;
    public Vector3 rotatePointOffsetFromCenter;
    public float ForceModifier = 50f;
    private Vector3 rotatePoint;


    private IEnumerator rotateAndFireCoroutine;

    private void Start()
    {
        //startForwardVector = Launcher.transform.up;
        //startForwardVector = Vector3.forward;
        startForwardVector = new Vector3(1f * (Direction * -1f), 0f, 0f);
        rotatePoint = this.transform.position + rotatePointOffsetFromCenter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == PUCK_TAG)
        {
            LaunchData puckData = other.gameObject.GetComponent<LaunchData>();
            if (puckData == null)
            {
                Debug.LogError("Launcher was given puck with no launch data.");
                Destroy(other.gameObject);
                return;
            }

            // freeze the puck so it appears like it was caught in some mechanism
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            rotateAndFireCoroutine = RotateAndFire(other.gameObject, puckData);
            StartCoroutine(rotateAndFireCoroutine);
        }
    }

    private IEnumerator RotateAndFire(GameObject puck, LaunchData puckData)
    {
        float angle = Vector3.Angle(startForwardVector, Launcher.transform.forward);
        while (angle < (puckData.Angle - angleTolerance) || angle > (puckData.Angle + angleTolerance))
        {
            //float rotateAngle = LauncherTurnSpeed * Time.deltaTime * Mathf.Sign(angle - puckData.Angle);
            float rotateAngle = LauncherTurnSpeed * Time.deltaTime * Mathf.Sign(angle - puckData.Angle) * Mathf.Sign(Direction);
            Launcher.transform.RotateAround(rotatePoint, Vector3.forward, rotateAngle);
            yield return null;
            angle = Vector3.Angle(startForwardVector, Launcher.transform.forward);
        }

        Fire(puck, puckData.Power);
    }

    private void Fire(GameObject puck, float force)
    {
        Vector3 forceVector = Launcher.transform.forward * force * ForceModifier;
        forceVector.z = 0;
        GameObject newPuck = Instantiate<GameObject>(puck, Launcher.transform.position, puck.transform.rotation);
        //puck.transform.Translate(Launcher.transform.forward * 5f);
        Rigidbody rb = newPuck.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        newPuck.GetComponent<Rigidbody>().AddForce(forceVector);
        Destroy(puck);
    }
}
