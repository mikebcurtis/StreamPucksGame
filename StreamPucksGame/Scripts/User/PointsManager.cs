using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    private static GameObject instance;

    public int DefaultBumperHitPoints = 1;
    public int DefaultCupCatchPoints = 5;

    // Use this for initialization
    void Start()
    {
        // enforce singleton
        if (instance != null)
        {
            Debug.LogError("A second points manager has been deleted. There can only be one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;

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

    private void SwitchPressedHandler(GameObject switchObj, GameObject puck)
    {
        //Debug.Log("Switch pressed");
    }

    private void SlotBoxReleaseHandler(GameObject slotBox, GameObject puck)
    {
        //Debug.Log("Slot Box release");
    }

    private void SlotBoxCaptureHandler(GameObject slotBox, GameObject puck)
    {
        //Debug.Log("Slot Box capture");
    }

    private void CupCatchHandler(GameObject cup, GameObject puck)
    {
        
    }

    private void BumperHitHandler(GameObject bumper, GameObject puck)
    {
        //PrizeComponent prizeComponent = arg1.GetComponent<PrizeComponent>();
        //if (bumperScript != null)
        //{


        //}
    }

    private void ScorePoints(User user, int points)
    {
        
    }
}
