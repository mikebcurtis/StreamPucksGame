using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchManager : MonoBehaviour
{
    public GameObject SwitchParent;
    private int HowManyPressedForWin;
    private List<GameObject> HitSwitches;

	// Use this for initialization
	void Start ()
    {
        HitSwitches = new List<GameObject>();
        if (SwitchParent != null)
        {
            HowManyPressedForWin = SwitchParent.transform.GetComponentsInChildren<SwitchScript>().Length;
        }
        else
        {
            Debug.LogError("SwitchManager can't determine how many switches are in the scene.");
            Destroy(this.gameObject);
            return;
        }
        EventList.SwitchPressed += SwitchPressedHandler;
	}

    private void OnDestroy()
    {
        EventList.SwitchPressed -= SwitchPressedHandler;
    }

    private void SwitchPressedHandler(GameObject switchObj, GameObject puckObj)
    {
        if (HitSwitches.Contains(switchObj))
        {
            HitSwitches.Remove(switchObj);
        }
        else
        {
            HitSwitches.Add(switchObj);

            if (HitSwitches.Count >= HowManyPressedForWin && EventList.AllSwitchesPressed != null)
            {
                EventList.AllSwitchesPressed();
            }
        }
    }
}
