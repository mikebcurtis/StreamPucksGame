using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstDayOnTheJobManager : MonoBehaviour 
{
    public string titleString = "Mission: Target Practice";
    public string objectiveString = "Light up all switches. Remaining: {0}/{1}";
    public string completedString = "Complete!";
    public int numSwitches = 13;
    public string missionName = "First Day on the Job";

    private int remainingHits;

    // Use this for initialization
    void Start()
    {
        remainingHits = numSwitches;

        if (EventList.AddRotatingText != null)
        {
            EventList.AddRotatingText(titleString, string.Format(objectiveString, remainingHits, numSwitches));
        }

        if (EventList.MissionStarted != null)
        {
            EventList.MissionStarted(missionName);
        }
        //if (titleText != null)
        //{
        //    titleText.text = titleString;
        //}

        //if (objectiveText != null)
        //{
        //    objectiveText.text = string.Format(objectiveString, remainingHits, numSwitches);
        //}

        EventList.SwitchPressed += SwitchHitHandler;
    }

    private void SwitchHitHandler(GameObject switchObj, GameObject puck)
    {
        if (remainingHits > 0)
        {
            SwitchScript switchScript = switchObj.GetComponent<SwitchScript>();
            remainingHits = switchScript.state ? remainingHits + 1 : remainingHits - 1;
            //objectiveText.text = string.Format(objectiveString, remainingHits, numSwitches);
            if (remainingHits <= 0)
            {
                //objectiveText.text = string.Format(completedString, numSwitches);
                if (EventList.AddRotatingText != null)
                {
                    EventList.AddRotatingText(titleString, string.Format(completedString, numSwitches));
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
                EventList.AddRotatingText(titleString, string.Format(objectiveString, remainingHits, numSwitches));
            }
        }
    }
}
