using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectBlock : MonoBehaviour
{
    public Vector3 normalVector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Constants.BLOCK_TAG)
        {
            RandomBlockMovement blockScript = other.gameObject.GetComponent<RandomBlockMovement>();
            if (blockScript == null)
                return;

            blockScript.movementVector = Vector3.Reflect(blockScript.movementVector, normalVector);
        }
    }
}
