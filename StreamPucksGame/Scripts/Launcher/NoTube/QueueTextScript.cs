using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class QueueTextScript : MonoBehaviour
{
    public int LauncherId;
    private Text TextDisplayObject;

	// Use this for initialization
	void Start ()
    {
        TextDisplayObject = this.gameObject.GetComponent<Text>();
        EventList.LaunchQueueUpdated += OnQueueUpdated;
	}

    private void OnDestroy()
    {
        EventList.LaunchQueueUpdated -= OnQueueUpdated;
    }

    private void OnQueueUpdated(int launcherId, IEnumerable<Launch> launchQueue)
    {
        if (launcherId == LauncherId && launchQueue != null)
        {
            StringBuilder sb = new StringBuilder();
            foreach(Launch launch in launchQueue)
            {
                sb.Append(string.Format("queued - {0} <{1}>\n", launch.PlayerInfo.DisplayName, launch.PuckCount));
            }

            string displayString = sb.ToString();

            if (string.IsNullOrEmpty(displayString))
            {
                displayString = "queued - ";
            }

            TextDisplayObject.text = displayString;
        }
    }
}
