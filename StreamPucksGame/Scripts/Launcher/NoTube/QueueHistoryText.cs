using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueHistoryText : MonoBehaviour
{
    public int LauncherId;
    public int MaxHistoryEntries = 4;
    private Queue<Launch> launches;
    private Text historyTextObj;

    // Use this for initialization
    void Start()
    {
        launches = new Queue<Launch>();
        historyTextObj = this.gameObject.GetComponent<Text>();
        EventList.LaunchFinished += OnLaunchFinished;
    }

    private void OnDestroy()
    {
        EventList.LaunchFinished -= OnLaunchFinished;
    }

    private void OnLaunchFinished(int launcherId, Launch launch)
    {
        if (launcherId != LauncherId || launches.Contains(launch))
        {
            return;
        }

        while (launches.Count >= MaxHistoryEntries)
        {
            launches.Dequeue();
        }

        launches.Enqueue(launch);

        DisplayHistory();
    }

    private void DisplayHistory()
    {
        if (historyTextObj != null)
        {
            string historyText = "";
            foreach(Launch launch in launches)
            {
                historyText = string.Format("launched - {0} <{1}>\n", launch.PlayerInfo.DisplayName, launch.PuckCount) + historyText;
            }

            historyTextObj.text = historyText;
        }
    }
}
