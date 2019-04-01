using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BumperMissionManager : MonoBehaviour
{
   // public Text titleText;
    //public Text objectiveText;
    public Text leaderBoardText;
    //public TitleDisplayManagerScript display;
    public string titleString = "Mission: Basic Training";
    public string objectiveString = "Hit bumpers {1} times. Remaining: {0}";
    public string completedString = "Hit bumpers {0} times. Complete!";
    public int goalHits = 100;
    public string missionName = "Basic Training";

    private Dictionary<string, int> hitsPerPlayer;
    private int remainingHits;

	// Use this for initialization
	void Start ()
    {
        remainingHits = goalHits;
  //      if (titleText != null)
  //      {
  //          titleText.text = titleString;
  //      }

		//if (objectiveText != null)
  //      {
  //          objectiveText.text = string.Format(objectiveString, remainingHits, goalHits);
  //      }

        //display.Add(titleString, string.Format(objectiveString, remainingHits, goalHits));

        if (EventList.AddRotatingText != null)
        {
            EventList.AddRotatingText(titleString, string.Format(objectiveString, remainingHits, goalHits));
        }

        if (EventList.MissionStarted != null)
        {
            EventList.MissionStarted(missionName);
        }

        hitsPerPlayer = new Dictionary<string, int>();
        UpdateLeaderboard();

        EventList.BumperHit += BumperHitHandler;
	}

    private void BumperHitHandler(GameObject bumper, GameObject puck)
    {
        if (remainingHits > 0)
        {
            //objectiveText.text = string.Format(objectiveString, --remainingHits, goalHits);
            //display.Add(titleString, string.Format(objectiveString, --remainingHits, goalHits), 5, 0);
            if (EventList.AddRotatingText != null)
            {
                EventList.AddRotatingText(titleString, string.Format(objectiveString, --remainingHits, goalHits));
            }
            if (remainingHits <= 0)
            {
                //objectiveText.text = string.Format(completedString, goalHits);
                //display.Add(titleString, string.Format(objectiveString, --remainingHits, goalHits), 5, 0);
                if (EventList.AddRotatingText != null)
                {
                    EventList.AddRotatingText(titleString, string.Format(completedString, goalHits));
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
        }

        LaunchData launchData = puck.GetComponent<LaunchData>();
        if (launchData != null)
        {
            if (hitsPerPlayer.ContainsKey(launchData.Player.DisplayName))
            {
                int currentHitAmount = hitsPerPlayer[launchData.Player.DisplayName];
                hitsPerPlayer[launchData.Player.DisplayName] = currentHitAmount + 1;
            }
            else
            {
                hitsPerPlayer.Add(launchData.Player.DisplayName, 1);
            }
        }

        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
       if (leaderBoardText != null)
       {
            string leaderBoardString = "Bumper Hits Leaderboard";
            List<KeyValuePair<string, int>> hitsSorted = hitsPerPlayer.ToList();
            hitsSorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            for (int i = 0; i < 5; i++)
            {
                if (hitsSorted.Count >= i + 1)
                {
                    leaderBoardString += string.Format("\n{0}- {1} {2}", i + 1, hitsSorted[i].Key, hitsSorted[i].Value);
                }
                else
                {
                    leaderBoardString += "\n" + (i + 1) + "-";
                }
            }

            leaderBoardText.text = leaderBoardString;
       }
    }
}
