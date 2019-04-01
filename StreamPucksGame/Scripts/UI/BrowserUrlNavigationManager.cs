using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;

public class BrowserUrlNavigationManager : MonoBehaviour
{
    private Browser browser;
    private string prevUrl = "";

	void Start ()
    {
        browser = GetComponent<Browser>();
        if (browser != null)
        {
            browser.onNavStateChange += OnNavigateHandler;
        }
    }

    private void OnDestroy()
    {
        if (browser != null)
        {
            browser.onNavStateChange -= OnNavigateHandler;
        }
    }

    private void OnNavigateHandler()
    {
        if (browser != null && 
            EventList.BrowserNavigation != null && 
            string.IsNullOrEmpty(browser.Url) == false && 
            browser.Url != prevUrl)
        {
            EventList.BrowserNavigation(browser.Url);
            prevUrl = browser.Url;
        }
    }
}
