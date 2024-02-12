using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievementHub : MonoBehaviour
{
    public static SteamAchievementHub steamAchievementHub;
    protected Callback<UserAchievementStored_t> AchievementStoredCallback;


    [SerializeField]
    private bool isResetAllInBegining;

    private void Awake()
    {
        if (steamAchievementHub != null && steamAchievementHub != this)
        {
            Destroy(this.gameObject);
            return;
        }

        steamAchievementHub = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        GameManager.Achievement_EnterGameScene += OnAchievement_EnterGameScene;
        CinematicChair.Achievement_Chair += OnAchievement_Chair;
        AlcoholStation.Achievements_Drink10Times += OnAchievements_Drink10Times;
        GameManager.PlayerReadyToLeave += OnAchievement_FinishLogs;
        BreadLauncher.Achievement_HundredBread += OnAchievement_HundredBread;
        GameManager.Achievement_MaxScore += OnAchievement_MaxScore;
        Guitar.Achievement_Guitar += OnAchievement_Guitar;
        PowerBox.Achievement_ChallengeAccepted += OnAchievement_ChallengeAccepted;
        Helicopter.Achievement_LeaveDrunk += OnAchievement_LeaveDrunk;
        Helicopter.Achievement_LeaveSober += OnAchievement_LeaveSober;

        GameManager.GameEnd += OnGameEnd;

        AchievementStoredCallback = Callback<UserAchievementStored_t>.Create(OnUserAchievementStored);
    }

    private void OnDisable()
    {
        GameManager.Achievement_EnterGameScene -= OnAchievement_EnterGameScene;
        CinematicChair.Achievement_Chair -= OnAchievement_Chair;
        AlcoholStation.Achievements_Drink10Times -= OnAchievements_Drink10Times;
        GameManager.PlayerReadyToLeave -= OnAchievement_FinishLogs;
        BreadLauncher.Achievement_HundredBread -= OnAchievement_HundredBread;
        GameManager.Achievement_MaxScore -= OnAchievement_MaxScore;
        Guitar.Achievement_Guitar -= OnAchievement_Guitar;
        PowerBox.Achievement_ChallengeAccepted -= OnAchievement_ChallengeAccepted;
        Helicopter.Achievement_LeaveDrunk -= OnAchievement_LeaveDrunk;
        Helicopter.Achievement_LeaveSober -= OnAchievement_LeaveSober;

        GameManager.GameEnd -= OnGameEnd;

        AchievementStoredCallback = null;

    }


    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steamworks is not INIT");
            return;
        }


#if UNITY_EDITOR

        if (isResetAllInBegining)
        {
            SteamUserStats.ResetAllStats(true);
        }


#endif

        for (int i = 0; i < TAE.Ach_Names.Length; i++)
        {
            UpdateAchievementsToSteam(TAE.Ach_Names[i]);
        }

        SteamUserStats.StoreStats();


    }
    /// <summary>
    /// For users that played in offline mode, once they get play online their achievemenmts will be sync
    /// </summary>
    private void UpdateAchievementsToSteam(string s)
    {
        if (PlayerPrefs.GetInt(s, 0) == 1)
        {
            CheckAndSetSteamAchievements(s, false);
        }
    }
    private void CheckAndSetSteamAchievements(string s, bool isSendToSteamNow)
    {
        bool isUnlocked;
        SteamUserStats.GetAchievement(s, out isUnlocked);

        if (!isUnlocked)
        {
            SteamUserStats.SetAchievement(s);
        }

        if (!isSendToSteamNow)
        {
            return;
        }

        SteamUserStats.StoreStats();
    }
    private void OnUserAchievementStored(UserAchievementStored_t achStored)
    {
        Debug.Log("Achievement: " + achStored.m_rgchAchievementName + " unlocked.");
    }


    //OnAch call backs from interaction/etc
    private void OnAchievement_LeaveSober()
    {


        PlayerPrefs.SetInt(TAE.Ach_LeaveSober, 1);
        //Debug.Log("OnAchievement_LeaveSober");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_LeaveSober, true);
    }

    private void OnAchievement_LeaveDrunk()
    {
        PlayerPrefs.SetInt(TAE.Ach_LeaveDrunk, 1);
        // Debug.Log("OnAchievement_LeaveDrunk");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_LeaveDrunk, true);
    }

    private void OnAchievement_ChallengeAccepted()
    {
        PlayerPrefs.SetInt(TAE.Ach_Challenge, 1);
        // Debug.Log("OnAchievement_ChallengeAccepted");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_Challenge, true);
    }

    private void OnAchievement_Guitar()
    {
        PlayerPrefs.SetInt(TAE.Ach_Guitar, 1);
        // Debug.Log("OnAchievement_Guitar");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_Guitar, true);
    }

    private void OnAchievement_MaxScore()
    {
        PlayerPrefs.SetInt(TAE.Ach_PerfectScore, 1);
        // Debug.Log("OnAchievement_MaxScore"); 
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_PerfectScore, true);
    }

    private void OnAchievement_HundredBread()
    {
        PlayerPrefs.SetInt(TAE.Ach_HundredBread, 1);

        //Debug.Log("OnAchievement_HundredBread"); 
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_HundredBread, true);
    }

    private void OnAchievement_FinishLogs()
    {
        PlayerPrefs.SetInt(TAE.Ach_FinishLog, 1);
        // Debug.Log("OnAchievement_FinishLogs");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_FinishLog, true);
    }

    private void OnAchievements_Drink10Times()
    {
        PlayerPrefs.SetInt(TAE.Ach_DrinkTen, 1);
        //Debug.Log("OnAchievements_Drink10Times");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_DrinkTen, true);
    }

    private void OnAchievement_Chair()
    {
        PlayerPrefs.SetInt(TAE.Ach_SitDown, 1);
        // Debug.Log("OnAchievement_Chair");
        if (!SteamManager.Initialized)
            return;
        CheckAndSetSteamAchievements(TAE.Ach_SitDown, true);
    }

    private void OnAchievement_EnterGameScene()
    {
        PlayerPrefs.SetInt(TAE.Ach_EnterScene, 1);
        // Debug.Log("OnAchievement_EnterGameScene");
        if (!SteamManager.Initialized)
            return;

        CheckAndSetSteamAchievements(TAE.Ach_EnterScene, true);

    }


    //callbacks to update bread thrown count for global stat
    private void OnGameEnd(int breadThrown)
    {
        int previousTotal = 0;

        SteamUserStats.GetStat(TAE.Stat_totalBreadThrown, out previousTotal);
        Debug.LogError("previousTotal: " + previousTotal + " breadThrown this session: " + breadThrown );
        SteamUserStats.SetStat(TAE.Stat_totalBreadThrown, breadThrown + previousTotal);

        SteamUserStats.StoreStats();
    }



}
