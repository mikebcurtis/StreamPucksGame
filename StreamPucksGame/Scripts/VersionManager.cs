using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[assembly:AssemblyVersionAttribute("0.0.*")]

public class VersionManager : MonoBehaviour
{
    public Text versionText;
    public Button getLatestVersionButton;

    private Version currentVersion;

	// Use this for initialization
	void Start ()
    {
        currentVersion = typeof(VersionManager).Assembly.GetName().Version;
        versionText.text = currentVersion.ToString();
        if (CheckVersion() == false)
        {
            // show the button 
        }
	}

    private bool CheckVersion()
    {
        Version gameVersion;
        Version latestVersion;

        try
        {
            gameVersion = typeof(VersionManager).Assembly.GetName().Version;
            latestVersion = new Version(RemoteSettings.GetString("LatestVersion"));
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is FormatException || ex is OverflowException)
            {
                Debug.LogError("Cannot parse version.");
                return false;
            }

            throw;
        }

        if (latestVersion.Major > gameVersion.Major || latestVersion.Minor > gameVersion.Minor)
        {
            // TODO enforce upgrade
            return false;
        }

        return true;
    }
}
