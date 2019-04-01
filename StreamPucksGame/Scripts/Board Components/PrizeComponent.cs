using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizeComponent : MonoBehaviour
{
    public bool Randomize = true;
    public int initPoints = 0; // only used when randomize is false
    public int initPucks = 0; // only used when randomize is false
    public int prizeTier = 0;
    public static Dictionary<string, int> pointMultipliers;
    public static Dictionary<string, int> puckMultipliers;
    public int Points
    {
        get
        {
            return points;
        }

        set
        {
            points = value;
            UpdateDisplay();
        }
    }
    public int Pucks
    {
        get
        {
            return pucks;
        }

        set
        {
            pucks = value;
            UpdateDisplay();
        }
    }
    public int[] initialPoints = new int[] { 0, 5, 10, 20, 30, 50 };
    public int[] initialPucks = new int[] { 0, 1, 2, 3, 4, 5 };
    public int PointMultiplier = 1;
    public int PuckMultiplier = 1;
    public string otherType;
    public Sprite optionalImage;
    public Text textDisplay;

    private int points = 0;
    private int pucks = 0;

	// Use this for initialization
	void Start ()
    {
        if (Randomize)
        {
            Points = 0;
            Pucks = 0;

            while (Points == 0 && Pucks == 0)
            {
                Points = initialPoints[UnityEngine.Random.Range(0, initialPoints.Length)];
                Pucks = initialPucks[UnityEngine.Random.Range(0, initialPucks.Length)];
            }
        }
        else
        {
            Points = initPoints;
            Pucks = initPucks;
        }

        if (pointMultipliers == null)
        {
            pointMultipliers = new Dictionary<string, int>();
        }

        if (puckMultipliers == null)
        {
            puckMultipliers = new Dictionary<string, int>();
        }

        EventList.CupCatch += CupCatchHandler;
	}

    private void OnDestroy()
    {
        EventList.CupCatch -= CupCatchHandler;
    }

    private void CupCatchHandler(GameObject cup, GameObject puck)
    {
        LaunchData launchData = puck.GetComponent<LaunchData>();
        if (launchData != null)
        {

            pointMultipliers[launchData.Player.Id] = PointMultiplier;
            puckMultipliers[launchData.Player.Id] = PuckMultiplier;
        }
        else
        {
            Debug.LogError("PrizeComponent - Cup caught puck that has no launch data attached.");
        }
    }

    public int GetPoints(string userId)
    {
        int points = Points;
        if (pointMultipliers.ContainsKey(userId))
        {
            points *= pointMultipliers[userId];
        }

        return points;
    }

    public int GetPucks(string userId)
    {
        int pucks = Pucks;
        if (puckMultipliers.ContainsKey(userId))
        {
            pucks *= puckMultipliers[userId];
        }

        return pucks;
    }

    public string GenerateString()
    {
        string result = "";
        if (Points > 0)
        {
            result += Points.ToString() + " Pts ";
        }

        if (Pucks > 0)
        {
            string puckWord = " Pucks ";
            if (Pucks == 1)
            {
                puckWord = " Puck ";
            }
            result += Pucks.ToString() + puckWord;
        }

        if (PointMultiplier > 1)
        {
            result += PointMultiplier.ToString() + "x Pts ";
        }

        if (PuckMultiplier > 1)
        {
            result += PuckMultiplier.ToString() + "x Pucks ";
        }

        if (string.IsNullOrEmpty(otherType) == false)
        {
            result += otherType + " ";
        }

        return result;
    }

    private void UpdateDisplay()
    {
        if (textDisplay != null)
        {
            textDisplay.text = GenerateString();
        }
    }
}
