using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static string PUCK_TAG = "Puck";
    public static string BLOCK_TAG = "Block";
    public static string BUG_PRIZE_TYPE = "Bug!";
    public static string HIDE_PUCK_NAME_KEY = "SHOW_PUCK_NAME_KEY";
    public static string SFX_VOLUME_KEY = "SfxVolume";
    public static string MUSIC_VOLUME_KEY = "MusicVolume";
    public static string DoNotShowAgainKey = "DoNotShowHowToPlay";
    public static string BACKGROUND_IMG_KEY = "BACKGROUND_IMG_KEY";
    public static string MOUSE_MOVEABLE_LAYER = "MouseMoveable";
    public static string GetMissionId(string missionName)
    {
        switch (missionName.ToUpper())
        {
            case "BASIC TRAINING":
                return "0-1";
            case "TARGET PRACTICE":
                return "0-2";
            case "COMMITMENT TO EXCELLENCE":
                return "0-3";
            case "DRESS TO IMPRESS":
                return "0-4";
            case "FIRST DAY ON THE JOB":
                return "1-1";
            case "BASIC MANEUVER":
                return "1-2";
            case "FIREFIGHT":
                return "1-3";
            case "BUG HUNT":
                return "1-4";
            case "BREAKOUT":
                return "2-1";
            case "LOGISTICS":
                return "2-2";
            case "LUCKY ACE":
                return "2-3";
            case "LONG HAUL":
                return "2-4";
            case "CUTTING EDGE":
                return "3-1";
            case "ONSLAUGHT":
                return "3-2";
            case "FOG OF WAR":
                return "3-3";
            case "AMBUSH":
                return "3-4";
            case "CYPHER":
                return "4-1";
            case "JUGGERNAUGHT":
                return "4-2";
            case "INSURRECTION":
                return "4-3";
            case "GLORY OF THE KILL":
                return "4-4";
            case "RED TAPE":
                return "5-1";
            case "DAY OF RECKONING":
                return "5-2";
            case "VALLEY OF DEATH":
                return "5-3";
            case "BADGE OF HONOR":
                return "5-4";
            default:
                return null;
        }
    }

    public static string GetMissionSceneName(string missionName)
    {
        switch(missionName.ToUpper())
        {
            case "BASIC TRAINING":
                return "Cadet_Basic_Training";
            case "TARGET PRACTICE":
                return "Cadet_Target_Practice";
            case "COMMITMENT TO EXCELLENCE":
                return "Cadet_Commitment_to_Excellence";
            case "DRESS TO IMPRESS":
                return "Cadet_Dress_to_Impress";
            case "FIRST DAY ON THE JOB":
                return "Ensign_First_Day_on_the_Job";
            case "BASIC MANEUVER":
                return "Ensign_Basic_Maneuver";
            case "FIREFIGHT":
                return "Ensign_Firefight";
            case "BUG HUNT":
                return "Ensign_Bug_Hunt";
            case "BREAKOUT":
                return "Lieutenant_Breakout";
            case "LOGISTICS":
                return "Lieutenant_Logistics";
            case "LUCKY ACE":
                return "Lieutenant_Lucky_Ace";
            case "LONG HAUL":
                return "Lieutenant_Long_Haul";
            case "CUTTING EDGE":
                return "Commander_Cutting_Edge";
            case "ONSLAUGHT":
                return "Commander_Onslaught";
            case "FOG OF WAR":
                return "Commander_Fog_of_War";
            case "AMBUSH":
                return "Commander_Ambush";
            case "CYPHER":
                return "Captain_Cypher";
            case "JUGGERNAUGHT":
                return "Captain_Juggernaught";
            case "INSURRECTION":
                return "Captain_Insurrection";
            case "GLORY OF THE KILL":
                return "Captain_Glory_of_the_Kill";
            case "RED TAPE":
                return "Admiral_Red_Tape";
            case "DAY OF RECKONING":
                return "Admiral_Day_of_Reckoning";
            case "VALLEY OF DEATH":
                return "Admiral_Valley_of_Death";
            case "BADGE OF HONOR":
                return "Admiral_Badge_of_Honor";
            default:
                return null;
        }
    }

    public static string GetEndlessSceneName(int endlessSceneId)
    {
        switch(endlessSceneId)
        {
            case 1:
                return "Endless_1";
            case 2:
                return "Endless_2";
            case 3:
                return "Endless_3";
            case 4:
                return "Endless_4";
            case 5:
                return "Endless_5";
            case 6:
                return "Endless_6";
            case 7:
                return "Endless_7";
            case 8:
                return "Endless_8";
            default:
                return "Endless_1";
        }
    }
}
