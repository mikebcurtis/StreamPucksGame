using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjectManager : MonoBehaviour
{
    public MoveObjectManager moveManager;
    public GameObject bumperPrefab;
    public float defaultZ = -17f;
    private GameObject placeObject;

	// Use this for initialization
	void Start ()
    {
        if (moveManager == null)
        {
            Debug.LogError("Must have a MoveObjectManager for the PlaceObjectManager to work.");
            throw new System.Exception("Must have a MoveObjectManager for the PlaceObjectManager to work.");
        }
	}

    private void Update()
    {
        ClickCheck();
    }

    private void ClickCheck()
    {
        if (Input.GetMouseButtonDown(0) && placeObject != null)
        {
            Debug.Log("Object placed, generate new object and have that move."); // DEBUG remove this
            StartCoroutine(GenerateNewPlaceObject());
        }
    }

    private IEnumerator GenerateNewPlaceObject()
    {
        yield return null;
        placeObject = Instantiate<GameObject>(placeObject);
        moveManager.StartMove(placeObject);
    }

    public void StartPlaceBumper()
    {
        if (bumperPrefab != null)
        {
            StartPlace(Instantiate<GameObject>(bumperPrefab));
        }
        else
        {
            Debug.LogError("No prefab given. Cannot place.");
        }
    }

    public void StartPlace(GameObject obj)
    {
        Debug.Log("Start place"); // DEBUG remove this
        placeObject = obj;
        moveManager.StartMove(placeObject);
    }

    public void CancelPlace()
    {
        Debug.Log("Cancel place"); // DEBUG remove this
        if (placeObject != null)
        {
            Debug.Log("   placeObject was not null"); // DEBUG remove this
            moveManager.CancelMove();
            Destroy(placeObject);
        }
    }
}
