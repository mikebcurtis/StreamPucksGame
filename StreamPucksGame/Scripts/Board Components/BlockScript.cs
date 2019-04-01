using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    //public GameObject burstEffectObj;
    private ParticleSystem ps;

	// Use this for initialization
	void Start ()
    {
        ps = GetComponentInParent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == Constants.PUCK_TAG)
        {
            if (ps != null)
            {
                ps.Play();
            }
            
            if (EventList.BlockHit != null)
            {
                EventList.BlockHit(this.gameObject, collision.collider.gameObject);
            }

            Destroy(this.transform.parent.gameObject, ps.main.duration); // after the particle effect is done, delete this object
            this.gameObject.SetActive(false); // in the meantime, disable this block obj so no collisions are registered
        }
    }
}
