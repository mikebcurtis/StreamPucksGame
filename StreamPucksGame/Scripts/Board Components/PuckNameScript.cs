using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuckNameScript : MonoBehaviour
{
    public GameObject puckNameTextObj;

	// Use this for initialization
	void Start ()
    {
        ShowHidePuckName(PlayerPrefs.GetInt(Constants.HIDE_PUCK_NAME_KEY, 1));

        EventList.ShowPuckPlayerNameChanged += ShowPuckPlayerNameChangedHandler;
	}

    private void OnDestroy()
    {
        EventList.ShowPuckPlayerNameChanged -= ShowPuckPlayerNameChangedHandler;
    }

    private void ShowPuckPlayerNameChangedHandler(int value)
    {
        ShowHidePuckName(value);
    }

    private void ShowHidePuckName(int showNameValue)
    {
        puckNameTextObj.SetActive(showNameValue == 0);
    }
}
