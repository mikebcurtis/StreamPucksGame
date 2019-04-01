using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail
{
    public string id;
    public GameObject trailObject;
}

public class TrailManager : MonoBehaviour
{
    public static GameObject instance;
    public GameObject[] trails;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There were two TrailManagers and there can only be one. Deleting the gameobject that had the second one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;
    }

    public GameObject GetTrailById(string id)
    {
        if (trails.Length > 0)
        {
            foreach (GameObject trail in trails)
            {
                IdScript idScript = trail.GetComponent<IdScript>();
                if (idScript != null && id == idScript.Value)
                {
                    return trail;
                }
            }
        }

        return null;
    }
}
