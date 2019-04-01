using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransporterScript : MonoBehaviour
{
    public GameObject teleportReceiver;

    private void OnCollisionEnter(Collision collision)
    {
        if (teleportReceiver != null)
        {
            collision.collider.transform.position = teleportReceiver.transform.position;
        }
    }
}
