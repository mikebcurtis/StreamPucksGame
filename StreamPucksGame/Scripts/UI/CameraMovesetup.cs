using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovesetup : MonoBehaviour {

    public Vector3 destiny;
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.Translate(destiny);
	}
}
