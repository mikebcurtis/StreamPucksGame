using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontRequireFocusScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
	}
}
