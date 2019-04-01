using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementChangeScript : MonoBehaviour
{
    public Vector3 ChangeVector;

    private void OnTriggerEnter(Collider other)
    {
        DropperScript dropper = other.GetComponent<DropperScript>();
        if (dropper != null)
        {
            dropper.velocityVector = new Vector3(dropper.velocityVector.x * ChangeVector.x,
                                                 dropper.velocityVector.y * ChangeVector.y,
                                                 dropper.velocityVector.z * ChangeVector.z);
        }
    }
}
