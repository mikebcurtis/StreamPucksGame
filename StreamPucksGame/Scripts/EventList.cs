using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventList
{
    #region Board Component Interaction
    /// <summary>
    /// arg1 is the bumper, arg2 is the puck
    /// </summary>
    public static Action<GameObject, GameObject> BumperHit;

    /// <summary>
    /// arg1 is the launcher, arg2 is the puck
    /// </summary>
    public static Action<GameObject, GameObject> LauncherHit;

    /// <summary>
    /// arg1 is the cup object, arg2 is the puck object
    /// </summary>
    public static Action<GameObject, GameObject> CupCatch;

    /// <summary>
    /// Called when a puck is trapped inside a slot box.
    /// arg1 is the slot box object, arg2 is the puck object
    /// </summary>
    public static Action<GameObject, GameObject> SlotBoxCapture;

    /// <summary>
    /// Called when a puck is released from a slot box.
    /// arg1 is the slot box object, arg2 is the puck object
    /// </summary>
    public static Action<GameObject, GameObject> SlotBoxRelease;

    /// <summary>
    /// Called when a block is hit by a puck.
    /// arg1 is the block, arg2 is the puck object
    /// </summary>
    public static Action<GameObject, GameObject> BlockHit;

    /// <summary>
    /// Called when a puck strikes a switch.
    /// arg1 is the switch gameobject, arg2 is the puck
    /// </summary>
    public static Action<GameObject, GameObject> SwitchPressed;

    /// <summary>
    /// Called when all the switches are lit up at the same time.
    /// </summary>
    public static Action AllSwitchesPressed;

    /// <summary>
    /// Called when a puck goes out of bounds.
    /// arg1 is the puck
    /// </summary>
    public static Action<GameObject> PuckOutOfBounds;

    /// <summary>
    /// Called to indicate a prize should change its value (if it is configured to do so)
    /// arg1 is the gameobject containing the Prize component
    /// </summary>
    public static Action<GameObject> RandomizePrize;
    #endregion

    #region Launch Management

    /// <summary>
    /// Intended to be called after a new launch happens and has been put into a launch object
    /// </summary>
    public static Action<Launch> NewLaunch;

    /// <summary>
    /// Sends out the raw launch data as received from database.
    /// </summary>
    public static Action<Dictionary<string, object>> NewLaunchRaw;

    /// <summary>
    /// arg1 is the launcher id, arg2 is the launch queue
    /// </summary>
    public static Action<int, IEnumerable<Launch>> LaunchQueueUpdated;

    /// <summary>
    /// arg1 is the launcher Id, arg2 is the launch object, arg3 is the remaining pucks that need to be launched still
    /// </summary>
    public static Action<int, Launch, int> NowLaunching;

    /// <summary>
    /// arg1 is the launcher Id, arg2 is the launch object
    /// </summary>
    public static Action<int, Launch> LaunchFinished;

    public static Action<GameObject> LauncherBarrelClear;

    #endregion

    #region User Management

    /// <summary>
    /// Called when the user manager needs details about a particular user from the database.
    /// </summary>
    public static Action<string> RequestUserInfo;

    /// <summary>
    /// Called when the database presents new user
    /// </summary>
    public static Action<Dictionary<string, object>> UserInfoRaw;

    /// <summary>
    /// Called when the user manager wishes to update the database with user info
    /// </summary>
    public static Action<User> PublishUserUpdate;

    /// <summary>
    /// Called when the user successfully logs into Twitch
    /// </summary>
    public static Action<TwitchLoginUserInfo> TwitchUserLogin;

    /// <summary>
    /// Called when the user indicates they want to log out
    /// </summary>
    public static Action TwitchUserLogOut;

    /// <summary>
    /// Called when user data is loaded from file. arg1 is the user info loaded from file.
    /// </summary>
    public static Action<TwitchLoginUserInfo> TwitchUserLoadedFromFile;

    /// <summary>
    /// Called when verification of Twitch login information fails.
    /// </summary>
    public static Action<TwitchLoginUserInfo> TwitchUserRejected;

    /// <summary>
    /// Called when browser navigates to new url
    /// </summary>
    public static Action<string> BrowserNavigation;

    /// <summary>
    /// Called when the database receives a new upgrade
    /// </summary>
    public static Action<Dictionary<string, object>> NewUpgradeRaw;

    /// <summary>
    /// Called when an upgrade was processed and can now be deleted.
    /// </summary>
    public static Action<Upgrade> UpgradeProcessed;

    #endregion

    #region Rank/Mission Management

    /// <summary>
    /// Called whenever a mission is completed.
    /// arg1 is the id of the mission that was completed.
    /// </summary>
    public static Action<string> MissionCompleted;

    /// <summary>
    /// Called whenever the rank has changed (usually do to completing all missions of previous rank).
    /// arg1 is the name of the new rank
    /// </summary>
    public static Action<int> RankChange;

    /// <summary>
    /// Called whenever a mission is started
    /// arg1 is the name of the mission
    /// </summary>
    public static Action<string> MissionStarted;
    #endregion

    #region Configuration Management
    /// <summary>
    /// Called whenever the configuration is changed for showing a puck's player's name above the puck.
    /// arg1 is the new configuration value
    /// </summary>
    public static Action<int> ShowPuckPlayerNameChanged;
    #endregion

    #region Display Text Management
    /// <summary>
    /// Called when a new message should be displayed repeatedly on a rotation. Adding a message with the same main text as another already rotating string will replace that string.
    /// arg1 is the main string to display, arg2 is the detail text to display
    /// </summary>
    public static Action<string, string> AddRotatingText;

    /// <summary>
    /// Called when a new message should be displayed immediately and then not be displayed again.
    /// arg1 is the main string to display, arg2 is the detail text to display
    /// </summary>
    public static Action<string, string> AlertText;
    #endregion
}
