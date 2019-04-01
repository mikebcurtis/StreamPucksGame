using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using ZenFulcrum.EmbeddedBrowser;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private static string CONNECT_TO_TWITCH = "Connect To Twitch";
    private static string SIGN_OUT = "Sign Out";
    private static string NOT_CONNECTED = "Not Connected";

    public GameObject[] Canvases;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject endlessLevelsMenu;
    public GameObject howToPlayMenu;

    public GameObject[] RankCanvases;

    // Twitch user UI
    public Text usernameText;
    public GameObject userProfilePicturePuck;
    public GameObject SignInOutButton;
    public GameObject ConnectPromptParent;
    public float BrowserZoomLevel = 4;
    public string DefaultTwitchLoginURL = "https://id.twitch.tv/oauth2/authorize?client_id=szo8vi33lrosktcp620grq9c0xm1j6&redirect_uri=https%3A%2F%2Fstreampucks.streamgenie.tv%2Foauth%2Fauthorize.html&response_type=token";
    public string TwitchLoginURLSettingName = "TwitchLoginURL";

    private TwitchLoginUserInfo userInfo;
    private int translateAmt = 2000;
    private string twitchLoginURL;
    private RemoteSettings.UpdatedEventHandler remoteSettingsUpdatedHandler;
    private bool continueToCampaign = true;

    private void Awake()
    {
        EventList.TwitchUserLogin += SetUserData;
        remoteSettingsUpdatedHandler = new RemoteSettings.UpdatedEventHandler(SetTwitchLoginURL);
        RemoteSettings.Updated += remoteSettingsUpdatedHandler;
    }

    private void OnDestroy()
    {
        EventList.TwitchUserLogin -= SetUserData;
        RemoteSettings.Updated -= remoteSettingsUpdatedHandler;
    }

    void Start ()
    {
        SetTwitchLoginURL();
        if (ConnectPromptParent != null)
        {
            Browser browser = ConnectPromptParent.GetComponentInChildren<Browser>();
            if (browser != null)
            {
                browser.Zoom = BrowserZoomLevel;
            }

            ConnectPromptParent.transform.Translate(new Vector3(-1 * translateAmt, 0, 0));
            ConnectPromptParent.SetActive(true);
        }

        //userInfo = TwitchUserManager.instance.GetComponent<TwitchUserManager>().userInfo;
        //if (userInfo != null)
        //{
        //    SetUserData(userInfo);
        //}
    }

    private void SetTwitchLoginURL()
    {
        twitchLoginURL = RemoteSettings.GetString(TwitchLoginURLSettingName, DefaultTwitchLoginURL);
    }

    public void DisableAllMenus()
    {
        foreach(GameObject canvas in Canvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);  
            }
        }

        foreach(GameObject canvas in RankCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
    }

    public void MainMenuReturn()
    {
        DisableAllMenus();
        mainMenu.SetActive(true);
    }

    public void CreditsMenuButton()
    {
        DisableAllMenus();
        creditsMenu.SetActive(true);
    }

    public void SettingsMenuButton()
    {
        DisableAllMenus();
        settingsMenu.SetActive(true);
    }

    public void PlayCampaignMenuButton()
    {
        int doNotShowAgainValue = PlayerPrefs.GetInt(Constants.DoNotShowAgainKey, 0);
        continueToCampaign = true;
        if (doNotShowAgainValue == 1)
        {
            if (userInfo != null)
            {
                DisableAllMenus();
                ShowCurrentRankMenu();
            }
            else
            {
                SignInOutClick();
            }
        }
        else
        {
            DisableAllMenus();
            howToPlayMenu.SetActive(true);
        }
    }

    public void PlayEndlessMenuButton()
    {
        int doNotShowAgainValue = PlayerPrefs.GetInt(Constants.DoNotShowAgainKey, 0);
        continueToCampaign = false;
        if (doNotShowAgainValue == 1)
        {
            if (userInfo != null)
            {
                DisableAllMenus();
                endlessLevelsMenu.SetActive(true);
            }
            else
            {
                SignInOutClick();
            }
        }
        else
        {
            DisableAllMenus();
            howToPlayMenu.SetActive(true);
        }
    }

    private void ShowCurrentRankMenu()
    {
        GameData gameData = GameData.instance.GetComponent<GameData>();
        if (gameData != null)
        {
            int currentRank = gameData.RankData.currentRank;
            RankCanvases[currentRank].SetActive(true);
        }
        else
        {
            RankCanvases[0].SetActive(true);
        }
    }

    public void ExitMenuButton()
    {
        Application.Quit();
    }

    public void HowToPlayContinueButton()
    {
        if (userInfo != null)
        {
            DisableAllMenus();
            if (continueToCampaign)
            {
                ShowCurrentRankMenu();
            }
            else
            {
                endlessLevelsMenu.SetActive(true);
            }
        }
        else
        {
            SignInOutClick();
        }
    }

    public void ShowRankMenu(int rankIdx)
    {
        if (rankIdx < RankCanvases.Length)
        {
            DisableAllMenus();
            RankCanvases[rankIdx].SetActive(true);
        }
        else
        {
            Debug.LogError("Tried to load rank menu that was out of range.");
        }
    }

    #region User Stuff

    public void SetUserData(TwitchLoginUserInfo userObj)
    {
        userInfo = userObj;
        if (usernameText != null)
        {
            usernameText.text = userObj.login;
        }

        if (SignInOutButton != null)
        {
            Text buttonText = SignInOutButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = SIGN_OUT;
            }
        }

        StartCoroutine(SetProfilePicture());

        ConnectPromptBackClick();
    }

    private IEnumerator SetProfilePicture()
    {
        // TODO implement this someday pleeeeassseee
        //Texture2D tex = new Texture2D(300, 300, TextureFormat.DXT5, false);
        //using (WWW www = new WWW())
        //{

        //}

        //using (WWW www = new WWW(user.AvatarUrl))
        //{
        //    yield return www;
        //    www.LoadImageIntoTexture(tex);
        //}
        // renderer.material.SetTexture("_MainTex", currentLaunch.PlayerInfo.Avatar);
        yield return null;
    }

    public void SignInOutClick()
    {
        if (userInfo == null)
        {
            // user is not signed in, show the connect screen
            if (ConnectPromptParent != null)
            {
                if (ConnectPromptParent.transform.position.x > -1 == false)
                {
                    ConnectPromptParent.transform.Translate(new Vector3(translateAmt, 0, 0));
                }
                //ConnectPromptParent.SetActive(true);
                Browser browser = ConnectPromptParent.GetComponentInChildren<Browser>();
                browser.Zoom = BrowserZoomLevel;
                browser.Url = twitchLoginURL;
            }
        }
        else
        {
            // user is signed in, sign them out
            if (EventList.TwitchUserLogOut != null)
            {
                EventList.TwitchUserLogOut();
            }

            if (SignInOutButton != null)
            {
                Text buttonText = SignInOutButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = CONNECT_TO_TWITCH;
                }

                if (usernameText != null)
                {
                    usernameText.text = NOT_CONNECTED;
                }
            }

            userInfo = null;
        }
    }

    public void ConnectPromptBackClick()
    {
        if (ConnectPromptParent != null)
        {
            if (ConnectPromptParent.transform.position.x > -1)
            {
                ConnectPromptParent.transform.Translate(new Vector3(-1 * translateAmt, 0, 0));
            }

            Browser browser = ConnectPromptParent.GetComponentInChildren<Browser>();
            if (browser != null && browser.Url != twitchLoginURL)
            {
                browser.Zoom = BrowserZoomLevel;
                browser.Url = twitchLoginURL;
            }
        }
    }

    #endregion
}
