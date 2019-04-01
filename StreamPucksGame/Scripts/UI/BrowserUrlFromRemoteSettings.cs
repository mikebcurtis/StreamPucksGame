using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;

public class BrowserUrlFromRemoteSettings : MonoBehaviour
{
    public string DefaultURL = "http://streampucks.streamgenie.tv/launcher/launcher_background.html";
    public string RemoteSettingName = "LauncherBackgroundURL";
    private Browser browser;
    private string browserUrl;
    private RemoteSettings.UpdatedEventHandler remoteSettingsHandler;

    private void Awake()
    {
        remoteSettingsHandler = new RemoteSettings.UpdatedEventHandler(RemoteSettingsUpdatedHandler);
        RemoteSettings.Updated += RemoteSettingsUpdatedHandler;
    }

    private void OnDestroy()
    {
        RemoteSettings.Updated -= remoteSettingsHandler;
    }

    // Use this for initialization
    void Start ()
    {
        browser = GetComponent<Browser>();
        StartCoroutine(CheckForUrl());
    }

    private IEnumerator CheckForUrl(int retries = 4, float waitTime = 2f)
    {
        while(retries-- > 0)
        {
            RemoteSettingsUpdatedHandler();
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void RemoteSettingsUpdatedHandler()
    {
        browserUrl = RemoteSettings.GetString(RemoteSettingName);
        SetUrl();
    }

    private void SetUrl()
    {
        if (browser != null && string.IsNullOrEmpty(browserUrl) == false && browser.Url != browserUrl)
        {
            browser.LoadURL(browserUrl, true);
            browser.Url = browserUrl;
        }
    }
}
