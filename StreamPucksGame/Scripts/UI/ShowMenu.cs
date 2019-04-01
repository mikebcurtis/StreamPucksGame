using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenu : MonoBehaviour
{
    public GameObject menuCanvas;

    public void OnClick()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
    }
}
