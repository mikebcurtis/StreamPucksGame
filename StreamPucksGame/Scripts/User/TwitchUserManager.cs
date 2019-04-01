using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

public class TwitchUserManager : MonoBehaviour
{
    public static GameObject instance { get; private set; }

    public TwitchLoginUserInfo userInfo;
    public string ValidateURLDefault = "https://us-central1-twitchplaysballgame.cloudfunctions.net/verifyToken"; // will be overwritten by remote setting, but if there is an error this url will be used
    public string ValidateURLSettingName = "VerifyLoginURL";

    private string verifyUrl;
    private RemoteSettings.UpdatedEventHandler remoteSettingsUpdatedHandler;

    private void Awake()
    {
        if (instance != null)
        {
            if (DestroyThis(instance))
            {
                Debug.LogError("There were two TwitchUserManagers and there can only be one. Deleting one.");
                TwitchUserManager userManager = instance.GetComponent<TwitchUserManager>();
                if (EventList.TwitchUserLogin != null && userManager != null)
                {
                    EventList.TwitchUserLogin(userManager.userInfo);
                }
                Destroy(this.gameObject);
                return;
            }
            else
            {
                Destroy(instance);
            }
        }

        instance = this.gameObject;
        DontDestroyOnLoad(this.gameObject);

        EventList.TwitchUserLogin += SetUserData;
        EventList.TwitchUserLogOut += LogoutHandler;
        //EventList.TwitchUserLoadedFromFile += LoadedUserHandler;

        VerifyHandler();
        remoteSettingsUpdatedHandler = new RemoteSettings.UpdatedEventHandler(VerifyHandler);
        RemoteSettings.Updated += remoteSettingsUpdatedHandler;
    }

    private void OnDestroy()
    {
        EventList.TwitchUserLogin -= SetUserData;
        EventList.TwitchUserLogOut -= LogoutHandler;
        //EventList.TwitchUserLoadedFromFile -= LoadedUserHandler;
        RemoteSettings.Updated -= remoteSettingsUpdatedHandler;
    }

    //private void LoadedUserHandler(TwitchLoginUserInfo userData)
    //{
    //    // TODO verify the user's access token with Twitch
    //    if (EventList.TwitchUserLogin != null)
    //    {
    //        EventList.TwitchUserLogin(userData);
    //    }
    //}

    private void VerifyHandler()
    {
        verifyUrl = RemoteSettings.GetString(ValidateURLSettingName, ValidateURLDefault);
    }

    private void LogoutHandler()
    {
        userInfo = null;
    }

    public void SetUserData(TwitchLoginUserInfo userObj)
    {
        userInfo = userObj;
    }

    private bool DestroyThis(GameObject other)
    {
        TwitchUserManager otherUserManager = other.GetComponent<TwitchUserManager>();
        if (otherUserManager == null)
        {
            return false;
        }

        return otherUserManager.userInfo != null;
    }
}
