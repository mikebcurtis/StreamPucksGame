using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentLaunchDisplayText : MonoBehaviour
{
    public int LauncherId;
    private Text currentLaunchTextObject;

	// Use this for initialization
	void Start ()
    {
        currentLaunchTextObject = this.gameObject.GetComponent<Text>();
        EventList.NowLaunching += OnCurrentLaunchUpdated;
	}

    private void OnDestroy()
    {
        EventList.NowLaunching -= OnCurrentLaunchUpdated;
    }

    private void OnCurrentLaunchUpdated(int launcherId, Launch launch, int remainingPucks)
    {
        if (launcherId == LauncherId && currentLaunchTextObject != null)
        {
            if (remainingPucks <= 0)
            {
                currentLaunchTextObject.text = "Launching: ";
            }
            else
            {
                currentLaunchTextObject.text = string.Format("Launching: {0} <{1}>", launch.PlayerInfo.DisplayName, launch.PuckCount);
            }
        }
    }
}
