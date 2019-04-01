using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BumperSound : MonoBehaviour {

    public static UnityAction BumperHit;

    private void OnCollisionEnter(Collision collision)
    {
        BumperHit();
    }
}
