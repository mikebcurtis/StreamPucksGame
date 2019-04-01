using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectManager : MonoBehaviour
{
    public GameObject moveObj { get; private set; }

    private Camera viewCamera;

    // Use this for initialization
    void Start()
    {
        viewCamera = FindObjectOfType<Camera>(); // this assumes just one camera, which will probably break things someday
        CancelMove();
    }

    // Update is called once per frame
    void Update()
    {
        MoveLoop();
        CheckClick();
    }

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (moveObj == null)
            {
                GrabObjectIfThere();
            }
            else
            {
                CancelMove();
            }
        }
    }

    private void GrabObjectIfThere()
    {
        RaycastHit hit;
        Vector3 origin = viewCamera.ScreenToWorldPoint(Input.mousePosition);
        origin.z = -100f; // DEBUG change this, magic number
        int layerMask = LayerMask.GetMask(Constants.MOUSE_MOVEABLE_LAYER);
        if (Physics.Raycast(origin, Vector3.forward, out hit, float.MaxValue, layerMask))
        {
            StartMove(hit.collider.gameObject);
        }
    }

    public void StartMove(GameObject obj)
    {
        Debug.Log("Start move"); // DEBUG remove this
        if (moveObj != null)
        {
            CancelMove();
        }

        moveObj = obj;
    }

    public void CancelMove()
    {
        Debug.Log("Cancel move");
        moveObj = null;
    }

    public void MoveLoop()
    {
        if (moveObj != null)
        {
            moveObj.transform.position = GetMovePosition();
        }
    }

    public Vector3 GetMovePosition()
    {
        Vector3 position = viewCamera.ScreenToWorldPoint(Input.mousePosition);
        if (moveObj != null)
        {
            position.z = moveObj.transform.position.z;
        }

        return position;
    }
}
