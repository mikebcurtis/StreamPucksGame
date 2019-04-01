using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessMissionManager : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
		if (EventList.MissionStarted != null)
        {
            EventList.MissionStarted("Endless Mode");
        }
	}
}
