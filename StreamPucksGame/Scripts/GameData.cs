using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class RankInfo
{
    public int currentRank;
    public int[] UnlockedRanks;
    public string[] CompletedMissions;

    public RankInfo()
    {
        currentRank = 0;
        UnlockedRanks = new int[0];
        CompletedMissions = new string[0];
    }
}

public class GameData : MonoBehaviour
{
    public static GameObject instance;

    public RankInfo RankData
    {
        get
        {
            return rankData;
        }
        set
        {
            rankData = value;
            SaveRankData();
        }
    }

    public string userInfoFileName = "userInfo.dat";
    private TwitchLoginUserInfo userData;

    public string rankInfoFileName = "rankInfo.dat";
    public RankInfo rankData;

	// Use this for initialization
	void Awake()
    {
        if (instance == null)
        {
            instance = this.gameObject;
        }
        else
        {
            Debug.LogError("There were two GameData objects and there can only be one. Deleting the second one.");
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(instance);

        if (LoadRankData() == false)
        {
            rankData = new RankInfo();
            SaveRankData();
        }

        EventList.TwitchUserLogOut += OnTwitchUserLogout;
        EventList.MissionCompleted += MissionCompletedHandler;
        EventList.RankChange += RankPromotionHandler;
	}

    private void Start()
    {
        //if (LoadUserData() && EventList.TwitchUserLogin != null)
        //{
        //    EventList.TwitchUserLogin(userData);
        //}
        LoadUserData();
        EventList.TwitchUserLogin += OnTwitchUserLogin;
    }

    private void OnDestroy()
    {
        EventList.TwitchUserLogOut -= OnTwitchUserLogout;
        EventList.TwitchUserLogin -= OnTwitchUserLogin;
        EventList.MissionCompleted -= MissionCompletedHandler;
        EventList.RankChange -= RankPromotionHandler;
    }

    #region UserInfo

    private void OnTwitchUserLogout()
    {
        DeleteUserFile();
    }

    private void OnTwitchUserLogin(TwitchLoginUserInfo userInfo)
    {
        userData = userInfo;
        SaveUserData();
    }

    private bool LoadUserData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + userInfoFileName))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + userInfoFileName, FileMode.Open);
                TwitchLoginUserInfo data = (TwitchLoginUserInfo)bf.Deserialize(file);
                userData = data;
                file.Close();
            }
            catch
            {
                return false;
            }

            if (EventList.TwitchUserLoadedFromFile != null)
            {
                EventList.TwitchUserLoadedFromFile(userData);
            }

            return true;
        }

        return false;
    }

    private void SaveUserData()
    {
        if (userData != null)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + userInfoFileName);

            bf.Serialize(file, userData);
            file.Close();
        }
    }

    private void DeleteUserFile()
    {
        if (File.Exists(Application.persistentDataPath + "/" + userInfoFileName))
        {
            File.Delete(Application.persistentDataPath + "/" + userInfoFileName);
        }
    }

    #endregion

    #region RankInfo
    
    private void RankPromotionHandler(int rankId)
    {
        List<int> ranksList = new List<int>(rankData.UnlockedRanks);
        if (ranksList.Contains(rankId) == false)
        {
            ranksList.Add(rankId);
            rankData.UnlockedRanks = ranksList.ToArray();
            SaveRankData();
        }

        if (rankId > rankData.currentRank)
        {
            rankData.currentRank = rankId;
            SaveRankData();
        }
    }

    private void MissionCompletedHandler(string missionId)
    {
        List<string> missionsList = new List<string>(rankData.CompletedMissions);
        if (missionsList.Contains(missionId) == false)
        {
            missionsList.Add(missionId);
            rankData.CompletedMissions = missionsList.ToArray();
            SaveRankData();
        }
    }

    private bool LoadRankData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + rankInfoFileName))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + rankInfoFileName, FileMode.Open);
                RankInfo data = (RankInfo)bf.Deserialize(file);
                rankData = data;
                file.Close();
            }
            catch
            {
                return false;
            }

            //if (EventList.TwitchUserLoadedFromFile != null)
            //{
            //    EventList.TwitchUserLoadedFromFile(userData);
            //}

            return true;
        }

        return false;
    }

    private void SaveRankData()
    {
        if (rankData != null)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + rankInfoFileName);

            bf.Serialize(file, rankData);
            file.Close();
        }
    }

    private void DeleteRankFile()
    {
        if (File.Exists(Application.persistentDataPath + "/" + rankInfoFileName))
        {
            File.Delete(Application.persistentDataPath + "/" + rankInfoFileName);
        }
    }
    #endregion
}
