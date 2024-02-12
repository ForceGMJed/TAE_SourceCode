using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAE : MonoBehaviour
{
    public const string FULLSCREEN  = "FullScreen";
    public const string ISEVERPLAYED = "IsPlayedBefore";

    public const string Ach_EnterScene = "Ach_EnterScene";
    public const string Ach_SitDown = "Ach_SitDown";
    public const string Ach_DrinkTen = "Ach_DrinkTen";
    public const string Ach_FinishLog = "Ach_FinishLog";
    public const string Ach_PerfectScore = "Ach_PerfectScore";
    public const string Ach_Guitar = "Ach_Guitar";
    public const string Ach_Challenge = "Ach_Challenge";
    public const string Ach_HundredBread = "Ach_HundredBread";
    public const string Ach_LeaveSober = "Ach_LeaveSober";
    public const string Ach_LeaveDrunk = "Ach_LeaveDrunk";
    public const string Stat_totalBreadThrown = "totalBreadThrown";

    public static string[] Ach_Names = new string[] { Ach_EnterScene, Ach_SitDown, Ach_DrinkTen,
        Ach_FinishLog, Ach_PerfectScore, Ach_Guitar, Ach_Challenge, Ach_HundredBread, Ach_LeaveSober, Ach_LeaveDrunk};
}
