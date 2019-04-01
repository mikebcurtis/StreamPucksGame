using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupCollectScript : MonoBehaviour
{
    public float fadeTime = 0.5f;
    public Color hitColor = Color.red;
    private Color originalColor;
    private float remainingTime = 0f;

    public void Start()
    {
        originalColor = transform.parent.GetComponent<Renderer>().material.color;
    }

    public void Update()
    {
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0f)
            {
                remainingTime = 0f;
            }

            transform.parent.GetComponent<Renderer>().material.color = Color.Lerp(originalColor, hitColor, remainingTime / fadeTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != Constants.PUCK_TAG)
        {
            return;
        }

        remainingTime = fadeTime;

        if (EventList.CupCatch != null)
        {
            EventList.CupCatch(transform.parent.gameObject, other.gameObject);
        }

        ParticleSystem ps = transform.parent.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }

            ps.Play();
        }
        Destroy(other.gameObject);

        if (EventList.RandomizePrize != null)
        {
            EventList.RandomizePrize(transform.parent.gameObject);
        }
    }
}
