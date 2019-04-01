using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public bool Toggleable = true;
    public float noRehitTime = 1f;
    public GameObject lastHit;

    public bool state
    {
        get;
        private set;
    }
    private Renderer rend;
    private Color offColor;
    private Color onColor;

    private void Start()
    {
        rend = this.GetComponent<Renderer>();
        if (rend != null)
        {
            onColor = rend.material.GetColor("_EmissionColor");
            offColor = Color.black;

            rend.material.SetColor("_EmissionColor", state ? onColor : offColor);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == Constants.PUCK_TAG &&
            (Toggleable == true || state == false) && 
            (lastHit == null || lastHit != collision.gameObject))
        {
            if (EventList.SwitchPressed != null)
            {
                EventList.SwitchPressed(transform.gameObject, collision.gameObject);
                lastHit = collision.gameObject;
                StartCoroutine(ClearLastHitAfterWait());
            }

            state = !state;

            if (rend != null)
            {
                rend.material.SetColor("_EmissionColor", state ? onColor : offColor);
            }
        }
    }

    private IEnumerator ClearLastHitAfterWait()
    {
        yield return new WaitForSeconds(noRehitTime);
        lastHit = null;
    }
}
