using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPuckUsernameToggle : MonoBehaviour
{
    public string hotkey = "u";
    public Toggle toggleScript;

	// Use this for initialization
	void Start ()
    {
        //toggleScript = GetComponent<Toggle>();

        if (toggleScript != null)
        {
            toggleScript.onValueChanged.AddListener(ToggleChangedHandler);
            if (PlayerPrefs.HasKey(Constants.HIDE_PUCK_NAME_KEY) == false)
            {
                PlayerPrefs.SetInt(Constants.HIDE_PUCK_NAME_KEY, 1);
            }
            toggleScript.isOn = PlayerPrefs.GetInt(Constants.HIDE_PUCK_NAME_KEY) != 0;
        }

        EventList.ShowPuckPlayerNameChanged += ShowPuckPlayerNameChangedHandler;
	}

    private void ToggleChangedHandler(bool value)
    {
        if (EventList.ShowPuckPlayerNameChanged != null)
        {
            int intValue = value ? 1 : 0;
            PlayerPrefs.SetInt(Constants.HIDE_PUCK_NAME_KEY, intValue);
            EventList.ShowPuckPlayerNameChanged(intValue);
        }
    }

    private void OnDestroy()
    {
        if (toggleScript != null)
        {
            toggleScript.onValueChanged.RemoveListener(ToggleChangedHandler);
        }
    }

    private void Update()
    {
        if (toggleScript != null && Input.GetKeyDown(hotkey))
        {
            ToggleChangedHandler(!toggleScript.isOn);
        }
    }

    private void ShowPuckPlayerNameChangedHandler(int value)
    {
        if (toggleScript != null)
        {
            toggleScript.isOn = (value != 0);
        }
    }
}
