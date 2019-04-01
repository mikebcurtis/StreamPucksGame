using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : IEquatable<Launch>
{
    public static bool TryParseLaunch(Dictionary<string, object> launchData, out Launch launchObj)
    {
        string id;
        string uid;
        string randStamp;
        string avatarUrl = "";
        string displayName = null;
        int pucks;
        float angle;
        int side;
        float power;
        string trail = null;

        try
        {
            id = launchData["userId"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("Launch was received with invalid id.");
                launchObj = null;
                return false;
            }

            uid = launchData["opaqueUserId"].ToString();
            if (string.IsNullOrEmpty(uid))
            {
                Debug.LogError("Launch was received with invalid user id.");
                launchObj = null;
                return false;
            }

            randStamp = launchData["id"].ToString();
            if (string.IsNullOrEmpty(randStamp))
            {
                Debug.LogError("Launch was received without a random id.");
                launchObj = null;
                return false;
            }

            if (launchData.ContainsKey("avatarUrl"))
            {
                avatarUrl = launchData["avatarUrl"].ToString();
            }

            if (launchData.ContainsKey("displayName"))
            {
                displayName = launchData["displayName"].ToString();
            }

            if (Int32.TryParse(launchData["pucks"].ToString(), out pucks) == false)
            {
                Debug.LogError("Launch was received with invalid puck count.");
                launchObj = null;
                return false;
            }

            if (float.TryParse(launchData["angle"].ToString(), out angle) == false)
            {
                Debug.LogError("Launch was received with invalid angle.");
                launchObj = null;
                return false;
            }

            if (Int32.TryParse(launchData["side"].ToString(), out side) == false)
            {
                Debug.LogError("Launch was received with invalid launcher id.");
                launchObj = null;
                return false;
            }

            if (float.TryParse(launchData["power"].ToString(), out power) == false)
            {
                Debug.LogError("Launch was received with invalid power.");
                launchObj = null;
                return false;
            }

            if (launchData.ContainsKey("trail"))
            {
                trail = launchData["trail"].ToString();
            }
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            Debug.LogError(ex.Message);
            launchObj = null;
            return false;
        }

        launchObj = new Launch(id,
                               randStamp,
                               new User()
                               {
                                   Id = id,
                                   AvatarUrl = avatarUrl,
                                   Avatar = null,
                                   DisplayName = displayName,
                                   Points = -1,
                                   Pucks = -1
                               },
                               pucks,
                               angle,
                               side,
                               power,
                               trail
                              );
        return true;
    }

    public string Id { get; set; }
    public string RandId { get; set; }
    public User PlayerInfo { get; set; }
    public int PuckCount { get; set; }
    public float Angle { get; set; }
    public int LauncherId { get; set; }
    public float Power { get; set; }
    public string Trail { get; set; }

    public Launch(string id, string randId, User player, int puckCount, float angle, int launcherId, float power, string trail = null)
    {
        Id = id;
        RandId = randId;
        PlayerInfo = player;
        PuckCount = puckCount;
        Angle = angle;
        LauncherId = launcherId;
        Power = power;
        Trail = trail;
    }

    public bool Equals(Launch other)
    {
        return this.Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is Launch)
        {
            return this.Id == (obj as Launch).Id;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public class LaunchData : MonoBehaviour
{
    public float Angle;
    public float Power;
    public User Player;
}
