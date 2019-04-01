using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLightUpScript : MonoBehaviour
{
    private Color original;
    private Color targetColor;
    public float time = 0f;
    public float max = 9f;
    public float fadeTime = 0.5f;

	// Use this for initialization
	void Start ()
    {
        EventList.BumperHit += HitHandler;
        EventList.LauncherHit += HitHandler;
        Renderer r = this.GetComponent<Renderer>();
        if (r != null)
        {
            //original = r.material.color;
            //targetColor = r.material.color * Mathf.LinearToGammaSpace(max);
            original = r.material.GetColor("_EmissionColor");
            targetColor = original * Mathf.LinearToGammaSpace(max);
        }
	}

    private void OnDestroy()
    {
        EventList.BumperHit -= HitHandler;
        EventList.LauncherHit -= HitHandler;
    }

    private void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;

            Renderer r = this.GetComponent<Renderer>();
            if (r != null)
            {
                //r.material.color = Color.Lerp(original, targetColor, (time / fadeTime));
                r.material.SetColor("_EmissionColor", Color.Lerp(original, targetColor, (Mathf.Max(time, 0f) / fadeTime)));
            }
        }
    }

    private void HitHandler(GameObject arg1, GameObject arg2)
    {
        if (arg1 == this.gameObject)
        {
            time = fadeTime;
        }
    }
}
