using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSettingsMenuHoverScript : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject SettingsUIElementsObj;
    public float translateDist = 90f;
    public float totalTime = 2f;

    private Vector3 inactivePos;
    private Vector3 activePos;
    private bool active = false;

    private void Start()
    {
        inactivePos = SettingsUIElementsObj.transform.position;
        activePos = inactivePos;
        activePos.y -= translateDist;
    }

    public void OnClick()
    {
        if (SettingsUIElementsObj == null)
        {
            return;
        }

        if (active)
        {
            StartCoroutine(MoveMenu(activePos, inactivePos)); // move up
        }
        else
        {
            StartCoroutine(MoveMenu(inactivePos, activePos)); // move down
        }

        active = !active;
    }

    // TODO make it so the mouseover works instead of making them push a button

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (SettingsUIElementsObj != null)
    //    {
    //        //SettingsUIElementsObj.SetActive(true);
    //        StartCoroutine(MoveMenu(inactivePos, activePos)); // move down
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (SettingsUIElementsObj != null)
    //    {
    //        //SettingsUIElementsObj.SetActive(false);
    //        StartCoroutine(MoveMenu(activePos, inactivePos)); // move up
    //    }
    //}

    private IEnumerator MoveMenu(Vector3 pos1, Vector3 pos2)
    {
        float elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            SettingsUIElementsObj.transform.position = Vector3.Lerp(pos1, pos2, elapsedTime / totalTime);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }
}
