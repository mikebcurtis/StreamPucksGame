using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupRotationScript : MonoBehaviour {
    public float RotateInterval = 1.5f;
    public float turnSpeed = 30f;
    private float timeSinceLastRotate = 0f;
    private float targetAngle = 180f;
    private Quaternion originalQ;
    private Quaternion flippedQ;
    private Coroutine rotateCoroutine;
    private bool flipped = true;

    private void Start()
    {
        originalQ = transform.rotation;
        flippedQ = Quaternion.Euler(new Vector3(
                        originalQ.eulerAngles.x,
                        originalQ.eulerAngles.y,
                        originalQ.eulerAngles.z + 180
                    ));
    }

    private void Update()
    {
        timeSinceLastRotate += Time.deltaTime;

        if (timeSinceLastRotate >= RotateInterval)
        {
            if (rotateCoroutine != null)
            {
                StopCoroutine(rotateCoroutine);
            }

            Quaternion targetRotation = flipped ? flippedQ : originalQ;
            rotateCoroutine = StartCoroutine(Rotate(targetRotation));
            flipped = !flipped;
            timeSinceLastRotate = 0f;
        }
    }

    private IEnumerator Rotate(Quaternion target)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        yield return null;
    }
}
