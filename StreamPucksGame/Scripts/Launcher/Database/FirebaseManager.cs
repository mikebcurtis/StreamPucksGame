using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class FirebaseManager : MonoBehaviour
{
    public static GameObject instance;
    private static string IdKeyStr = "id";

    public string channelId = "193884700";
    public float userUpdateMinIntervalSec = 4f;
    public float launchesDeleteMinIntervalSec = 4f;
    public float upgradesDeleteMinIntervalSec = 4f;
    public string hash;
    public int MaxRecentLaunchListSize = 400;
    public string DefaultUpdateUsersURL = "https://us-central1-twitchplaysballgame.cloudfunctions.net/updateUsers";
    public string UpdateUsersURLSettingName = "UpdateUsersURL";
    public string DefaultDeleteLaunchesURL = "https://us-central1-twitchplaysballgame.cloudfunctions.net/deleteLaunches";
    public string DeleteLaunchesURLSettingName = "DeleteLaunchesURL";
    public string DefaultDatabaseURL = "https://twitchplaysballgame.firebaseio.com/";
    public string DatabaseURLSettingName = "DatabaseURL";
    public string DefaultDeleteUpgradesURL = "https://us-central1-twitchplaysballgame.cloudfunctions.net/deleteUpgrades";
    public string DeleteUpgradesURLSettingName = "DeleteUpgradesURL";
    public string LevelStartedURLSettingName = "LevelStartedURL";
    public string DefaultLevelStartedURL = "https://us-central1-twitchplaysballgame.cloudfunctions.net/levelStarted";

    private string databaseUrl = "https://twitchplaysballgame.firebaseio.com/";
    private string updateUsersUrl;
    private string deleteLaunchesUrl;
    private string deleteUpgradesUrl;
    private string missionStartedAlertUrl;
    private RemoteSettings.UpdatedEventHandler remoteSettingsUpdatedHandler;
    private DatabaseReference launchesRef;
    private string launchesRoot = "launches";
    private DatabaseReference playersRef;
    private string playersRoot = "players";
    private string puckCountName = "puckCount";
    private string pointsName = "points";
    private DatabaseReference upgradesRef;
    private string upgradesName = "upgrades";
    
    private Queue<string> recentLaunches;
    private List<string> deletedLaunches;
    private List<string> deletedUpgrades;
    private List<User> updatedUsers;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There were two FirebaseManagers and there can only be one. Deleting the gameobject that had the second one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;

        if (TwitchUserManager.instance != null)
        {
            TwitchLoginUserInfo userInfo = TwitchUserManager.instance.GetComponent<TwitchUserManager>().userInfo;
            channelId = userInfo.user_id;
            hash = userInfo.hash;
        }
        else if(string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(hash))
        {
            Debug.LogError("Could not get channel Id or hash. Unable to set up Firebase.");
            return;
        }

        recentLaunches = new Queue<string>();
        deletedLaunches = new List<string>();
        deletedUpgrades = new List<string>();
        updatedUsers = new List<User>();

        RemoteSettingsUpdated();
        remoteSettingsUpdatedHandler = new RemoteSettings.UpdatedEventHandler(RemoteSettingsUpdated);
        RemoteSettings.Updated += remoteSettingsUpdatedHandler;

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(databaseUrl);
        launchesRef = FirebaseDatabase.DefaultInstance.GetReference(string.Format("{0}/{1}", launchesRoot, channelId));
        launchesRef.ChildAdded += OnLaunchAdded;

        EventList.PublishUserUpdate += UserInfoUpdatedHandler;

        playersRef = FirebaseDatabase.DefaultInstance.GetReference(string.Format("{0}/{1}", playersRoot, channelId));
        playersRef.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.Message);
                return;
            }
            
            LoadPlayerData(task.Result);
        });
        playersRef.ChildAdded += OnPlayerChanged;
        playersRef.ChildChanged += OnPlayerChanged;

        upgradesRef = FirebaseDatabase.DefaultInstance.GetReference(string.Format("{0}/{1}", upgradesName, channelId));
        upgradesRef.ChildAdded += OnUpgradeAdded;
        EventList.UpgradeProcessed += OnUpgradeProcessed;

        EventList.MissionStarted += MissionStartedHandler;
    }

    private void OnDestroy()
    {
        launchesRef.ChildAdded -= OnLaunchAdded;
        EventList.PublishUserUpdate -= UserInfoUpdatedHandler;
        playersRef.ChildAdded -= OnPlayerChanged;
        playersRef.ChildChanged -= OnPlayerChanged;
        upgradesRef.ChildAdded -= OnUpgradeAdded;
        RemoteSettings.Updated -= remoteSettingsUpdatedHandler;
        EventList.UpgradeProcessed -= OnUpgradeProcessed;
        EventList.MissionStarted -= MissionStartedHandler;
    }

    private void Start()
    {
        StartCoroutine(DeleteAllLaunches());
        StartCoroutine(DeleteLaunchCheck());
        StartCoroutine(UpdateUsers());
        StartCoroutine(DeleteUpgradesCheck());
    }

    private void RemoteSettingsUpdated()
    {
        SetDatabaseURL();
        SetUpdateUsersURL();
        SetDeleteLaunchesURL();
        SetDeleteUpgradesURL();
        SetLevelStartedURL();
    }

    private void SetDatabaseURL()
    {
        databaseUrl = RemoteSettings.GetString(DatabaseURLSettingName, DefaultDatabaseURL);
    }

    private void SetUpdateUsersURL()
    {
        updateUsersUrl = RemoteSettings.GetString(UpdateUsersURLSettingName, DefaultUpdateUsersURL);
    }

    private void SetDeleteLaunchesURL()
    {
        deleteLaunchesUrl = RemoteSettings.GetString(DeleteLaunchesURLSettingName, DefaultDeleteLaunchesURL);
    }

    private void SetDeleteUpgradesURL()
    {
        deleteUpgradesUrl = RemoteSettings.GetString(DeleteUpgradesURLSettingName, DefaultDeleteUpgradesURL);
    }

    private void SetLevelStartedURL()
    {
        missionStartedAlertUrl = RemoteSettings.GetString(LevelStartedURLSettingName, DefaultLevelStartedURL);
    }

    private void MissionStartedHandler(string missionName)
    {
        StartCoroutine(MissionStartedSendAlert(missionName));
    }

    private IEnumerator MissionStartedSendAlert(string gameMode)
    {
        while (missionStartedAlertUrl == null)
        {
            yield return null;
        }

        UnityWebRequest req = UnityWebRequest.Get(missionStartedAlertUrl + "?channelId=" + channelId + "&gameMode=" + gameMode);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", hash);
        yield return req.SendWebRequest();
    }

    private IEnumerator UpdateUsers()
    {
        while (true)
        {
            yield return new WaitForSeconds(userUpdateMinIntervalSec);

            if (updatedUsers.Count > 0)
            {
                User[] updatedUsersCopy = new User[updatedUsers.Count]; // copy, because it could be updated during our web request
                updatedUsers.CopyTo(updatedUsersCopy);
                string bodyData = UsersToJson(updatedUsersCopy);
                //Debug.Log("Sending string: " + bodyData); // DEBUG
                UnityWebRequest req = UnityWebRequest.Put(updateUsersUrl + "?channelId=" + channelId, bodyData);
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Authorization", hash);
                yield return req.SendWebRequest();

                // remove updated users
                foreach(User user in updatedUsersCopy)
                {
                    updatedUsers.Remove(user);
                }
            }
        }
    }

    private string UsersToJson(User[] users)
    {
        bool first = true;
        string result = "{";
        foreach(User user in users)
        {
            if (!first)
            {
                result += ",";
            }
            else
            {
                first = false;
            }

            result += string.Format("\"{0}\": {{ \"puckCount\": {1}, \"points\": {2} }}", user.Id, user.Pucks, user.Points);
        }
        result += "}";
        return result;
    }

    private IEnumerator DeleteAllLaunches()
    {
        UnityWebRequest req = UnityWebRequest.Put(deleteLaunchesUrl + "?channelId=" + channelId, "{\"deleteAll\":true}");
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", hash);
        yield return req.SendWebRequest();
    }

    private IEnumerator DeleteLaunchCheck()
    {
        yield return new WaitForSeconds(1); // offset by 1 so it doesn't get called at exactly the same time as the other checks

        while (true)
        {
            yield return new WaitForSeconds(launchesDeleteMinIntervalSec);

            if (deletedLaunches.Count > 0)
            {
                string[] deletedLaunchesCopy = new string[deletedLaunches.Count]; // copy, because it could be updated during our web request
                deletedLaunches.CopyTo(deletedLaunchesCopy);
                string postData = DeleteIdsToJson("launchids", deletedLaunchesCopy);

                UnityWebRequest req = UnityWebRequest.Put(deleteLaunchesUrl + "?channelId=" + channelId, postData);
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Authorization", hash);
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.LogError(req.error);
                }
                else
                {
                    foreach(string launchId in deletedLaunchesCopy)
                    {
                        deletedLaunches.Remove(launchId);
                    }
                }
            }
        }
    }

    private IEnumerator DeleteUpgradesCheck()
    {
        yield return new WaitForSeconds(2); // offset by 2 so it doesn't get called at exactly the same time as the other checks

        while (true)
        {
            yield return new WaitForSeconds(upgradesDeleteMinIntervalSec);

            if (deletedUpgrades.Count > 0)
            {
                string[] deletedUpgradesCopy = new string[deletedUpgrades.Count]; // copy, because it could be updated during our web request
                deletedUpgrades.CopyTo(deletedUpgradesCopy);
                string postData = DeleteIdsToJson("upgradeids", deletedUpgradesCopy);

                UnityWebRequest req = UnityWebRequest.Put(deleteUpgradesUrl + "?channelId=" + channelId, postData);
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Authorization", hash);
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.LogError(req.error);
                }
                else
                {
                    foreach (string upgradeId in deletedUpgradesCopy)
                    {
                        deletedUpgrades.Remove(upgradeId);
                    }
                }
            }
        }
    }

    private string DeleteIdsToJson(string name, string[] deletedIds)
    {
        string json = "{\"" + name + "\":[";
        bool first = true;
        foreach (string id in deletedIds)
        {
            if (!first)
            {
                json += ",";
            }
            else
            {
                first = false;
            }

            json += "\"" + id + "\"";
        }
        json += "]}";

        return json;
    }

    private void OnPlayerChanged(object sender, ChildChangedEventArgs e)
    {
        if (e == null)
        {
            Debug.LogError("Database updated handler was given null arguments.");
            return;
        }

        if (e.DatabaseError != null && e.DatabaseError.Message != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }

        if (e.Snapshot != null && e.Snapshot.Value != null && EventList.UserInfoRaw != null)
        {
            Dictionary<string, object> userData = e.Snapshot.Value as Dictionary<string, object>;
            userData.Add("id", e.Snapshot.Key);
            if (userData != null)
            {
                EventList.UserInfoRaw(userData);
                //Debug.Log(string.Format("User change from firebase: {0}", userData["id"]));
            }
        }
    }

    private void UserInfoUpdatedHandler(User user)
    {
        int idx = updatedUsers.FindIndex((otherUser) => { return otherUser.Id == user.Id; });
        if (idx == -1)
        {
            updatedUsers.Add(user);
        }
        else
        {
            updatedUsers[idx] = user;
        }
    }

    private void LoadPlayerData(DataSnapshot snapshot)
    {
        if (snapshot != null && snapshot.Value != null)
        {
            Dictionary<string, object> playersData = snapshot.Value as Dictionary<string, object>;
            if (EventList.UserInfoRaw != null)
            {
                foreach (KeyValuePair<string, object> pair in playersData)
                {
                    Dictionary<string, object> userInfo = pair.Value as Dictionary<string, object>;
                    userInfo.Add("id", pair.Key);
                    EventList.UserInfoRaw(userInfo);
                }
            }

        }
    }

    private void OnLaunchAdded(object sender, ChildChangedEventArgs e)
    {
        if (e == null)
        {
            Debug.LogError("Database updated handler was given null arguments.");
            return;
        }

        if (e.DatabaseError != null && e.DatabaseError.Message != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }

        if (e.Snapshot != null && e.Snapshot.Value != null)
        {
            //debugDisplay.text = DebugDBResultsToString(e.Snapshot.Value);
            // generate Launch from json
            Dictionary<string, object> launchData = e.Snapshot.Value as Dictionary<string, object>;
            if (launchData != null && EventList.NewLaunchRaw != null && recentLaunches.Contains(launchData[IdKeyStr].ToString().Trim()) == false)
            {
                if (recentLaunches.Count >= MaxRecentLaunchListSize)
                {
                    recentLaunches.Dequeue();
                }

                recentLaunches.Enqueue(launchData[IdKeyStr].ToString().Trim());
                EventList.NewLaunchRaw(launchData);

                //Debug.Log(string.Format("New launch from firebase: {0}", launchData[IdKeyStr]));

                // delete the launch from the database
                deletedLaunches.Add(e.Snapshot.Key);
                //DatabaseReference launchRef = FirebaseDatabase.DefaultInstance.GetReference(string.Format("{0}/{1}/{2}", launchesRoot, channelId, e.Snapshot.Key));
                //launchRef.RemoveValueAsync().ContinueWith(
                //    (Task t) => {
                //        if (t.IsFaulted)
                //        {
                //            Debug.LogError("Failed to delete launch on the database: " + t.Exception.Message);
                //        }
                //    }
                //);
            }
        }
    }

    private void OnUpgradeAdded(object sender, ChildChangedEventArgs e)
    {
        if (e == null)
        {
            Debug.LogError("Database updated handler was given null arguments.");
            return;
        }

        if (e.DatabaseError != null && e.DatabaseError.Message != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }

        if (e.Snapshot != null && e.Snapshot.Value != null && EventList.NewUpgradeRaw != null)
        {
            Dictionary<string, object> upgradeData = e.Snapshot.Value as Dictionary<string, object>;
            upgradeData.Add("id", e.Snapshot.Key);
            EventList.NewUpgradeRaw(upgradeData);
        }
    }

    private void OnUpgradeProcessed(Upgrade upgrade)
    {
        if (deletedUpgrades.Contains(upgrade.id) == false)
        {
            deletedUpgrades.Add(upgrade.id);
        }
    }
}
