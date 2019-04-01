using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionUIManager : MonoBehaviour
{
    public string CompletedText = "Status: Complete";
    public Color completedColor = Color.green;
    public string IncompleteText = "Status: Incomplete";
    public Color incompleteColor = Color.red;
    public Text StatusText;
    public int Rank = 0;
    public GameObject lockObject;
    public Button playButton;
    public string missionName = "Basic Training";
    public bool DEBUG_UNLOCK = false;

    private string missionId;
    private string missionSceneName;
	// Use this for initialization
	void Start ()
    {
        missionId = Constants.GetMissionId(missionName);
        missionSceneName = Constants.GetMissionSceneName(missionName);

        if (missionId == null)
        {
            Debug.LogError("Could not load missionId for mission name: " + missionName);
        }

        if (missionSceneName == null)
        {
            Debug.LogError("Could not load scene name for mission name: " + missionName);
        }

        if (GameData.instance != null)
        {
            GameData gameData = GameData.instance.GetComponent<GameData>();
            if (gameData == null)
            {
                Debug.LogError("Could not load GameData component from the GameData GameObject.");
                return;
            }

            List<int> unlockedRanks = new List<int>(gameData.RankData.UnlockedRanks);
            List<string> completedMissions = new List<string>(gameData.RankData.CompletedMissions);

            if (playButton != null && lockObject != null)
            {
                if (Rank == 0 || unlockedRanks.Contains(Rank) || DEBUG_UNLOCK)
                {
                    // mission should be unlocked for play
                    Unlock();
                }
                else
                {
                    // player hasn't unlocked this rank yet
                    Lock();
                }
            }
            else
            {
                Debug.LogError("MissionUIManager cannot find play button or lock image object.");
            }

            if (completedMissions.Contains(missionId))
            {
                MarkCompleted();
            }
            else
            {
                MarkIncomplete();
            }
        }
        else
        {
            Debug.LogError("Could not initialize mission text - cannot find GameData object.");
        }

        EventList.MissionCompleted += MissionCompletedHandler;
        EventList.RankChange += RankPromotionHandler;
	}

    private void OnDestroy()
    {
        EventList.MissionCompleted -= MissionCompletedHandler;
        EventList.RankChange -= RankPromotionHandler;
    }

    private void RankPromotionHandler(int newRank)
    {
        if (newRank >= Rank)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    private void MissionCompletedHandler(string missionId)
    {
        if (missionId == Constants.GetMissionId(missionName))
        {
            MarkCompleted();
        }
    }

    public void PlayButtonClicked()
    {
        SceneManager.LoadScene(missionSceneName);
    }

    private void MarkIncomplete()
    {
        StatusText.text = IncompleteText;
        StatusText.color = incompleteColor;
    }

    private void MarkCompleted()
    {
        StatusText.text = CompletedText;
        StatusText.color = completedColor;
    }

    private void Unlock()
    {
        playButton.gameObject.SetActive(true);
        lockObject.gameObject.SetActive(false);
    }

    private void Lock()
    {
        playButton.gameObject.SetActive(false);
        lockObject.gameObject.SetActive(true);
    }
}
