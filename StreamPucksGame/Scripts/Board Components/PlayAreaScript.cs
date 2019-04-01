using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == Constants.PUCK_TAG && EventList.PuckOutOfBounds != null)
        {
            EventList.PuckOutOfBounds(collision.gameObject);
        }

        Destroy(collision.gameObject);
    }
}
