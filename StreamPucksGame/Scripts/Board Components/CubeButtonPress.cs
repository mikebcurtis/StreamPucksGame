using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeButtonPress : MonoBehaviour
{

    private Renderer rend;
    private Color currentColor;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == Constants.PUCK_TAG && EventList.SwitchPressed != null)
        {
            EventList.SwitchPressed(transform.parent.gameObject, collision.gameObject);

            rend = transform.parent.Find("Light").GetComponent<Renderer>();
            currentColor = rend.material.GetColor("_Color");
            if (currentColor == Color.red)
            {
                rend.material.SetColor("_Color", Color.green);
            }
            else if(currentColor == Color.green)
            {
                rend.material.SetColor("_Color", Color.red);
            }
        }
    }
}
//test
