using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperBasic : MonoBehaviour
{
    public float Strength = 100f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        if (rb != null && collision.gameObject.tag == Constants.PUCK_TAG)
        {
            Vector3 forceVector = collision.collider.transform.position - transform.position;
            rb.AddForce(forceVector * Strength);
            if (EventList.BumperHit != null)
            {
                EventList.BumperHit(this.gameObject, rb.gameObject);
            }
        }
    }
}
