using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LaunchQueueManager : MonoBehaviour
{
    public GameObject PuckPrefab;
    public int LauncherId;
    public string DefaultUserAvatarPath = "";
    private List<Launch> launchQueue;
    private Launch currentLaunch;
    private int remainingPucks;
    private Vector3 dropPosition;
    private int triggerCount = 0;

	// Use this for initialization
	void Start ()
    {
        BoxCollider dropCollider = this.gameObject.GetComponent<BoxCollider>();
        if (dropCollider != null)
        {
            dropPosition = this.transform.TransformPoint(dropCollider.center);
        }
        else
        {
            dropPosition = this.transform.position;
        }

        launchQueue = new List<Launch>();
        EventList.NewLaunch += NewLaunchHandler;
	}

    private void OnDestroy()
    {
        EventList.NewLaunch -= NewLaunchHandler;
    }

    private void NewLaunchHandler(Launch launch)
    {
        if (launch.LauncherId == LauncherId && 
            launchQueue.Contains(launch) == false && 
            (currentLaunch == null || currentLaunch.Equals(launch) == false))
        {
            launchQueue.Add(launch);
            DropNextPuck();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ++triggerCount;
    }

    /// <summary>
    /// This script assumes there is a box collider attached to the launch tube 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (--triggerCount <= 0)
        {
            DropNextPuck();
        }
    }

    private void DropNextPuck()
    {
        if (currentLaunch != null && remainingPucks > 0)
        {
            --remainingPucks;
            //GameObject puck = Instantiate<GameObject>(PuckPrefab, dropPosition, Quaternion.AngleAxis(-90f, new Vector3(1, 0, 0)));
            GameObject puck = Instantiate<GameObject>(PuckPrefab, dropPosition, PuckPrefab.transform.rotation);
            //GameObject puckGroup = Instantiate<GameObject>(PuckGroupPrefab, dropPosition, Quaternion.identity);
            //GameObject puck = puckGroup.transform.GetChild(0).gameObject;
            LaunchData puckData = puck.AddComponent<LaunchData>();
            puckData.Power = currentLaunch.Power;
            puckData.Angle = currentLaunch.Angle;
            puckData.Player = currentLaunch.PlayerInfo;
            //++triggerCount;
            if (currentLaunch.PlayerInfo.Avatar != null)
            {
                Renderer renderer = puck.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.SetTexture("_MainTex", currentLaunch.PlayerInfo.Avatar);
                }
                else
                {
                    Debug.LogError("Can't find puck renderer component.");
                }
            }

            Text userNameDisplay = puck.GetComponentInChildren<Text>();
            //Text userNameDisplay = puckGroup.GetComponentInChildren<Text>();
            if (userNameDisplay != null)
            {
                userNameDisplay.text = currentLaunch.PlayerInfo.DisplayName;
            }
            else
            {
                Debug.LogError("Can't find puck username display.");
            }

            Rigidbody rb = puck.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                Debug.LogError("Launch Queue Manager could not get rigidbody of the puck prefab.");
            }
        }
        else if (launchQueue.Count > 0)
        {
            currentLaunch = launchQueue[0];
            launchQueue.RemoveAt(0);
            remainingPucks = currentLaunch.PuckCount;
            DropNextPuck();
        }
    }
}
