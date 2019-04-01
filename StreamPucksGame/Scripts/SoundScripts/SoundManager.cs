using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour {

    public AudioSource pinSoundClip;
    public AudioSource bumperSoundClip;
    // Use this for initialization
    void Start()
    {
        //PinSound.PinHit += PinHitHandler;
        BumperSound.BumperHit += BumperHitHandler;
    }

    private void BumperHitHandler()
    {
    //    bumperSoundClip.pitch = UnityEngine.Random.Range(-1, 2);
        bumperSoundClip.Play();
    }

    //Pins Sound
    private void PinHitHandler()
    {
        pinSoundClip.pitch = UnityEngine.Random.Range(-1, 2);
        pinSoundClip.Play();
    }

}
