using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public float rotateIntervalZ = 0;
    public float rotateIntervalY = 0;
    public float rotateIntervalX = 0;
    public bool spinWhenHit = false;
    public float spinDuration = 1.5f;
    public int spinMultiplier = 5;

    void Update ()
    {
        transform.Rotate (new Vector3(rotateIntervalX, rotateIntervalY, rotateIntervalZ)*Time.deltaTime);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (spinWhenHit && collision.gameObject.tag == Constants.PUCK_TAG)
        {
            StartCoroutine(Spin());
        }
    }

    private IEnumerator Spin()
    {
        float elapsedTime = 0;
        float originalX = rotateIntervalX;
        float originalY = rotateIntervalY;
        float originalZ = rotateIntervalZ;
        float tolerance = 0.1f;
        rotateIntervalX *= spinMultiplier;
        rotateIntervalY *= spinMultiplier;
        rotateIntervalZ *= spinMultiplier;
        while ((rotateIntervalX != 0 && rotateIntervalX > (originalX + tolerance)) ||
               (rotateIntervalY != 0 && rotateIntervalY > (originalY + tolerance)) ||
               (rotateIntervalZ != 0 && rotateIntervalZ > (originalZ + tolerance)))
        {
            float t = elapsedTime / spinDuration;
            rotateIntervalX = Mathf.Lerp(rotateIntervalX, originalX, t);
            rotateIntervalY = Mathf.Lerp(rotateIntervalY, originalY, t);
            rotateIntervalZ = Mathf.Lerp(rotateIntervalZ, originalZ, t);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }
}
