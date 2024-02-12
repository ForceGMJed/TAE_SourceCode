using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static event Action<bool> SetCursorAndLookState;
    public static event Action PlayerReadyToLeave;
    public static event Action EnableArcade;
    public static event Action<float> SetPanThrowState;
    public static event Action TrialLegitEnd;
    public static event Action<float> UserSensitivityChange;
    public static event Action ExtractStartMoveHeli;
    public static event Action<bool, float> LerpHeliRotor;
    public static event Action BeginGame;
    public static event Action<bool> TestingMode;
    public static event Action Achievement_EnterGameScene;
    public static event Action Achievement_MaxScore;
    /// <summary>
    /// Only Sub to this event if ur a persistant object
    /// </summary>
    public static event Action<int> GameEnd;

    [Header("Prefabs")]

    [SerializeField]
    GameObject InfoCanvasPrefab;
    [SerializeField]
    GameObject AudioPrefab;
    [SerializeField]
    GameObject AudioPrefab_Timed;
    [SerializeField]
    GameObject playerPrefab;

    [Header("Audio")]

    [SerializeField]
    AudioMixer AM;
    [SerializeField]
    AudioClip UIButtonSound;
    [SerializeField]
    AudioClip menuOpenSound;
    [SerializeField]
    AudioClip journalOpenSound;
    [SerializeField]
    AudioClip QuestCompleteSound;
    [SerializeField]
    AudioClip HelicopterSound;
    [SerializeField]
    AudioClip playerSpawnSound;
    [SerializeField]
    AudioClip countDownSound;
    [SerializeField]
    AudioClip trialBeginSound;
    [SerializeField]
    AudioClip trialEndSound_bad;
    [SerializeField]
    AudioClip trialEndSound_good;
    [SerializeField]
    AudioClip finalTenSceondSound;
    [SerializeField]
    AudioClip HeliPowerDown;
    [SerializeField]
    AudioClip HeliPowerUpAndFly;
    [SerializeField]
    AudioClip ArcadeUnlockSound;

    [Header("UISprites")]
    [SerializeField]
    Sprite fullscreenSprite;
    [SerializeField]
    Sprite windowSprite;


    [Header("GameOver")]

    [SerializeField]
    float gameOverTextFadeTime = 3f;
    [SerializeField]
    float gameOverTextHoldTime = 2f;

    [Header("Heli")]

    [SerializeField]
    private float heliVolumeLeave;
    [SerializeField]
    private float heliVolumeEntry;
    [SerializeField]
    private float heliAndScreenFadeTime;
    [SerializeField]
    private float heliLandedFadeTime;

    private float heliFadeTimer;
    private float screenFadeTimer;
    private float helirotorLerpTime;

    private Coroutine c_LoopingHeliSound;

    //Refences
    private AudioSource AS;

    private Transform spawnPoint;
    private GameObject player;

    private GameObject infoCanvas;

    private Transform menuPanel;
    private Button backToMenuButton;
    private Button closeButton;
    private Slider MusicSlider;
    private Slider EffectSlider;
    private Slider SensitivitySlider;
    private Button fullScreenToggleButton;
    private Image fullScreenToggleImage;

    private Transform InteractPrompt;
    private Transform InteractTextBox;
    private TMP_Text InteractText;

    private Transform journalPanel;
    private GameObject[] QuestsGUI;
    private int questCompleted;
    private float fadeCounter = 0f;

    private Transform gameOverPanel;
    private TMP_Text[] gameOverTexts;

    private Transform TrialPanel;
    private TMP_Text trialText;

    private Transform SkyScore;
    private TMP_Text skyScoreText;

    private Transform CrosshairPanel;

    private Transform TrialTimerPanel;
    private TMP_Text trialTimerText;

    private Transform FadePanel;
    private Image fadeImage;
    private Color imageFadeColor;

    private bool isScreenFade;
    private bool isHeliSoundFade;
    private bool isMenuOpen;
    private bool isJournalOpen;
    private bool IsFadeGameOverTextNow;
    private bool isGameEnding;
    private bool isBobbleScore;
    private bool isTrialActive;

    [Header("TESTMODE?")]
    public bool testingMode;

    //SkyScore
    private int panScore;
    private float scoreSpinTimer;

    [Header("SkyScore")]
    [SerializeField]
    private float scoreSpinFlipTime;
    [SerializeField]
    private float scoreSpinDegPerSeconds;
    [SerializeField]
    private float trialActiveTime;

    private Coroutine coro_ActiveTrialCD;
    private Coroutine coro_ActiveTrialTimer;

   //MenuSliders
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string EFFECT_VOLUME = "EffectVolume";
    private const string SENSITIVITY = "Sensitivity";
    private float musicVol;
    private float effectVol;
    private float currentSensitivity;
    private float AS_initialVolume;


    [Header("Particles")]

    [SerializeField]
    private ParticleSystem PS;
    [SerializeField]
    private Material goldenSnowMat;

    //intro fields
    [Header("Intro")]

    [SerializeField]
    GameObject IntroCamera;
    [SerializeField]
    GameObject IntroCanvasPrefab;
    GameObject IntroCanvas;
    [SerializeField]
    AnimationCurve fadeInCurve;
    [SerializeField]
    AnimationCurve fadeOutCurve;

    Image headphoneIcon;
    RectTransform headphoneIMG;
    RectTransform headphoneStart;
    RectTransform headphoneEnd;

    Image cameraIcon;
    RectTransform cameraIMG;
    RectTransform cameraStart;
    RectTransform cameraEnd;

    TMP_Text introText;
    RectTransform introTextLoc;
    RectTransform textStart;
    RectTransform textEnd;

    TMP_Text continueText;

    Image introFadeIcon;

    [SerializeField]
    float fadeEntryTime;
    [SerializeField]
    float fadeExitTime;
    [SerializeField]
    float iconFadeDelay;
    [SerializeField]
    float textFadeDelay;
    [SerializeField]
    float continueEntryDelay;
    [SerializeField]
    private float continueFlashTimePeriod;
    [SerializeField]
    private float introScreenFadeOutTime;

    float headphoneFadeTimer;
    float cameraFadeTimer;
    float textFadeTimer;
    float continueFlashTimer;
    float introScreenFadeOutTimer;
    private float continue_alpha_start = 0.2f;
    private float continue_alpha_end = 1;

    Color iconInitialColor;//0; for both icons 
    Color continue_text_color;
    Color introFadeColor;

    bool isFlashContinue;
    bool isContinuePending;
    bool isIntroActive;

    [SerializeField]
    float introTextTypeSpeed;


    //General/misc gameState
   
    private bool screenState;

    [Header("General fields")]
    [SerializeField]
    float musicReductionTime;
    float musicReductionTimer;

    private bool GUI_State;

    private int breadThrown;

    //ujnity mthods
    private void Awake()
    {
        //other initialisaio
        AS = GetComponent<AudioSource>();
#if UNITY_EDITOR
        //PlayerPrefs.DeleteAll();
#endif

        //UI initialisation
        infoCanvas = Instantiate(InfoCanvasPrefab);
        GUI_State = true;

        InteractPrompt = infoCanvas.transform.Find("InteractPrompt");
        InteractPrompt.gameObject.SetActive(false);
        InteractTextBox = InteractPrompt.transform.Find("InteractText");
        InteractText = InteractTextBox.GetComponent<TMP_Text>();

        menuPanel = infoCanvas.transform.Find("MenuPanel");
        backToMenuButton = menuPanel.Find("BackToMenuButton").GetComponent<Button>();
        backToMenuButton.onClick.AddListener(BackToMainMenu);
        backToMenuButton.onClick.AddListener(PlayButtonSound);
        closeButton = menuPanel.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => ForceCloseMenu());
        closeButton.onClick.AddListener(PlayButtonSound);
        fullScreenToggleButton = menuPanel.Find("FullScreenButton").GetComponent<Button>();
        fullScreenToggleButton.onClick.AddListener(() => ToggleFullScreen());
        fullScreenToggleImage = fullScreenToggleButton.transform.GetChild(0).GetComponent<Image>();
        fullScreenToggleImage.sprite = Screen.fullScreen ? windowSprite : fullscreenSprite;


        MusicSlider = menuPanel.Find("MusicSlider").GetComponent<Slider>();
        MusicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        EffectSlider = menuPanel.Find("EffectSlider").GetComponent<Slider>();
        EffectSlider.onValueChanged.AddListener(OnEffectVolumeChange);
        SensitivitySlider = menuPanel.Find("SensitivitySlider").GetComponent<Slider>();
        SensitivitySlider.onValueChanged.AddListener(OnSensitivityChange);

        journalPanel = infoCanvas.transform.Find("JournalPanel");
        QuestsGUI = GameObject.FindGameObjectsWithTag("Quest");
        foreach (GameObject questGUI in QuestsGUI)
        {
            questGUI.GetComponentInChildren<Image>().enabled = false;
        }
        ForceCloseJournal();

        gameOverPanel = infoCanvas.transform.Find("GameOverPanel");
        gameOverTexts = new TMP_Text[gameOverPanel.childCount];
        for (int i = 0; i < gameOverPanel.childCount; i++)
        {
            gameOverTexts[i] = gameOverPanel.GetChild(i).GetComponent<TMP_Text>();
        }
        gameOverPanel.gameObject.SetActive(false);

        TrialPanel = infoCanvas.transform.Find("TrialPanel");
        TrialPanel.gameObject.SetActive(false);
        trialText = TrialPanel.GetComponentInChildren<TMP_Text>();

        SkyScore = GameObject.Find("PanScoreText").transform;
        skyScoreText = SkyScore.GetComponent<TMP_Text>();
        SkyScore.gameObject.SetActive(false);

        CrosshairPanel = infoCanvas.transform.Find("CrosshairPanel");
        CrosshairPanel.gameObject.SetActive(false);

        TrialTimerPanel = infoCanvas.transform.Find("TrialTimerPanel");
        trialTimerText = TrialTimerPanel.GetComponentInChildren<TMP_Text>();
        TrialTimerPanel.gameObject.SetActive(false);

        FadePanel = infoCanvas.transform.Find("FadePanel");
        FadePanel.gameObject.SetActive(true);
        fadeImage = FadePanel.GetComponentInChildren<Image>();
        imageFadeColor = fadeImage.color;

        panScore = 0;

        isHeliSoundFade = false;
        isScreenFade = false;
        isGameEnding = false;

        //data persisting ( volume, sens)

        musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1);
        MusicSlider.value = musicVol;

        effectVol = PlayerPrefs.GetFloat(EFFECT_VOLUME, 1);
        EffectSlider.value = effectVol;

        currentSensitivity = PlayerPrefs.GetFloat(SENSITIVITY, 1.5f);
        SensitivitySlider.value = currentSensitivity;

        screenState = PlayerPrefs.GetInt(TAE.FULLSCREEN, 1) == 1 ? true : false;


        //other references initialission
        helirotorLerpTime = HeliPowerDown.length;
     


        //INITIALISE INTRO STUFF

        IntroCanvas = Instantiate(IntroCanvasPrefab);
        IntroCamera.SetActive(false);
        IntroCanvas.GetComponent<Canvas>().enabled = false;
        //playerInput.enabled = false;

        headphoneIMG = IntroCanvas.transform.Find("headphoneIMG").GetComponent<RectTransform>();
        headphoneIcon = headphoneIMG.GetComponent<Image>();
        headphoneStart = IntroCanvas.transform.Find("HeadphoneStart").GetComponent<RectTransform>();
        headphoneEnd = IntroCanvas.transform.Find("HeadphoneEnd").GetComponent<RectTransform>();
        headphoneIMG.localPosition = headphoneStart.localPosition;

        cameraIMG = IntroCanvas.transform.Find("cameraIMG").GetComponent<RectTransform>();
        cameraIcon = cameraIMG.GetComponent<Image>();
        cameraStart = IntroCanvas.transform.Find("CameraStart").GetComponent<RectTransform>();
        cameraEnd = IntroCanvas.transform.Find("CameraEnd").GetComponent<RectTransform>();
        cameraIMG.localPosition = cameraStart.localPosition;

        iconInitialColor = cameraIcon.color;
        iconInitialColor.a = 0;
        headphoneIcon.color = iconInitialColor;
        cameraIcon.color = iconInitialColor;

        headphoneFadeTimer = 0f;
        cameraFadeTimer = 0f;
        textFadeTimer = 0f;
        introScreenFadeOutTimer = 0;
        continueFlashTimer = 0f;


        introTextLoc = IntroCanvas.transform.Find("IntroText").GetComponent<RectTransform>();
        introText = introTextLoc.GetComponent<TMP_Text>();
        introText.text = "";
        textStart = IntroCanvas.transform.Find("TextStart").GetComponent<RectTransform>();
        textEnd = IntroCanvas.transform.Find("TextEnd").GetComponent<RectTransform>();

        continueText = IntroCanvas.transform.Find("continueText").GetComponent<TMP_Text>();
        continue_text_color = continueText.color;
        continueText.gameObject.SetActive(false);

        introFadeIcon = IntroCanvas.transform.Find("IntroFade").GetComponent<Image>();
        introFadeColor = introFadeIcon.color;

        isFlashContinue = false;
        isContinuePending = false;
        isIntroActive = false;




    }
    private void OnEnable()
    {
        SkiLift.TriggerPrompt += CreateInteractionPrompt;
        BreadLauncher.TriggerPrompt += CreateInteractionPrompt;
        Piano.TriggerPrompt += CreateInteractionPrompt;
        CinematicChair.TriggerPrompt += CreateInteractionPrompt;
        AlcoholStation.TriggerPrompt += CreateInteractionPrompt;
        Dumpster.TriggerPrompt += CreateInteractionPrompt;
        PowerBox.TriggerPrompt += CreateInteractionPrompt;

        StarterAssetsInputs.ToggleJournal += ToggleJournal;
        StarterAssetsInputs.ToggleMenu += ToggleMenu;
        Quests.QuestCompleted += UpdateJournal;

        TargetPractice.StartTrial += OnTriggerTrialStart;
        TargetPractice.EndTrial += OnTriggerTrialEnd;
        TargetPractice.ForceSkyScoreOff += OnForceSkyScoreOff;

        TargetPan.PanHit += OnPanHit;

        Helicopter.TriggerPrompt += CreateInteractionPrompt;
        Helicopter.HeliStateTriggerGM += OnHeliStateTriggerGM;

        StarterAssetsInputs.Continue += OnContinue;
        //StarterAssetsInputs.ForceWin += OnForceWin;
        StarterAssetsInputs.ToggleGUI += OnToggleGUI;
        StarterAssetsInputs.CinematicFadeOut += OnCinematicFadeOut;

        BreadLauncher.BreadThrown += OnBreadThrown;
    }
    private void OnDisable()
    {
        SkiLift.TriggerPrompt -= CreateInteractionPrompt;
        BreadLauncher.TriggerPrompt -= CreateInteractionPrompt;
        Piano.TriggerPrompt -= CreateInteractionPrompt;
        CinematicChair.TriggerPrompt -= CreateInteractionPrompt;
        AlcoholStation.TriggerPrompt -= CreateInteractionPrompt;
        Dumpster.TriggerPrompt -= CreateInteractionPrompt;
        PowerBox.TriggerPrompt -= CreateInteractionPrompt;

        StarterAssetsInputs.ToggleJournal -= ToggleJournal;
        StarterAssetsInputs.ToggleMenu -= ToggleMenu;
        Quests.QuestCompleted -= UpdateJournal;

        TargetPractice.StartTrial -= OnTriggerTrialStart;
        TargetPractice.EndTrial -= OnTriggerTrialEnd;
        TargetPractice.ForceSkyScoreOff -= OnForceSkyScoreOff;

        TargetPan.PanHit -= OnPanHit;

        Helicopter.TriggerPrompt -= CreateInteractionPrompt;
        Helicopter.HeliStateTriggerGM -= OnHeliStateTriggerGM;

        StarterAssetsInputs.Continue -= OnContinue;
       //StarterAssetsInputs.ForceWin -= OnForceWin;
        StarterAssetsInputs.ToggleGUI -= OnToggleGUI;
        StarterAssetsInputs.CinematicFadeOut -= OnCinematicFadeOut;

        BreadLauncher.BreadThrown -= OnBreadThrown;
    }
    private void Start()
    {
        //need to call at start because some how cursor states are notset correctly if call in awake
        //ForceCloseMenu();
        TestingMode?.Invoke(testingMode);

        Cursor.lockState =  CursorLockMode.Locked;
        Cursor.visible = false;

        SetMixerVolume(MUSIC_VOLUME, musicVol);
        SetMixerVolume(EFFECT_VOLUME, effectVol);

        heliFadeTimer = 0f;
        screenFadeTimer = 0f;

        if (testingMode)
        {
            StartGame();
        }
        else
        {
            EnableIntroStuff();
        }
     

    }
    private void Update()
    {
        if (isScreenFade)
        {
            if (isGameEnding)
            {
                LerpBlackImage(0, 1, Time.deltaTime);
            }
            else
            {
                LerpBlackImage(1, 0, Time.deltaTime);
            }
        }

        if (isHeliSoundFade)
        {
            if (isGameEnding)
            {
                LerpHeliSoundDown(AS_initialVolume, 0f, Time.deltaTime, heliAndScreenFadeTime);
            }
            else
            {
                LerpHeliSoundDown(AS_initialVolume, 0f, Time.deltaTime, heliLandedFadeTime);
            }
        }



        if (IsFadeGameOverTextNow)
        {
            FadeGameOverText(Time.deltaTime);
        }

        if (isBobbleScore)
        {
            BobbleScoreText(Time.deltaTime);
        }

    }

    //event subs
    private void ToggleJournal()
    {
        if (isMenuOpen)
            return;
        if (isIntroActive)
            return;

        if (isJournalOpen == false)
        {
            PlaySound(journalOpenSound, 0.5f);
        }

        isJournalOpen = !isJournalOpen;
        journalPanel.gameObject.SetActive(isJournalOpen);
    }
    private void UpdateJournal(QuestTypes qt)
    {
        foreach (GameObject questGUI in QuestsGUI)
        {
            if (qt == questGUI.GetComponent<QuestGUISort>().questType)
            {
                questGUI.GetComponentInChildren<Image>().enabled = true;
                PlaySound(QuestCompleteSound, 1f);
                questCompleted++;
                break;
            }
        }

        if (questCompleted >= QuestsGUI.Length)
        {
            PlayerReadyToLeave?.Invoke();
            EnableArcade?.Invoke();
            DisplayGameOverPanel();
            PlaySound(ArcadeUnlockSound, 0.5f, ArcadeUnlockSound.length);
            PS.GetComponent<ParticleSystemRenderer>().material = goldenSnowMat;

            StartCoroutine(TempReduceMusic());
        }
    }
    private void ToggleMenu()
    {
        if (isIntroActive)
            return;

        ForceCloseJournal();

        if (isMenuOpen == false)
        {
            PlaySound(menuOpenSound, 0.25f); ;
        }

        isMenuOpen = !isMenuOpen;

        SetCursorAndLookState?.Invoke(!isMenuOpen);
        menuPanel.gameObject.SetActive(isMenuOpen);
    }
    private void OnTriggerTrialStart()
    {
        isTrialActive = true;
        //begin countdown of 3 2 1 
        TrialPanel.gameObject.SetActive(true);
        CrosshairPanel.gameObject.SetActive(true);
        SkyScore.gameObject.SetActive(true);
        SetSkyScoreState(true);

        coro_ActiveTrialCD = StartCoroutine(HoldForTrialCountDown());

        panScore = 0;
    }
    private void OnTriggerTrialEnd()
    {
        SetSkyScoreState(false);
        EndTrialDefault();
        PlaySound(trialEndSound_bad, 0.1f);
    }
    private void OnPanHit()
    {
        if (!isTrialActive)
            return;

        panScore++;
        if (panScore ==40)
        {
            Achievement_MaxScore?.Invoke();
        }
        skyScoreText.text = panScore.ToString("00.##");
    }
    private void OnForceSkyScoreOff()
    {
        SetSkyScoreState(false);
    }
    private void OnHeliStateTriggerGM(bool b)
    {
        if (b)
        {//leaving
            AS.Stop();
            AS.volume = heliVolumeLeave;
            AS_initialVolume = heliVolumeLeave;
            AS.clip = HeliPowerUpAndFly;
            AS.spatialBlend = 0;
            AS.Play();

            heliFadeTimer = 0f;
            screenFadeTimer = 0f;
            isGameEnding = true;
            StartCoroutine(HoldForHeliPowerUPThenMoveHeli());

            LerpHeliRotor?.Invoke(isGameEnding, helirotorLerpTime);
        }
        else
        {//landing
            PlaySound(playerSpawnSound, 0.5f);
            PlaySound(HeliPowerDown, 0.2f, HeliPowerDown.length);
            isGameEnding = false;
            isHeliSoundFade = true;
            heliFadeTimer = 0f;

            LerpHeliRotor?.Invoke(isGameEnding, helirotorLerpTime);
        }


    }
    private void OnContinue()
    {
        if (!isContinuePending)
            return;

        //Debug.Log("continuing!");

        isContinuePending = false;

        StartCoroutine(FadeOutEntryIcons(fadeOutCurve, fadeExitTime));
        StartCoroutine(FadeOutIntroScreen());
    }
    private void OnForceWin()
    {
        StartCoroutine(TempReduceMusic());
    }
    private void OnToggleGUI()
    {

        GUI_State = !GUI_State;
        infoCanvas.GetComponent<Canvas>().enabled = GUI_State;
    }

    private void OnCinematicFadeOut()
    {
        StartCoroutine(ForceFadeOut());
    }
    private void OnBreadThrown()
    {
        breadThrown++;
    }



    //private mehods
    private void ForceCloseMenu()
    {
        isMenuOpen = false;
        SetCursorAndLookState?.Invoke(!isMenuOpen);
        menuPanel.gameObject.SetActive(false);
    }
    private void ForceCloseJournal()
    {
        isJournalOpen = false;
        SetCursorAndLookState?.Invoke(!isJournalOpen);
        journalPanel.gameObject.SetActive(false);
    }
    private void EndTrialLegit()
    {
        EndTrialDefault();
        PlaySound(trialEndSound_good, 0.4f);
        SkyScore.gameObject.SetActive(true);
    }
    private void EndTrialDefault()
    {
        isTrialActive = false;

        if (coro_ActiveTrialCD != null)
        {
            StopCoroutine(coro_ActiveTrialCD);
        }
        if (coro_ActiveTrialTimer != null)
        {
            StopCoroutine(coro_ActiveTrialTimer);
        }

        TrialPanel.gameObject.SetActive(false);
        CrosshairPanel.gameObject.SetActive(false);
        TrialTimerPanel.gameObject.SetActive(false);
    }
    private void SetSkyScoreState(bool state)
    {
        if (state)
        {
            SkyScore.gameObject.SetActive(true);
            panScore = 0;
            skyScoreText.text = panScore.ToString("00.##");
            isBobbleScore = true;
        }
        else
        {
            SkyScore.gameObject.SetActive(false);
            isBobbleScore = false;
        }
    }
    private void StartGame()
    {
  
        //player initialistion

        if (!testingMode)
        {
            spawnPoint = GameObject.Find("SpawnPoint").transform;
            player = Instantiate(playerPrefab, spawnPoint);
            player.transform.localPosition = Vector3.zero;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        //DO THESE AFTER PLAYER HAS BEEN SPAWN OR ELSE LISTENERS WONT SUBSCRIBE ON TIME! 
        //need to trigger locking cursor and keep it invisible  manually when play is spawner
        ForceCloseMenu();
        BeginGame?.Invoke();

        isIntroActive = false;
        isScreenFade = true;
        if (!testingMode)
        {
            c_LoopingHeliSound = StartCoroutine(LoopEntryHeliSound());
        }

        PlayerPrefs.SetInt(TAE.ISEVERPLAYED, 1);
        Achievement_EnterGameScene?.Invoke();
    }
    private void EnableIntroStuff()
    {
        isIntroActive = true;
        IntroCamera.SetActive(true);
        IntroCanvas.GetComponent<Canvas>().enabled = true;
        //playerInput.enabled = true;
        StartCoroutine(FadeInEntryIcons(fadeInCurve, fadeEntryTime));

    }


    //untilitis
    private void PlayButtonSound()
    {
        PlaySound(UIButtonSound, 0.25f);
    }
    private void PlaySound(AudioClip ac, float volume)
    {
        GameObject audio = Instantiate(AudioPrefab);
        AudioSource AS = audio.GetComponent<AudioSource>();

        AS.volume = volume;
        AS.clip = ac;
        AS.spatialBlend = 0;

        AS.Play();
    }
    private void PlaySound(AudioClip ac, float volume, float duration)
    {
        GameObject audio = Instantiate(AudioPrefab_Timed);
        AudioSource AS = audio.GetComponent<AudioSource>();
        audio.GetComponent<MenuAS>().timer = duration;

        AS.volume = volume;
        AS.clip = ac;
        AS.spatialBlend = 0;

        AS.Play();
    }
    private void CreateInteractionPrompt(bool isActiveatePrompt, string s)
    {
        InteractPrompt.gameObject.SetActive(isActiveatePrompt);
        InteractText.text = s;
    }
    private void FadeGameOverText(float dt)
    {

        if (fadeCounter > gameOverTextFadeTime)
        {
            IsFadeGameOverTextNow = false;

            gameOverPanel.gameObject.SetActive(false);
        }

        fadeCounter += dt;

        for (int i = 0; i < gameOverTexts.Length; i++)
        {
            gameOverTexts[i].alpha = Mathf.Lerp(1f, 0f, (fadeCounter / gameOverTextFadeTime));
        }
    }
    private void DisplayGameOverPanel()
    {
        gameOverPanel.gameObject.SetActive(true);
        StartCoroutine(HoldThenFadeGameOverText());
    }
    private void LerpBlackImage(float alphaStart, float alphaEnd, float dt)
    {
        if (screenFadeTimer > heliAndScreenFadeTime)
        {
            isScreenFade = false;
            screenFadeTimer = 0f;
            return;
        }

        screenFadeTimer += dt;
        imageFadeColor.a = Mathf.Lerp(alphaStart, alphaEnd, screenFadeTimer / heliAndScreenFadeTime);
        fadeImage.color = imageFadeColor;

    }
    private void LerpHeliSoundDown(float volStart, float volEnd, float dt, float fadeTime)
    {
        if (heliFadeTimer > fadeTime)
        {//landing

            isHeliSoundFade = false;
            AS.Stop();
            AS.clip = null;
            heliFadeTimer = 0f;

            if (c_LoopingHeliSound != null)
            {
                StopCoroutine(c_LoopingHeliSound);
                c_LoopingHeliSound = null;
            }

            return;
        }

        heliFadeTimer += dt;
        AS.volume = Mathf.Lerp(volStart, volEnd, heliFadeTimer / fadeTime);

    }
    private void BobbleScoreText(float dt)
    {
        scoreSpinTimer += dt;

        if (scoreSpinTimer >= scoreSpinFlipTime)
        {
            scoreSpinDegPerSeconds *= -1f;
            scoreSpinTimer = 0;
        }

        SkyScore.RotateAround(SkyScore.transform.position, SkyScore.transform.right, scoreSpinDegPerSeconds * dt);
    }
    private void SetMixerVolume(string MixerGroup, float intensity)
    {
        AM.SetFloat(MixerGroup, Mathf.Log10(intensity) * 20);
    }


    //ui button clicks
    private void BackToMainMenu()
    {
        GameEnd?.Invoke(breadThrown);
        SceneManager.LoadScene("Menu");
    }
    private void OnMusicVolumeChange(float targeVol)
    {

        musicVol = targeVol;
        SetMixerVolume(MUSIC_VOLUME, musicVol);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVol);


    }
    private void OnEffectVolumeChange(float targeVol)
    {

        effectVol = targeVol;
        SetMixerVolume(EFFECT_VOLUME, effectVol);
        PlayerPrefs.SetFloat(EFFECT_VOLUME, effectVol);

    }
    private void OnSensitivityChange(float targetSens)
    {
        currentSensitivity = targetSens;
        UserSensitivityChange?.Invoke(currentSensitivity);
        PlayerPrefs.SetFloat(SENSITIVITY, currentSensitivity);
    }
    private void ToggleFullScreen()
    {
        screenState = !screenState;

        if (screenState)
        {
            StartCoroutine(EnableFullScreen());
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        fullScreenToggleImage.sprite = screenState ? windowSprite : fullscreenSprite;

        int n = screenState ? 1 : 0;
        PlayerPrefs.SetInt(TAE.FULLSCREEN, n);

    }

    //corotines
    IEnumerator HoldThenFadeGameOverText()
    {
        yield return new WaitForSeconds(gameOverTextHoldTime);

        IsFadeGameOverTextNow = true;
    }
    IEnumerator HoldThenSceneChange()
    {
        yield return new WaitForSeconds(heliAndScreenFadeTime);
        BackToMainMenu();
    }
    IEnumerator HoldForTrialCountDown()
    {
        int cd = 3;
        trialText.text = "Stay behind the table!";

        while (true)
        {
            skyScoreText.text = cd.ToString();
            PlaySound(countDownSound, 0.2f);

            yield return new WaitForSeconds(1);
            cd--;
            if (cd == 0)
            {
                break;
            }

        }
        skyScoreText.text = "GO!";

        PlaySound(trialBeginSound, 0.2f);

        SetPanThrowState?.Invoke(trialActiveTime);
        coro_ActiveTrialTimer = StartCoroutine(HoldForTrialTimer());
    }
    IEnumerator HoldForTrialTimer()
    {
        int timer = (int)trialActiveTime;
        TrialTimerPanel.gameObject.SetActive(true);

        while (true)
        {
            trialTimerText.text = timer.ToString();
            if (timer <= 10)
            {
                PlaySound(finalTenSceondSound, 0.1f);
            }

            yield return new WaitForSeconds(1);
            timer--;


            if (timer <= 0)
            {
                break;
            }

        }

        isTrialActive = false;
        TrialLegitEnd?.Invoke();
        skyScoreText.text = "Nice Job!" + "\n" + panScore;
        EndTrialLegit();
    }
    IEnumerator HoldForHeliMoveThenFadeScreen()
    {
        yield return new WaitForSeconds(3);
        isScreenFade = true;
        isHeliSoundFade = true;
        StartCoroutine(HoldThenSceneChange());
    }
    IEnumerator HoldForHeliPowerUPThenMoveHeli()
    {
        yield return new WaitForSeconds(4f);

        ExtractStartMoveHeli?.Invoke();
        StartCoroutine(HoldForHeliMoveThenFadeScreen());

    }
    IEnumerator LoopEntryHeliSound()
    {

        AS.Stop();

        AS.volume = heliVolumeEntry;
        AS_initialVolume = heliVolumeEntry;
        AS.clip = HelicopterSound;
        AS.spatialBlend = 0;

        WaitForSeconds refreshIntervalWait = new WaitForSeconds(HelicopterSound.length);

        while (true)
        {
            AS.Play();
            //Debug.Log("play music" + ac.name);
            yield return refreshIntervalWait;

        }
    }
    IEnumerator FadeInEntryIcons(AnimationCurve fadeInCurve, float fadeInTime)
    {
        StartCoroutine(FadeHeadphone(headphoneStart, headphoneEnd, fadeInCurve, fadeInTime, false));
        yield return new WaitForSeconds(iconFadeDelay);
        StartCoroutine(FadeCamera(cameraStart, cameraEnd, fadeInCurve, fadeInTime, false));
        yield return new WaitForSeconds(textFadeDelay);
        StartCoroutine(TypeWrite(new ActiveTypingText(introText, "Headphones and first person mode are recommended for optimal experience.", introTextTypeSpeed)));

        // StartCoroutine(FadeText(textStart, textEnd, fadeInCurve, fadeInTime));
        yield return new WaitForSeconds(continueEntryDelay);
        StartCoroutine(FlashButton());

    }
    IEnumerator FadeOutEntryIcons(AnimationCurve fadeOutCurve, float fadeOutTime)
    {
        StartCoroutine(FadeHeadphone(headphoneEnd, headphoneStart, fadeOutCurve, fadeOutTime, true));
        //yield return new WaitForSeconds(iconFadeDelay);
        StartCoroutine(FadeCamera(cameraEnd, cameraStart, fadeOutCurve, fadeOutTime, true));
        //yield return new WaitForSeconds(textFadeDelay);
        StartCoroutine(FadeText(textEnd, textStart, fadeOutCurve, fadeOutTime));
        yield return null;

    }
    IEnumerator FadeHeadphone(RectTransform start, RectTransform fin, AnimationCurve fadeCurve, float fadeTime, bool fadePosOnly)
    {
        while (headphoneFadeTimer < fadeTime)
        {
            headphoneFadeTimer += Time.deltaTime;
            iconInitialColor.a = Mathf.Lerp(0, 1, headphoneFadeTimer / fadeTime);

            if (!fadePosOnly)
            {
                headphoneIcon.color = iconInitialColor;
            }
            headphoneIMG.localPosition = Vector2.Lerp(start.localPosition, fin.localPosition, fadeCurve.Evaluate(headphoneFadeTimer / fadeTime));

            yield return null;
        }

        headphoneFadeTimer = 0;

    }
    IEnumerator FadeCamera(RectTransform start, RectTransform fin, AnimationCurve fadeCurve, float fadeTime, bool fadePosOnly)
    {
        while (cameraFadeTimer < fadeTime)
        {
            cameraFadeTimer += Time.deltaTime;
            iconInitialColor.a = Mathf.Lerp(0, 1, cameraFadeTimer / fadeTime);

            if (!fadePosOnly)
            {
                cameraIcon.color = iconInitialColor;
            }
            cameraIMG.localPosition = Vector2.Lerp(start.localPosition, fin.localPosition, fadeCurve.Evaluate(cameraFadeTimer / fadeTime));

            yield return null;
        }

        cameraFadeTimer = 0f;

    }
    IEnumerator FadeText(RectTransform start, RectTransform fin, AnimationCurve fadeCurve, float fadeTime)
    {
        textFadeTimer = 0f;

        while (textFadeTimer < fadeTime)
        {
            textFadeTimer += Time.deltaTime;

            introTextLoc.localPosition = Vector2.Lerp(start.localPosition, fin.localPosition, fadeCurve.Evaluate(textFadeTimer / fadeTime));

            yield return null;
        }

        textFadeTimer = 0f;
    }
    IEnumerator FlashButton()
    {
        continueText.gameObject.SetActive(true);
        isFlashContinue = true;
        isContinuePending = true;
        while (isFlashContinue)
        {
            continueFlashTimer += Time.deltaTime;
            if (continueFlashTimer > continueFlashTimePeriod)
            {
                continueFlashTimer = 0f;
                float temp = continue_alpha_start;
                continue_alpha_start = continue_alpha_end;
                continue_alpha_end = temp;
            }

            continue_text_color.a = Mathf.Lerp(continue_alpha_start, continue_alpha_end, continueFlashTimer / continueFlashTimePeriod);
            continueText.color = continue_text_color;


            yield return null;
        }

        continueFlashTimer = 0f;
    }
    IEnumerator FadeOutIntroScreen()
    {
        while (introScreenFadeOutTimer < introScreenFadeOutTime)
        {
            introScreenFadeOutTimer += Time.deltaTime;
            introFadeColor.a = Mathf.Lerp(0, 1, introScreenFadeOutTimer / introScreenFadeOutTime);
            introFadeIcon.color = introFadeColor;
            yield return null;
        }

        introScreenFadeOutTimer = 0f;
        isFlashContinue = false;
        yield return new WaitForSeconds(1f);

        //playerInput.enabled = false;
        IntroCanvas.SetActive(false);
        IntroCamera.SetActive(false);

        StartGame();
    }
    IEnumerator TypeWrite(ActiveTypingText activeTypingText)
    {
        while (activeTypingText.tmpT != null)
        {
            activeTypingText.UpdateText();
            yield return null;
        }

       
    }
    IEnumerator EnableFullScreen()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        yield return new WaitForSeconds(60 * Time.deltaTime);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    IEnumerator TempReduceMusic()
    {
        float originalVol = musicVol;
        float targetVol = 0.25f;

        musicReductionTimer = 0;
        while (musicReductionTimer < musicReductionTime)
        {
            musicReductionTimer += Time.deltaTime;

            musicVol = Mathf.Lerp(originalVol, targetVol, musicReductionTimer / musicReductionTime);
            SetMixerVolume(MUSIC_VOLUME, musicVol);
            yield return null;
        }

        yield return new WaitForSeconds(ArcadeUnlockSound.length - 2*musicReductionTime -6f);

        musicReductionTimer = 0;

        while (musicReductionTimer < musicReductionTime)
        {
            musicReductionTimer += Time.deltaTime;

            musicVol = Mathf.Lerp(targetVol, originalVol, musicReductionTimer / musicReductionTime);
            SetMixerVolume(MUSIC_VOLUME, musicVol);
            yield return null;
        }

        musicReductionTimer = 0;

    }
    IEnumerator ForceFadeOut()
    {
        float fadeTimer = 0;
        float fadeTime = 1f;


        while (fadeTimer <= fadeTime+Time.deltaTime)
        {

            imageFadeColor.a = Mathf.Lerp(0, 1, fadeTimer / fadeTime);
            fadeImage.color = imageFadeColor;

            fadeTimer += Time.deltaTime;
            yield return null;

        }


    }


}






