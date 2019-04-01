using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LauncherScript : MonoBehaviour
{
    public int LauncherId;
    public int Direction = 1; // if positive, turns right. If negative, turns left.
    public float AngleTolerance = 0.8f;
    public float LauncherTurnSpeed = 80f;
    public Vector3 RotatePointOffsetFromCenter;
    public float ForceModifier = 50f;
    //public Text CurrentLaunchText;
    //public Text LaunchQueueText;
    public float CheckForLaunchIntervalSeconds = 1f;
    public GameObject LaunchShuteObj;
    public GameObject PuckPrefab;
    public float MaxFireRate = 0.5f;

    private float timeSinceLastFire;
    private Vector3 rotatePoint;
    private List<Launch> launchQueue;
    private Launch currentLaunch;
    private int remainingPucks;
    private Vector3 startForwardVector;

    private void Awake()
    {
        startForwardVector = new Vector3(1f * (Direction * -1f), 0f, 0f);
        launchQueue = new List<Launch>();
        rotatePoint = this.transform.position + RotatePointOffsetFromCenter;
        EventList.NewLaunch += NewLaunchHandler;
        EventList.LauncherBarrelClear += OnLauncherBarrelClear;
        StartCoroutine(CheckForLaunch());
    }

    private void OnDestroy()
    {
        EventList.NewLaunch -= NewLaunchHandler;
        EventList.LauncherBarrelClear -= OnLauncherBarrelClear;
    }

    private void Update()
    {
        if (timeSinceLastFire < MaxFireRate)
        {
            timeSinceLastFire += Time.deltaTime;
        }
    }

    private void NewLaunchHandler(Launch launch)
    {
        if (launch.LauncherId == LauncherId &&
            launchQueue.Contains(launch) == false &&
            (currentLaunch == null || currentLaunch.RandId != launch.RandId))
        {
            launchQueue.Add(launch);

            if (EventList.LaunchQueueUpdated != null)
            {
                EventList.LaunchQueueUpdated(LauncherId, launchQueue);
            }
        }
    }

    private IEnumerator CheckForLaunch()
    {
        while (true)
        {
            if (currentLaunch == null && launchQueue.Count > 0)
            {
                Launch();
            }

            yield return new WaitForSeconds(CheckForLaunchIntervalSeconds);
        }
    }

    private void Launch()
    {
        if (currentLaunch != null)
        {
            if (remainingPucks > 0)
            {
                var rotateFireCoroutine = RotateAndFire(currentLaunch);
                StartCoroutine(rotateFireCoroutine);
            }
            else
            {
                // remaining pucks is 0 but current launch is still defined, set it to null
                if (EventList.LaunchFinished != null)
                {
                    EventList.LaunchFinished(LauncherId, currentLaunch);
                }

                currentLaunch = null;
                Launch();
            }
        }
        else if (launchQueue.Count > 0)
        {
            currentLaunch = launchQueue[0];
            remainingPucks = currentLaunch.PuckCount;
            launchQueue.RemoveAt(0);

            if (EventList.LaunchQueueUpdated != null)
            {
                EventList.LaunchQueueUpdated(LauncherId, launchQueue);
            }

            var rotateFireCoroutine = RotateAndFire(currentLaunch);
            StartCoroutine(rotateFireCoroutine);
        }
    }

    private void OnLauncherBarrelClear(GameObject launcherBarrel)
    {
        if (launcherBarrel == LaunchShuteObj)
        {
            Launch();
        }
    }

    private IEnumerator RotateAndFire(Launch launch)
    {
        float angle = Vector3.Angle(startForwardVector, this.transform.forward);
        while (angle < (launch.Angle - AngleTolerance) || angle > (launch.Angle + AngleTolerance))
        {
            //float rotateAngle = LauncherTurnSpeed * Time.deltaTime * Mathf.Sign(angle - puckData.Angle);
            float rotateAngle = LauncherTurnSpeed * Time.deltaTime * Mathf.Sign(angle - launch.Angle) * Mathf.Sign(Direction);
            this.transform.RotateAround(rotatePoint, Vector3.forward, rotateAngle);
            yield return null;
            angle = Vector3.Angle(startForwardVector, this.transform.forward);
        }
        
        if (timeSinceLastFire < MaxFireRate)
        {
            yield return new WaitForSeconds(MaxFireRate - timeSinceLastFire);
            timeSinceLastFire = MaxFireRate;
        }
        Fire(launch.Power);
    }

    private void Fire(float force)
    {
        timeSinceLastFire = 0f;
        Vector3 forceVector = this.transform.forward * force * ForceModifier;
        forceVector.z = 0;
        GameObject newPuck = Instantiate<GameObject>(PuckPrefab, this.transform.position, PuckPrefab.transform.rotation);
        GameObject trail = TrailManager.instance.GetComponent<TrailManager>().GetTrailById(currentLaunch.Trail);
        if (trail != null)
        {
            GameObject trailObj = Instantiate<GameObject>(trail, 
                                                          new Vector3(newPuck.transform.position.x, 
                                                                      newPuck.transform.position.y, 
                                                                      newPuck.transform.position.z + 10f), // make sure the trail doesn't move in front of the puck
                                                          Quaternion.identity, 
                                                          newPuck.transform);
        }
        LaunchData launch = newPuck.AddComponent<LaunchData>();
        launch.Angle = currentLaunch.Angle;
        launch.Power = currentLaunch.Power;
        launch.Player = currentLaunch.PlayerInfo;
        if (currentLaunch.PlayerInfo.Avatar != null)
        {
            Renderer renderer = newPuck.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.SetTexture("_MainTex", currentLaunch.PlayerInfo.Avatar);
            }
            else
            {
                Debug.LogError("Can't find puck renderer component.");
            }
        }

        Text userNameDisplay = newPuck.GetComponentInChildren<Text>();
        //Text userNameDisplay = puckGroup.GetComponentInChildren<Text>();
        if (userNameDisplay != null)
        {
            userNameDisplay.text = currentLaunch.PlayerInfo.DisplayName;
        }
        else
        {
            // removing this message for now
            //Debug.LogError("Can't find puck username display.");
        }

        Rigidbody rb = newPuck.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        newPuck.GetComponent<Rigidbody>().AddForce(forceVector);
        if (EventList.NowLaunching != null)
        {
            EventList.NowLaunching(LauncherId, currentLaunch, --remainingPucks);
        }

        if (remainingPucks <= 0)
        {
            if (EventList.LaunchFinished != null)
            {
                EventList.LaunchFinished(LauncherId, currentLaunch);
            }
        }
    }
}
