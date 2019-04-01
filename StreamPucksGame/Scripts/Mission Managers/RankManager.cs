using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankManager : MonoBehaviour
{
    public static GameObject instance;

    public string[] CadetMissions;
    public string[] EnsignMissions;
    public string[] LieutenantMissions;
    public string[] CommanderMissions;
    public string[] CaptainMissions;
    public string[] AdmiralMissions;

    public bool DEBUG_UNLOCK_ALL = false;

    public int CurrentRank
    {
        get
        {
            return currentRank;
        }

        private set
        {
            if (currentRank != value && EventList.RankChange != null)
            {
                EventList.RankChange(value);
            }

            currentRank = value;
        }
    }

    private List<string> CompletedMissions;
    private int currentRank = 0;

    // Use this for initialization
    void Start ()
    {
        if (instance == null)
        {
            instance = this.gameObject;
        }
        else
        {
            Debug.LogError("There were two RankManager objects and there can only be one. Deleting the second one.");
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(instance);

        RankInfo rankInfo = GameData.instance.GetComponent<GameData>().rankData;
        currentRank = rankInfo.currentRank;
        CompletedMissions = new List<string>(rankInfo.CompletedMissions);

        EventList.MissionCompleted += MissionCompletedHandler;

        VerifyRank();
    }

    private void OnDestroy()
    {
        EventList.MissionCompleted -= MissionCompletedHandler;
    }

    private void MissionCompletedHandler(string missionId)
    {
        if (CompletedMissions.Contains(missionId) == false)
        {
            CompletedMissions.Add(missionId);
            VerifyRank();
        }
    }

    private void VerifyRank()
    {
        if (CheckCompletedMissions(CadetMissions) == false)
        {
            CurrentRank = 0;
            return;
        }

        if (CheckCompletedMissions(EnsignMissions) == false)
        {
            CurrentRank = 1;
            return;
        }

        if (CheckCompletedMissions(LieutenantMissions) == false)
        {
            CurrentRank = 2;
            return;
        }

        if (CheckCompletedMissions(CommanderMissions) == false)
        {
            CurrentRank = 3;
            return;
        }

        if (CheckCompletedMissions(CaptainMissions) == false)
        {
            CurrentRank = 4;
            return;
        }

        if (CheckCompletedMissions(AdmiralMissions) == false)
        {
            CurrentRank = 5;
            return;
        }

        CurrentRank = 5;
        return;
    }

    private bool CheckCompletedMissions(string[] missions)
    {
        if (DEBUG_UNLOCK_ALL)
        {
            return true;
        }

        if (missions.Length <= 0)
        {
            return false;
        }

        foreach(string missionId in missions)
        {
            if (CompletedMissions.Contains(missionId) == false)
            {
                return false;
            }
        }

        return true;
    }
}
