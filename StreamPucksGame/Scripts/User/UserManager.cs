using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public static string ANONYMOUS_NAME = "anonymous";

    public static bool TryParsePucks(Dictionary<string, object> rawData, out int result)
    {
        int pucks = -1;
        result = -1;

        try
        {
            if (rawData == null || rawData["puckCount"] == null || Int32.TryParse(rawData["puckCount"].ToString(), out pucks) == false)
            {
                Debug.LogError("Could not parse puck count.");
                Debug.LogError(rawData);
                return false;
            }
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        result = pucks;
        return true;
    }

    public static bool TryParsePoints(Dictionary<string, object> rawData, out int result)
    {
        int points = -1;
        result = -1;

        try
        {
            if (rawData["points"] == null || Int32.TryParse(rawData["points"].ToString(), out points) == false)
            {
                Debug.LogError("Could not parse points.");
                Debug.LogError(rawData);
                return false;
            }
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        result = points;
        return true;
    }

    public string Id { get; set; }
    public string AvatarUrl { get; set; }
    public int Pucks { get; set; }
    public int Points { get; set; }
    public Texture2D Avatar { get; set; }
    public string DisplayName {
        get
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return ANONYMOUS_NAME;
            }

            return displayName;
        }

        set
        {
            if (value == ANONYMOUS_NAME)
            {
                displayName = null;
            }
            else
            {
                displayName = value;
            }
        }
    }

    private string displayName;
}

public class Upgrade
{
    public string id { get; private set; }
    public int pucks { get; private set; }
    public User source { get; private set; }
    public int pucksMultiplyer { get; private set; }
    public bool applyToAll { get; private set; }

    public Upgrade (string Id, int pucksCount, User sourceUser, bool applyToAllBool)
    {
        id = Id;
        pucks = pucksCount;
        source = sourceUser;
        pucksMultiplyer = 1;
        applyToAll = applyToAllBool;
    }

    public Upgrade (string Id, int pucksCount, int multiplyer, User sourceUser)
    {
        id = Id;
        pucks = pucksCount;
        pucksMultiplyer = multiplyer;
        source = sourceUser;
        applyToAll = false;
    }
}

public class UserManager : MonoBehaviour
{
    public static GameObject instance;

    private Dictionary<string, User> knownUsers;

    private List<User> updatedUsers;

    public IEnumerable<User> GetUsers()
    {
        return knownUsers.Values;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There were two UserManagers and there can only be one. Deleting the gameobject that had the second one.");
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;
        knownUsers = new Dictionary<string, User>();
        updatedUsers = new List<User>();
        EventList.NewLaunchRaw += LaunchRawHandler;
        EventList.CupCatch += CupCatchHandler;
        EventList.SlotBoxRelease += SlotBoxReleaseHandler;
        EventList.UserInfoRaw += UserInfoRawHandler;
        EventList.NewUpgradeRaw += UpgradeHandler;
    }

    private void OnDestroy()
    {
        EventList.NewLaunchRaw -= LaunchRawHandler;
        EventList.CupCatch -= CupCatchHandler;
        EventList.SlotBoxRelease -= SlotBoxReleaseHandler;
        EventList.UserInfoRaw -= UserInfoRawHandler;
        EventList.NewUpgradeRaw -= UpgradeHandler;
    }

    private void UpgradeHandler(Dictionary<string, object> rawJSON)
    {
        if (rawJSON["id"] == null || rawJSON["id"].ToString() == null || rawJSON["id"].ToString() == "")
        {
            Debug.LogError("Could not parse id from upgrade.");
            return;
        }
        string id = rawJSON["id"].ToString();

        int pucks = -1;
        if (User.TryParsePucks(rawJSON, out pucks) == false)
        {
            Debug.LogError("Failed to parse an upgrade's pucks.");
            return;
        }

        User sourceUser;

        if (rawJSON["source"].ToString() != null)
        {
            sourceUser = knownUsers[rawJSON["source"].ToString()];
        }
        else
        {
            Debug.LogError("Could not parse source user Id from upgrade.");
            return;
        }

        bool applyToAll = false;
        if (rawJSON["target"] != null && rawJSON["target"].ToString() == "all")
        {
            ApplyPrizeToAll(0, pucks);
            applyToAll = true;
        }
        else
        {
            ApplyPrize(0, pucks, sourceUser.Id);
            applyToAll = false;
        }

        if (EventList.UpgradeProcessed != null)
        {
            EventList.UpgradeProcessed(new Upgrade(id, pucks, sourceUser, applyToAll));
        }
    }

    private void LaunchRawHandler(Dictionary<string, object> rawData)
    {
        Launch launch;
        if (Launch.TryParseLaunch(rawData, out launch) == false)
        {
            Debug.LogError("Could not parse launch.");
            return;
        }

        string userId = launch.PlayerInfo.Id;
        launch.PlayerInfo = MergeUser(launch.PlayerInfo);

        if (knownUsers[userId].Avatar != null)
        {
            InitiateLaunch(launch);
        }
        else
        {
            StartCoroutine(LaunchAfterDownloadingAvatar(launch));
        }
    }

    private void CupCatchHandler(GameObject cup, GameObject puck)
    {
        PrizeComponent prize = cup.GetComponent<PrizeComponent>();
        LaunchData launchData = puck.GetComponent<LaunchData>();
        if (prize != null && launchData != null)
        {
            string userId = launchData.Player.Id;
            ApplyPrize(prize, userId);
        }
        else
        {
            Debug.LogError("UserManager - can't process cup catch because prize component or launch data was missing.");
        }
    }

