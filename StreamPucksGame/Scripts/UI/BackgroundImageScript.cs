using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImageScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int width = 1920;
    public int height = 1080;

	// Use this for initialization
	void Start ()
    {
        string path = PlayerPrefs.GetString(Constants.BACKGROUND_IMG_KEY, null);
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (string.IsNullOrEmpty(path))
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (System.IO.File.Exists(path))
            {
                // load background
                var bytes = System.IO.File.ReadAllBytes(path);
                var tex = new Texture2D(1, 1);
                tex.LoadImage(bytes);
                TextureScale.Bilinear(tex, width, height);
                spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0, 0));
            }
            else
            {
                Debug.LogError("Saved path to background image does not point to a file that exists.");
                this.gameObject.SetActive(false);
                return;
            }
        }
	}
}
