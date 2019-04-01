using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinSound : MonoBehaviour
{
    public AudioSource pinHitSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (pinHitSound != null)
        {
            pinHitSound.Play();
        }
    }
}
