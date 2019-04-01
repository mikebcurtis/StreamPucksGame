using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringPair : IEquatable<StringPair>
{
    public string main { get; private set; }
    public string detail { get; private set; }
    public float duration { get; private set; }
    public float typeSpeed { get; private set; }

    public StringPair(string mainStr, string detailStr, float tDuration, float tTypeSpeed)
    {
        main = mainStr;
        detail = detailStr;
        duration = tDuration;
        typeSpeed = tTypeSpeed;
    }

    public bool Equals(StringPair other)
    {
        return main.Equals(other.main);
    }

    public override bool Equals(object obj)
    {
        StringPair other = obj as StringPair;
        if (other != null)
        {
            return Equals(other);
        }
        else
        {
            return false;
        }
    }
}

public class TitleDisplayManagerScript : MonoBehaviour
{
    public Text MainText;
    public Text SubText;
    public string[] CongratulationStrings;
    public int alertQueueSpeedupSize = 10;

    private List<StringPair> rotatingStrings;
    private List<StringPair> alertQueue;
    private int currentIdx = 0;
    private StringPair currentPair;
    
	void Awake ()
    {
        rotatingStrings = new List<StringPair>();
        alertQueue = new List<StringPair>();
        currentIdx = 0;
        MainText.text = "";
        SubText.text = "";
        EventList.UpgradeProcessed += UpgradeProcessedHandler;
        EventList.AddRotatingText += AddRotatingTextHandler;
        EventList.AlertText += AlertTextHandler;
	}

    private void OnDestroy()
    {
        EventList.UpgradeProcessed -= UpgradeProcessedHandler;
        EventList.AddRotatingText -= AddRotatingTextHandler;
        EventList.AlertText -= AlertTextHandler;
    }

    private void AddRotatingTextHandler(string main, string detail)
    {
        Add(main, detail);
    }

    private void AlertTextHandler(string main, string detail)
    {
        float duration = alertQueue.Count > alertQueueSpeedupSize ? 5f : 10f;
        float typeSpeed = alertQueue.Count > alertQueueSpeedupSize ? 0.1f : 0.05f;
        Alert(main, detail, duration, typeSpeed);
    }

    private void UpgradeProcessedHandler(Upgrade upgrade)
    {
        if (upgrade.applyToAll)
        {
            AlertTextHandler(upgrade.source.DisplayName + " just gave everyone " + upgrade.pucks + " pucks!", GetCongratulationString());
        }
        else
        {
            AlertTextHandler(upgrade.source.DisplayName + " acquired " + upgrade.pucks + " pucks.", "");
        }
    }

    private void Update()
    {
        if (currentPair == null && (rotatingStrings.Count > 0 || alertQueue.Count > 0))
        {
            StartCoroutine(DisplayStringPair(GetNextStringPair()));
        }
    }

    //private void Start()
    //{
    //    StartCoroutine(CycleMessages());
    //}

    //private IEnumerator CycleMessages()
    //{
    //    currentPair = null;
    //    while (true)
    //    {
    //        yield return null;

    //        if (displayStrings.Count <= 0)
    //        {
    //            continue;
    //        }

    //        elapsedTime += Time.deltaTime;

    //        if (currentPair == null || currentPair.duration <= elapsedTime)
    //        {
    //            if (displayStrings.Count > 1 || currentPair == null)
    //            {
    //                if (currentIdx >= displayStrings.Count)
    //                {
    //                    currentIdx = 0;
    //                }

    //                StopCoroutine("DisplayStringPair");
    //                currentPair = displayStrings[currentIdx++];
    //                StartCoroutine(DisplayStringPair(currentPair));
    //            }

    //            elapsedTime = 0f;
    //        }
    //    }
    //}

    private string GetCongratulationString()
    {
        if (CongratulationStrings.Length <= 0)
        {
            return "";
        }

        return CongratulationStrings[UnityEngine.Random.Range(0, CongratulationStrings.Length)];
    }

    public void Alert(string mainString, string detailString, float duration = 10f, float typeSpeed = 0.1f)
    {
        StopAllCoroutines();
        //DisplayStringPair(new StringPair(mainString, detailString, duration, typeSpeed));
        currentPair = null;
        alertQueue.Add(new StringPair(mainString, detailString, duration, typeSpeed));
    }

    public void Add(string mainString, string detailString, float duration = 5f, float typeSpeed = 0.1f)
    {
        StringPair newPair = new StringPair(mainString, detailString, duration, typeSpeed);
        if (rotatingStrings.Contains(newPair))
        {
            rotatingStrings.Remove(newPair);
        }

        rotatingStrings.Add(newPair);

        if (currentPair != null && currentPair.Equals(newPair))
        {
            StopAllCoroutines();
            currentPair = newPair;
            StartCoroutine(DisplayStringPair(currentPair));
        }
    }

    private StringPair GetNextStringPair()
    {
        if (alertQueue.Count > 0)
        {
            // erase the alert message - these are only shown once
            StringPair tmp = alertQueue[0];
            alertQueue.RemoveAt(0);
            currentPair = tmp;
            return tmp;
        }

        if (rotatingStrings.Count <= 0)
        {
            return null;
        }

        if (++currentIdx >= rotatingStrings.Count)
        {
            currentIdx = 0;
        }

        currentPair = rotatingStrings[currentIdx];
        return currentPair;
    }

    private IEnumerator DisplayStringPair(StringPair stringPair)
    {
        float elapsedTime = 0;

        if (stringPair == null)
        {
            MainText.text = "";
            SubText.text = "";
            yield break;
        }

        if (stringPair.typeSpeed <= 0 || stringPair.main == MainText.text)
        {
            // if no typespeed, just display the string without the typing effect
            MainText.text = stringPair.main;
            SubText.text = stringPair.detail;
        }
        else
        {
            int length = 1;
            while (length <= stringPair.main.Length || length <= stringPair.detail.Length)
            {
                MainText.text = stringPair.main.Substring(0, Math.Min(length, stringPair.main.Length));
                SubText.text = stringPair.detail.Substring(0, Math.Min(length, stringPair.detail.Length));
                yield return new WaitForSeconds(stringPair.typeSpeed);
                elapsedTime += stringPair.typeSpeed;
                if (elapsedTime >= stringPair.duration)
                {
                    break;
                }
                ++length;
            }
        }

        // wait remaining time
        if (elapsedTime < stringPair.duration)
        {
            yield return new WaitForSeconds(stringPair.duration - elapsedTime);
        }

        // display next string
        currentPair = GetNextStringPair();
        StartCoroutine(DisplayStringPair(currentPair));
    }
}
