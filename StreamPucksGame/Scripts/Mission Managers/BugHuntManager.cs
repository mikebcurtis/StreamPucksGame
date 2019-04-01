using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BugHuntManager : MonoBehaviour
{
    public string titleString = "Mission: Bug Hunt";
    public string objectiveString = "Get {0} bug icons. Remaining: {1}";
    public string completedString = "Complete!";
    public int bugsGoal = 30;
    public string missionName = "Bug Hunt";

    private int remainingBugs;

    // Use this for initialization
    void Start()
    {
        remainingBugs = bugsGoal;
        //if (titleText != null)
        //{
        //    titleText.text = titleString;
        //}
        if (EventList.AddRotatingText != null)
        {
            EventList.AddRotatingText(titleString, string.Format(objectiveString, remainingBugs, bugsGoal));
        }

        if (EventList.MissionStarted != null)
        {
            EventList.MissionStarted(missionName);
        }

        //if (objectiveText != null)
        //{
        //    objectiveText.text = string.Format(objectiveString, remainingBugs, bugsGoal);
        //}

        EventList.SlotBoxRelease += SlotBoxReleaseHandler;
    }

    private void SlotBoxReleaseHandler(GameObject slotBoxObj, GameObject puckObj)
    {
        SlotBoxScript slotBox = slotBoxObj.GetComponentInChildren<SlotBoxScript>();

        if (slotBox != null)
        {
            PrizeComponent prize = slotBox.currentPrize;

            if (prize != null && prize.otherType == Constants.BUG_PRIZE_TYPE && remainingBugs > 0)
            {

                //objectiveText.text = string.Format(objectiveString, --remainingBugs, bugsGoal);

                if (--remainingBugs <= 0)
                {
                    //objectiveText.text = completedString;
                    if (EventList.AddRotatingText != null)
                    {
                        EventList.AddRotatingText(titleString, completedString);
                    }
                    string missionId = Constants.GetMissionId(missionName);
                    if (missionId == null)
                    {
                        Debug.LogError("Could not retrieve mission Id for mission name: " + missionName);
                        return;
                    }

                    if (EventList.MissionCompleted != null)
                    {
                        EventList.MissionCompleted(missionId);
                    }
                }
                else if (EventList.AddRotatingText != null)
                {
                    EventList.AddRotatingText(titleString, string.Format(objectiveString, remainingBugs, bugsGoal));
                }
            }
        }
    }
}
