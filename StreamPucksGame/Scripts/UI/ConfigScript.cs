using System.Collections;
using System.Collections.Generic;
using System.IO;
using MK.Glow;
//using UnityEditor;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ConfigScript : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Toggle HowToPlayToggle;
    public GameObject ImageText;
    public string NoBackgroundText = "None";
    public string BackgroundErrorText = "Error!";
    public GameObject NoneImage;
    public int BackgroundImageWidth = 308;
    public int BackgroundImageHeight = 173;
    public GameObject BackgroundRawImage;
    public Material glowingTextMaterial;
    public Material[] bumperMaterials;

    private string musicVolumeKey = Constants.MUSIC_VOLUME_KEY;

	// Use this for initialization
	void Start ()
    {
        // sfx volume initialization
        if (PlayerPrefs.HasKey(Constants.SFX_VOLUME_KEY) == false)
        {
            PlayerPrefs.SetFloat(Constants.SFX_VOLUME_KEY, 0.75f);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat(Constants.SFX_VOLUME_KEY);
            OnSfxSliderChange();
        }

        // music volume initialization
        if (PlayerPrefs.HasKey(musicVolumeKey) == false)
        {
            PlayerPrefs.SetFloat(musicVolumeKey, 0.75f);
        }


        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat(musicVolumeKey);
            OnMusicSliderChange();
        }

        // how to play toggle initialization
        if (HowToPlayToggle != null)
        {
            HowToPlayToggle.isOn = PlayerPrefs.GetInt(Constants.DoNotShowAgainKey, 0) == 1;
        }

        // background img initialization
        if (PlayerPrefs.HasKey(Constants.BACKGROUND_IMG_KEY) && 
            string.IsNullOrEmpty(PlayerPrefs.GetString(Constants.BACKGROUND_IMG_KEY, "")) == false)
        {
            LoadBackgroundImage(PlayerPrefs.GetString(Constants.BACKGROUND_IMG_KEY));
        }
	}

    public void OnSfxSliderChange()
    {
        if (sfxSlider != null)
        {
            PlayerPrefs.SetFloat(Constants.SFX_VOLUME_KEY, sfxSlider.value);
        }
    }

    public void OnMusicSliderChange()
    {
        if (musicSlider != null)
        {
            PlayerPrefs.SetFloat(musicVolumeKey, musicSlider.value);
            if (musicSource != null)
            {
                musicSource.volume = musicSlider.value;
            }
        }
    }

    public void OnHowToPlayToggleChanged()
    {
        PlayerPrefs.SetInt(Constants.DoNotShowAgainKey, HowToPlayToggle.isOn ? 1 : 0);
    }

    public void SetBackgroundImage()
    {
        //string path = EditorUtility.OpenFilePanelWithFilters("Set Stream Pucks Background Image", "", new string []{ "Image Files", "png,jpg,jpeg" });
        var extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
            };
        string path = StandaloneFileBrowser.OpenFilePanel("Set Background Image", "", extensions, false)[0];
        LoadBackgroundImage(path);
    }

    private void LoadBackgroundImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(data))
            {
                NoneImage.SetActive(false);
                ImageText.SetActive(false);
                BackgroundRawImage.SetActive(true);
                TextureScale.Bilinear(tex, BackgroundImageWidth, BackgroundImageHeight);
                BackgroundRawImage.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, BackgroundImageWidth, BackgroundImageHeight), new Vector2(0, 0));
                PlayerPrefs.SetString(Constants.BACKGROUND_IMG_KEY, path);
            }
            else
            {
                ErrorBackgroundImage();
            }
        }
        else
        {
            ErrorBackgroundImage();
        }
    }

    public void ClearBackgroundImage()
    {
        BackgroundRawImage.SetActive(false);
        NoneImage.SetActive(true);
        ImageText.GetComponent<Text>().text = NoBackgroundText;
        ImageText.SetActive(true);
        PlayerPrefs.SetString(Constants.BACKGROUND_IMG_KEY, "");
    }

    public void ErrorBackgroundImage()
    {
        ClearBackgroundImage();
        ImageText.GetComponent<Text>().text = BackgroundErrorText;
    }

    public void SetTextBumperColorClick()
    {
        Shader glowShader = Shader.Find("MK/Glow/Selective/UI/Default");

        if (glowShader != null && glowingTextMaterial != null)
        {
            glowingTextMaterial.SetColor("_Color", Color.red);
            glowingTextMaterial.SetColor("_MKGlowColor", Color.red);
            glowingTextMaterial.SetColor("_MKGlowTexColor", Color.red);
        }

        foreach(Material mat in bumperMaterials)
        {
            mat.SetColor("_EmissionColor", Color.red);
        }
    }
}
