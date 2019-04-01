using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

[Serializable]
public class TwitchLoginUserInfo
{
    public string login;
    public string user_id;
    public string client_id;
    public string hash;
    public string access_token;

    public TwitchLoginUserInfo()
    {
        login = null;
        user_id = null;
        client_id = null;
        hash = null;
        access_token = null;
    }

    public TwitchLoginUserInfo(string _login, string _user_id, string _client_id, string _hash, string _access_token)
    {
        login = _login;
        user_id = _user_id;
        client_id = _client_id;
        hash = _hash;
        access_token = _access_token;
    }
}

public class TwitchLoginScript : MonoBehaviour
{
    private static GameObject instance;

    public Action<bool> CheckLoginHandler;
    public string ValidateURLDefault = "https://us-central1-twitchplaysballgame.cloudfunctions.net/verifyToken"; // remote setting will be used, but if there is an error this url will be used
    public string ValidateURLSettingName = "VerifyLoginURL";
    public string OauthCallbackURLDefault = "https://streampucks.streamgenie.tv/oauth/authorize.html";
    public string OauthCallbackURLSettingName = "OAuthCallbackURL";
    public float secWait = 4f;

    //public Browser browser;
    private string token;
    private string validateURL;
    private string oauthCallbackHost = "streampucks.streamgenie.tv";
    private float navigateTime;

    private void Awake()
    {
        RemoteSettingsUpdatedHandler();
        RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler(RemoteSettingsUpdatedHandler);

        EventList.TwitchUserLoadedFromFile += UserLoadedHandler;
    }

    private void Start()
    {
        if (instance != null)
        {
            Debug.LogError("There were two TwitchLoginScript and there can only be one. Deleting the gameobject that had the second one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;

        navigateTime = -1f;

        //browser = GetComponent<Browser>();
        //if (browser != null)
        //{
        //    browser.onNavStateChange += OnNavigateHandler;
        //}

        EventList.BrowserNavigation += OnNavigateHandler;
    }

    private void OnDestroy()
    {
        //if (browser != null)
        //{
        //    browser.onNavStateChange -= OnNavigateHandler;
        //}

        EventList.TwitchUserLoadedFromFile -= UserLoadedHandler;
        EventList.BrowserNavigation -= OnNavigateHandler;
    }
    
    private void UserLoadedHandler(TwitchLoginUserInfo loadedData)
    {
        token = loadedData.access_token;
        StartCoroutine(CheckTwitchToken());
    }

    private void RemoteSettingsUpdatedHandler()
    {
        validateURL = RemoteSettings.GetString(ValidateURLSettingName, ValidateURLDefault);
        SetOAuthCallbackHost();
    }

    private void SetOAuthCallbackHost()
    {
        string url = RemoteSettings.GetString(OauthCallbackURLSettingName, OauthCallbackURLDefault);
        Uri tmp = new Uri(url);
        oauthCallbackHost = tmp.Host;
    }

    private void OnNavigateHandler(string url)
    {
        //if (browser == null)
        //{
        //    return;
        //}

        //string url = browser.Url;

        string tmpToken = "";
        if (TryGetToken(url, out tmpToken))
        {
            token = tmpToken;
            StartCoroutine(CheckTwitchToken());
        }

        //Debug.Log("Browser navigation: " + url); // DEBUG
    }

    private bool TryGetToken(string url, out string token)
    {
        Uri navigatedUri;
        try
        {
            navigatedUri = new Uri(url);
        }
        catch (UriFormatException ex)
        {
            token = null;
            return false;
        }

        if (navigatedUri != null && 
            string.IsNullOrEmpty(navigatedUri.Fragment) == false &&
            string.IsNullOrEmpty(navigatedUri.Host) == false &&
            navigatedUri.Host == oauthCallbackHost &&
            (navigateTime < 0 || Time.fixedTime - navigateTime >= secWait))
        {
            navigateTime = Time.fixedTime;
            string[] pairs = (navigatedUri.Fragment.Remove(0, 1)).Split('&');
            Dictionary<string, string> fragments = new Dictionary<string, string>();
            foreach (string pair in pairs)
            {
                string[] keyValueArr = pair.Split('=');
                if (keyValueArr[0] != null)
                {
                    fragments.Add(keyValueArr[0], keyValueArr.Length > 1 ? keyValueArr[1] : "");
                }
            }

            if (fragments.ContainsKey("access_token"))
            {
                token = fragments["access_token"];
                return true;
            }
        }

        token = null;
        return false;
    }

    public IEnumerator CheckTwitchToken()
    {
        if (string.IsNullOrEmpty(token))
        {
            if (EventList.TwitchUserLogOut != null)
            {
                EventList.TwitchUserLogOut();
            }
            yield break;
        }
        else
        {
            //UnityWebRequest req = UnityWebRequest.Get("https://id.twitch.tv/oauth2/validate");
            UnityWebRequest req = UnityWebRequest.Get(validateURL);
            //req.SetRequestHeader("Authorization", "OAuth " + token);
            req.SetRequestHeader("Authorization", token);
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                string debugMsg = "";
                if (req.isNetworkError)
                {
                    debugMsg = "Network error when checking Twitch token.";
                }
                else if (req.isHttpError)
                {
                    debugMsg = "HTTP error when checking Twitch token.";
                }

                Debug.LogError(debugMsg);
                Debug.LogError(req.error);
                if (EventList.TwitchUserLogOut != null)
                {
                    EventList.TwitchUserLogOut();
                }
                yield break;
            }
            else
            {
                //Debug.Log(req.downloadHandler.text);
                TwitchLoginUserInfo userInfo = JsonUtility.FromJson<TwitchLoginUserInfo>(req.downloadHandler.text);
                userInfo.access_token = token;
                if (string.IsNullOrEmpty(userInfo.login) == false && string.IsNullOrEmpty(userInfo.user_id) == false && EventList.TwitchUserLogin != null)
                {
                    EventList.TwitchUserLogin(userInfo);
                }
                else if (EventList.TwitchUserLogOut != null)
                {
                    EventList.TwitchUserLogOut();
                }

                yield break;
            }
        }
    }
}
