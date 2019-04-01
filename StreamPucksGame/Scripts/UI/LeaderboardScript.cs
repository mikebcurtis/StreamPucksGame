using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour
{
    public string header = "Leaderboard";
    public int maxSpots = 5;
    private UserManager userManager;
    private Text text;
    private User[] leaders;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
        leaders = new User[maxSpots];
        EventList.PublishUserUpdate += UpdateHandler;
	}

    private void OnDestroy()
    {
        EventList.PublishUserUpdate -= UpdateHandler;
    }

    private void UpdateHandler(User obj)
    {
        if (userManager == null)
        {
            userManager = UserManager.instance.GetComponent<UserManager>();
        }

        if (userManager != null)
        {
            IEnumerable<User> users = userManager.GetUsers();
            var sorted = users.OrderBy(u => u.Points * -1);
            List<User> sortedUserList = sorted.ToList<User>();
            for (int i = 0; i < Mathf.Min(leaders.Length, sortedUserList.Count); i++)
            {
                leaders[i] = sortedUserList.ElementAt<User>(i);
            }

            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (text == null)
        {
            return;
        }

        string display = header + "\n";
        for (int i = 0; i < leaders.Length; i++)
        {
            if (leaders[i] != null)
            {
                display += string.Format("{0} {1} {2}\n", (i + 1), leaders[i].DisplayName, leaders[i].Points);
            }
            else
            {
                display += "- \n";
            }
        }

        text.text = display;
    }
}
