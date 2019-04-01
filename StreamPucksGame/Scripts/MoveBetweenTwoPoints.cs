using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenTwoPoints : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public float moveTime = 4f;

    public float tolerance = 0.1f;

    private GameObject targetObject;
    private AnimationCurve curveX;
    private AnimationCurve curveY;
    private AnimationCurve curveZ;
    
	void Start ()
    {
        targetObject = pointB;
        float percentBetweenObjects = Vector3.Distance(gameObject.transform.position, targetObject.transform.position) /
                                      Vector3.Distance(pointA.transform.position, pointB.transform.position);
        SetCurves(gameObject, targetObject, percentBetweenObjects * moveTime);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (curveX == null || curveY == null || curveZ == null)
        {
            return;
        }

        float xPos = curveX.Evaluate(Time.fixedTime);
        float yPos = curveY.Evaluate(Time.fixedTime);
        float zPos = curveZ.Evaluate(Time.fixedTime);
        gameObject.transform.position = new Vector3(xPos, yPos, zPos);

        // check if we've arrived at our target, if so, switch targets
        if (isWithinTolerance(xPos, targetObject.transform.position.x, tolerance) &&
            isWithinTolerance(yPos, targetObject.transform.position.y, tolerance) &&
            isWithinTolerance(zPos, targetObject.transform.position.z, tolerance))
        {
            targetObject = (targetObject == pointA) ? pointB : pointA;
            SetCurves(gameObject, targetObject);
        }
    }

    private void SetCurves(GameObject from, GameObject to, float time = 0.0f)
    {
        if (time == 0.0f)
        {
            time = moveTime;
        }

        curveX = AnimationCurve.EaseInOut(Time.fixedTime, from.transform.position.x, Time.fixedTime + time, to.transform.position.x);
        curveY = AnimationCurve.EaseInOut(Time.fixedTime, from.transform.position.y, Time.fixedTime + time, to.transform.position.y);
        curveZ = AnimationCurve.EaseInOut(Time.fixedTime, from.transform.position.z, Time.fixedTime + time, to.transform.position.z);
    }

    private bool isWithinTolerance(float value, float target, float tolerance)
    {
        return (value <= target + tolerance) && (value >= target - tolerance);
    }
}
