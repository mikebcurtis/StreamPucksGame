using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOnHitScript : MonoBehaviour
{
    public float fadeTime = 0.5f;
    public Color hitColor = Color.red;
    private Color originalColor;
    private float remainingTime = 0f;

    public void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
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

            GetComponent<Renderer>().material.color = Color.Lerp(originalColor, hitColor, remainingTime / fadeTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        remainingTime = fadeTime;
    }
}
