using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    Dictionary<AudioSource, float> lastPlayedDictionary;

    public float minTimeThreshold = 0.01f;
    public float sameSoundMinTimeDelay = 0.1f;

    public AudioSource BumperHit;
    public float bumperMinPitch = 1f;
    public float bumperMaxPitch = 1.25f;

    public AudioSource CupHit;
    public float cupMinPitch = 1f;
    public float cupMaxPitch = 1.25f;

    public AudioSource LaunchSound;
    public float launchMinPitch = 1f;
    public float launchMaxPitch = 1.25f;

    public AudioSource SwitchSound;
    public float switchMinPitch = 1f;
    public float switchMaxPitch = 1.1f;

    public AudioSource LauncherHitSound;
    public float launcherHitMinPitch = 0.9f;
    public float launcherHitMaxPitch = 1f;

    public AudioSource PuckOutOfBoundsSound;
    public float puckOutMinPitch = 0.9f;
    public float puckOutMaxPitch = 1.25f;

    public AudioSource BlockHitSound;
    public float blockHitMinPitch = 0.9f;
    public float blockHitMaxPitch = 1.25f;

    private void Start()
    {
        InitializeSound();

        lastPlayedDictionary = new Dictionary<AudioSource, float>();

        EventList.BumperHit += BumperHitHandler;
        EventList.CupCatch += CupCatchHandler;
        EventList.NowLaunching += LaunchHandler;
        EventList.SwitchPressed += SwitchPressedHandler;
        EventList.LauncherHit += LauncherHitHandler;
        EventList.PuckOutOfBounds += PuckOutHandler;
        EventList.BlockHit += BlockHitHandler;
    }

    private void OnDestroy()
    {
        EventList.BumperHit -= BumperHitHandler;
        EventList.CupCatch -= CupCatchHandler;
        EventList.NowLaunching -= LaunchHandler;
        EventList.SwitchPressed -= SwitchPressedHandler;
        EventList.LauncherHit -= LauncherHitHandler;
        EventList.PuckOutOfBounds -= PuckOutHandler;
        EventList.BlockHit -= BlockHitHandler;
    }

    private void InitializeSound()
    {
        AdjustVolume(BumperHit);
        AdjustVolume(CupHit);
        AdjustVolume(LaunchSound);
        AdjustVolume(SwitchSound);
        AdjustVolume(LauncherHitSound);
        AdjustVolume(PuckOutOfBoundsSound);
        AdjustVolume(BlockHitSound);
    }

    private void AdjustVolume(AudioSource audio)
    {
        float sfxVolume = PlayerPrefs.GetFloat(Constants.SFX_VOLUME_KEY, 0.5f);
        audio.volume = Mathf.Lerp(0f, sfxVolume, audio.volume);
    }

    private void BlockHitHandler(GameObject arg1, GameObject arg2)
    {
        StartCoroutine(PlaySound(BlockHitSound, blockHitMinPitch, blockHitMaxPitch));
    }

    private void PuckOutHandler(GameObject obj)
    {
        StartCoroutine(PlaySound(PuckOutOfBoundsSound, puckOutMinPitch, puckOutMaxPitch));
    }

    private void LauncherHitHandler(GameObject arg1, GameObject arg2)
    {
        StartCoroutine(PlaySound(LauncherHitSound, launcherHitMinPitch, launcherHitMaxPitch));
    }

    private void SwitchPressedHandler(GameObject arg1, GameObject arg2)
    {
        StartCoroutine(PlaySound(SwitchSound, switchMinPitch, switchMaxPitch));
    }

    private void LaunchHandler(int arg1, Launch arg2, int arg3)
    {
        StartCoroutine(PlaySound(LaunchSound, launchMinPitch, launchMaxPitch));
    }

    private void CupCatchHandler(GameObject arg1, GameObject arg2)
    {
        StartCoroutine(PlaySound(CupHit, cupMinPitch, cupMaxPitch));
    }

    private void BumperHitHandler(GameObject arg1, GameObject arg2)
    {
        StartCoroutine(PlaySound(BumperHit, bumperMinPitch, bumperMaxPitch));
    }

    private IEnumerator PlaySound(AudioSource audio, float minVariance, float maxVariance, float timeToWait = 0f)
    {
        if (audio != null)
        {
            float time = Time.fixedTime;
            if (lastPlayedDictionary.ContainsKey(audio))
            {
                float timeDiff = time - lastPlayedDictionary[audio];
                if (timeDiff < minTimeThreshold)
                {
                    yield return new WaitForSeconds(sameSoundMinTimeDelay - timeDiff);
                }
            }
            else
            {
                lastPlayedDictionary.Add(audio, time);
            }

            audio.Play();
        }

        yield return null;
    }
}
