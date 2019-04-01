using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakoutManager : MonoBehaviour
{
    public string titleString = "Mission: Breakout";
    public string objectiveString = "Break {0} blocks. Remaining: {1}";
    public string completedString = "Complete!";
    public GameObject blocksParent; // define this to auto-discover the blocks that need to be hit
    public int optionalBlockCount; // define this to set a specific number of blocks that need to be hit
    public string missionName = "Breakout";

    private int blockGoal;
    private int remainingBlocks;

    // Use this for initialization
    void Start()
    {
        if (optionalBlockCount > 0)
        {
            blockGoal = optionalBlockCount;
        }
        else
        {
            blockGoal = blocksParent.GetComponentsInChildren<BlockScript>().Length;
        }
        remainingBlocks = blockGoal;

        if (EventList.AddRotatingText != null)
        {
            EventList.AddRotatingText(titleString, string.Format(objectiveString, remainingBlocks, blockGoal));
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
        //    objectiveText.text = string.Format(objectiveString, remainingBlocks, blockGoal);
        //}

        EventList.BlockHit += BlockHitHandler;
    }

    private void BlockHitHandler(GameObject block, GameObject puck)
    {
        //objectiveText.text = string.Format(objectiveString, blockGoal, --remainingBlocks);
        if (remainingBlocks <= 0)
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
            EventList.AddRotatingText(titleString, string.Format(objectiveString, blockGoal, --remainingBlocks));
        }
    }
}

