using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        EventList.BumperHit += BumperHitHandler;
        EventList.CupCatch += CupCatchHandler;
        EventList.SlotBoxCapture += SlotBoxCaptureHandler;
        EventList.SlotBoxRelease += SlotBoxReleaseHandler;
        EventList.SwitchPressed += SwitchPressedHandler;
	}

    private void OnDestroy()
    {
        EventList.BumperHit -= BumperHitHandler;
        EventList.CupCatch -= CupCatchHandler;
        EventList.SlotBoxCapture -= SlotBoxCaptureHandler;
        EventList.SlotBoxRelease -= SlotBoxReleaseHandler;
        EventList.SwitchPressed -= SwitchPressedHandler;
    }

    private void SwitchPressedHandler(GameObject arg1, GameObject arg2)
    {
        Debug.Log("Switch pressed");
    }

    private void SlotBoxReleaseHandler(GameObject arg1, GameObject arg2)
    {
        Debug.Log("Slot Box release");
    }

    private void SlotBoxCaptureHandler(GameObject arg1, GameObject arg2)
    {
        Debug.Log("Slot Box capture");
    }

    private void CupCatchHandler(GameObject arg1, GameObject arg2)
    {
        Debug.Log("Cup catch");
    }

    private void BumperHitHandler(GameObject arg1, GameObject arg2)
    {
        Debug.Log("Bumper hit");
    }
}
