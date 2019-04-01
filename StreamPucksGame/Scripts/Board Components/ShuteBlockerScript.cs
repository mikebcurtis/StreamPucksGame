using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuteBlockerScript : MonoBehaviour
{
    public static int PuckLightLayer = 10;
    public static int ChuteLayer = 8;
    public static int PuckLaunchLayer = 9;

    public bool Occupied { get; private set; }

    private void Start()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PuckLaunchLayer)
        {
            Occupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == PuckLaunchLayer)
        {
            other.gameObject.layer = PuckLightLayer;
            Occupied = false;
            if (EventList.LauncherBarrelClear != null)
            {
                EventList.LauncherBarrelClear(this.gameObject);
            }
        }
    }
}
