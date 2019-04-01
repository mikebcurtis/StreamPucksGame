using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenuScript : MonoBehaviour
{
    public Toggle doNotShowAgainToggle;
    public string defaultInstallURL = "https://www.twitch.tv/ext/y4jq5ejqgodi64cueqvjdip2ekfg0r-0.0.2";
    public string InstallURLSettingName = "InstallExtensionURL";
    public string defaultInstallBitsURL = "https://www.twitch.tv/ext/4ynfkfb761mf5gbcslrgt8ksov8im6-0.0.1";
    public string InstallBitsURLSettingName = "InstallBitsExtensionURL";

    private string installUrl;
    private string installBitsUrl;
    private RemoteSettings.UpdatedEventHandler remoteSettingsUpdatedHandler;

    private void Start()
    {
        SetInstallUrl();
        remoteSettingsUpdatedHandler = new RemoteSettings.UpdatedEventHandler(SetInstallUrl);
        RemoteSettings.Updated += remoteSettingsUpdatedHandler;
    }

    private void OnEnable()
    {
        doNotShowAgainToggle.isOn = PlayerPrefs.GetInt(Constants.DoNotShowAgainKey, 1) == 1;
    }

    private void OnDestroy()
    {
        RemoteSettings.Updated -= remoteSettingsUpdatedHandler;
    }

    private void SetInstallUrl()
    {
        installUrl = RemoteSettings.GetString(InstallURLSettingName, defaultInstallURL);
        installBitsUrl = RemoteSettings.GetString(InstallBitsURLSettingName, defaultInstallBitsURL);
    }

    public void OnInstallExtensionClick()
    {
        Application.OpenURL(installUrl);
    }

    public void OnInstallBitsExtensionClick()
    {
        Application.OpenURL(installBitsUrl);
    }

    public void OnDoNotShowAgainClick()
    {
        if (doNotShowAgainToggle != null)
        {
            PlayerPrefs.SetInt(Constants.DoNotShowAgainKey, doNotShowAgainToggle.isOn ? 1 : 0);
        }
    }

    public void OnContinueClick()
    {
        if (doNotShowAgainToggle != null)
        {
            PlayerPrefs.SetInt(Constants.DoNotShowAgainKey, doNotShowAgainToggle.isOn ? 1 : 0);
        }
    }
}