    private void SlotBoxReleaseHandler(GameObject slotBox, GameObject puck)
    {
        SlotBoxScript slotScript = slotBox.GetComponentInChildren<SlotBoxScript>();
        if (slotScript == null)
        {
            Debug.LogError("UserManager - cannot process prize component because slot box did not have a slot box script.");
            return;
        }

        LaunchData launch = puck.GetComponent<LaunchData>();
        if (launch == null)
        {
            Debug.LogError("UserManager - cannot process prize component because the puck did not have an attached launch data object.");
            return;
        }

        PrizeComponent prize = slotScript.GetPrize();
        string userId = launch.Player.Id;
        ApplyPrize(prize, userId);
    }

    private void UserInfoRawHandler(Dictionary<string, object> rawUser)
    {
        if (rawUser.ContainsKey("id") == false)
        {
            return;
        }

        int pucks = -1;
        int points = -1;

        if (User.TryParsePucks(rawUser, out pucks) == false)
        {
            pucks = 30;
        }

        if (User.TryParsePoints(rawUser, out points) == false)
        {
            points = 0;
        }

        User newUser = new User { Id = rawUser["id"].ToString(), Points = points, Pucks = pucks };
        MergeUser(newUser);
    }

    private void InitiateLaunch(Launch launch)
    {
        string userId = launch.PlayerInfo.Id;
        if (knownUsers.ContainsKey(userId) == false || knownUsers[userId].Pucks <= -1)
        {
            Debug.LogError(string.Format("Launch rejected. Unknown user. id: {0} user: {1} launchPuckCount: {2} actualPuckCount: {3}", launch.Id, userId, launch.PuckCount, knownUsers[userId].Pucks));
            return;
        }

        if (knownUsers[userId].Pucks < launch.PuckCount)
        {
            // user does not have enough pucks to launch this amount
            Debug.LogError(string.Format("Launch rejected. Insufficient pucks. id: {0} user: {1} launchPuckCount: {2} actualPuckCount: {3}", launch.Id, userId, launch.PuckCount, knownUsers[userId].Pucks));

            // send update of user info to database, because they obviously don't have the right count
            if (EventList.PublishUserUpdate != null)
            {
                EventList.PublishUserUpdate(knownUsers[userId]);
            }

            return;
        }
        else
        {
            // update puck count and laumch
            int puckUpdate = knownUsers[userId].Pucks - launch.PuckCount;
            if (puckUpdate <= 0)
            {
                puckUpdate = 1;
            }

            knownUsers[userId].Pucks = puckUpdate;

            if (EventList.PublishUserUpdate != null)
            {
                EventList.PublishUserUpdate(knownUsers[userId]);
            }

            if (EventList.NewLaunch != null)
            {
                EventList.NewLaunch(launch);
            }
        }
    }

    private IEnumerator LaunchAfterDownloadingAvatar(Launch launch)
    {
        User user = launch.PlayerInfo;
        if (knownUsers.ContainsKey(user.Id) == false)
        {
            knownUsers.Add(user.Id, user);
        }

        if (string.IsNullOrEmpty(user.AvatarUrl))
        {
            // no Avatar url was sent with the launch
            InitiateLaunch(launch);
        }
        else
        {
            Texture2D tex = new Texture2D(300, 300, TextureFormat.DXT5, false);
            using (WWW www = new WWW(user.AvatarUrl))
            {
                yield return www;
                www.LoadImageIntoTexture(tex);
                knownUsers[user.Id].Avatar = tex;

                launch.PlayerInfo = knownUsers[user.Id];
                InitiateLaunch(launch);
            }
        }
    }
    
    private User MergeUser(User user)
    {
        if (knownUsers.ContainsKey(user.Id))
        {
            User mergedUser = knownUsers[user.Id];
            mergedUser.AvatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? mergedUser.AvatarUrl : user.AvatarUrl;
            mergedUser.Avatar = user.Avatar ?? mergedUser.Avatar;
            if (user.DisplayName != User.ANONYMOUS_NAME)
            {
                mergedUser.DisplayName = user.DisplayName;
            }
            
            if (user.Points != null && user.Points >= 0)
            {
                mergedUser.Points = user.Points;
            }

            if (user.Pucks != null && user.Pucks >= 0)
            {
                mergedUser.Pucks = user.Pucks;
            }

            knownUsers[user.Id] = mergedUser;
            //if (EventList.PublishUserUpdate != null)
            //{
            //    EventList.PublishUserUpdate(mergedUser);
            //}
        }
        else
        {
            knownUsers.Add(user.Id, user);
        }

        return knownUsers[user.Id];
    }

    private void ApplyPrize(PrizeComponent prize, string userId)
    {
        int pointsUpdate = prize.GetPoints(userId);
        int puckUpdate = prize.GetPucks(userId);

        ApplyPrize(pointsUpdate, puckUpdate, userId);
    }

    private void ApplyPrize(int points, int pucks, string userId)
    {
        if (knownUsers.ContainsKey(userId))
        {
            knownUsers[userId].Points += points;
            knownUsers[userId].Pucks += pucks;

            if (EventList.PublishUserUpdate != null)
            {
                EventList.PublishUserUpdate(knownUsers[userId]);
            }
        }
        else
        {
            Debug.LogError("UserManager - can't process prize because user is unknown.");
        }
    }
    
    private void ApplyPrizeToAll(int points, int pucks)
    {
        foreach(string userKey in knownUsers.Keys)
        {
            ApplyPrize(points, pucks, userKey);
        }
    }
}
