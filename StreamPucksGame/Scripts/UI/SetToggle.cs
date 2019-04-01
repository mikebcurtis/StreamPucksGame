using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetToggle : MonoBehaviour
{
    public string playerPref;
    public bool flip = false;
    private Toggle toggle;

	// Use this for initialization
	void Start ()
    {
        toggle = GetComponent<Toggle>();
	}

    private void OnEnable()
    {
        if (toggle != null && playerPref != null)
        {
            bool value = PlayerPrefs.GetInt(playerPref) != 0;
            if (flip)
            {
                value = !value;
            }

            toggle.isOn = value;
        }
    }
}
