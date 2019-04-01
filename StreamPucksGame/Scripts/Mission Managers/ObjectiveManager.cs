using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public Text ObjectiveText;
    public bool HitAllSwitches = true;

	// Use this for initialization
	void Start ()
    {
        EventList.AllSwitchesPressed += AllSwitchesPressedHandler;
	}

    private void AllSwitchesPressedHandler()
    {
        if (HitAllSwitches && ObjectiveText != null)
        {
            ObjectiveText.text = "Objective Complete! All switches pressed.";
        }
    }
}
