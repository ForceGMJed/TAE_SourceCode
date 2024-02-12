using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject MenuCanvasPrefab;
    [SerializeField]
    AudioClip menuMusic;
    [SerializeField]
    AudioClip pilotClip;
    [SerializeField]
    AudioClip seatBeltSound;

    [SerializeField]
    GameObject AudioPrefab;
    [SerializeField]
    AudioMixer AM;
    [SerializeField]
    AnimationCurve animationCurve_easeInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


    //UI REFERENCES
    GameObject MenuCanvasGo;
    Button playButton;
    Button quitButton;
    Button skipButton;

    TMP_Text PA_Text;
    TMP_Text Destination_Text;
    TMP_Text ETA_Text;
    TMP_Text EXIT_Text;
    TMP_Text TITLE_Text;

    Slider menuMusicSlider;

    [SerializeField]
    private float titleFadeTime;
    private float titleFadeTimer;
    private Color titleColor;

    private float PA_FlashTimer;
    [SerializeField]
    private float PA_FlashPeriod;
    private float PA_alpha_start = 0;
    private float PA_alpha_end = 1;
    private Color PA_text_color;

    private float EXIT_FlashTimer;
    [SerializeField]
    private float EXIT_FlashPeriod;
    private float EXIT_alpha_start = 0.2f;
    private float EXIT_alpha_end = 0.6f;
    private Color EXIT_text_color;

    [SerializeField]
    private float PlayButtonMaxMoveRange;
    [SerializeField]
    private float playButtonMoveFlipTime;
    private float playButtonMoveTimer;
    private float playButtonInitial_X_Pos;
    private float playButtonInitial_Y_Pos;
    private float playButton_start;
    private float playButton_end;
    private RectTransform playButtonRect;
    private Color playColor;
    private TMP_Text playText;

    [SerializeField]
    private float skipButtonFlashTime;
    private Color skipColor;
    private TMP_Text skipText;
    private float skip_alpha_start = 0;
    private float skip_alpha_end = 1;


    private AudioSource AS;
    private Coroutine c_menuMusic;
    private Image fadeImage;
    private Color imageFadeColor;
    private float loadFadeTimer;
    [SerializeField]
    private float LoadFadeTime;
    private bool isLoadFade;
    private bool isPA;
    private bool isEnding;
    private float menuVol;
    private const string EFFECT_VOLUME = "EffectVolume";
    private bool isFadeTitleAndPlay;
    private bool isPlayPending;

    private List<ActiveTypingText> activeTypingTexts;

    private AsyncOperation swissAlpsScene;

    [SerializeField]
    private float timeDelayTilFirstText;
    [SerializeField]
    private float timeDelayTilSecondText;


    private Button FullScreenButton;
    [SerializeField]
    Sprite fullscreen;
    [SerializeField]
    Sprite window;
    bool screenState;

    [SerializeField]
    private bool isTestingMode;



    private void Awake()
    {
#if UNITY_EDITOR
        if (isTestingMode)
        {
            PlayerPrefs.DeleteAll();
        }  
#endif


        AS = GetComponent<AudioSource>();
        AS.volume = 0.3f;

        swissAlpsScene = null;

        MenuCanvasGo = Instantiate(MenuCanvasPrefab);
        TITLE_Text = MenuCanvasGo.transform.Find("Title").GetComponent<TMP_Text>();
        titleColor = TITLE_Text.color;

        playButton = MenuCanvasGo.transform.Find("PlayButton").GetComponent<Button>();
        playButton.onClick.AddListener(OnPlayButtonClicked);
        playButtonRect = playButton.GetComponent<RectTransform>();
        playButtonInitial_X_Pos = playButtonRect.anchoredPosition.x;
        playButtonInitial_Y_Pos = playButtonRect.anchoredPosition.y;
        playButton_start = playButtonInitial_Y_Pos;
        playButton_end = playButtonInitial_Y_Pos - PlayButtonMaxMoveRange;
        playText = playButton.GetComponentInChildren<TMP_Text>();
        playColor = playText.color;

        quitButton = MenuCanvasGo.transform.Find("QuitButton").GetComponent<Button>();
        quitButton.onClick.AddListener(OnQuitGame);

        PA_Text = MenuCanvasGo.transform.Find("PA_Announcement").GetComponent<TMP_Text>();
        PA_text_color = PA_Text.color;
        PA_text_color.a = 0f;
        PA_Text.color = PA_text_color;
        isPA = false;

        Destination_Text = MenuCanvasGo.transform.Find("Destination_Text").GetComponent<TMP_Text>();
        Destination_Text.text = null;

        ETA_Text = MenuCanvasGo.transform.Find("ETA_Text").GetComponent<TMP_Text>();
        ETA_Text.text = null;

        PA_Text.gameObject.SetActive(false);
        Destination_Text.gameObject.SetActive(false);
        ETA_Text.gameObject.SetActive(false);

        fadeImage = MenuCanvasGo.transform.Find("BGFade").GetComponent<Image>();
        imageFadeColor = fadeImage.color;
        imageFadeColor.a = 0;
        fadeImage.color = imageFadeColor;
        loadFadeTimer = 0f;
        isLoadFade = false;
        fadeImage.gameObject.SetActive(false);

        EXIT_Text = MenuCanvasGo.transform.Find("QuitButton").GetComponentInChildren<TMP_Text>();
        EXIT_text_color = EXIT_Text.color;
        isEnding = false;

        menuMusicSlider = MenuCanvasGo.transform.Find("VolumeSlider").GetComponent<Slider>();
        menuMusicSlider.onValueChanged.AddListener(OnMenuVolumeChange);


        FullScreenButton = MenuCanvasGo.transform.Find("FullScreenButton").GetComponent<Button>();
        FullScreenButton.onClick.AddListener(OnFullScreenButton);
        int n = PlayerPrefs.GetInt("FullScreen", 1);
        screenState = n == 1 ? true : false;
        FullScreenButton.GetComponent<Image>().sprite = screenState ? window : fullscreen;

        skipButton = MenuCanvasGo.transform.Find("SkipButton").GetComponent<Button>();
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        skipText = skipButton.GetComponentInChildren<TMP_Text>();
        skipColor = skipText.color;
        skipButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        activeTypingTexts = new List<ActiveTypingText>();

        isPlayPending = false;

        Application.backgroundLoadingPriority = ThreadPriority.Normal;

    }


    private void Start()
    {

        menuVol = PlayerPrefs.GetFloat(EFFECT_VOLUME, 1);
        menuMusicSlider.value = menuVol;
        SetMixerVolume(EFFECT_VOLUME, menuVol);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {

        LerpPlayButton(Time.deltaTime);

        if (isLoadFade)
        {
            LerpBGImage(0, 1, Time.deltaTime);
        }
        if (isFadeTitleAndPlay)
        {
            LerpTitleAndPlayText(Time.deltaTime);
        }

        if (isPA)
        {
            PA_FlashText(Time.deltaTime);
            for (int i = 0; i < activeTypingTexts.Count; i++)
            {
                if (activeTypingTexts[i].tmpT != null)
                {
                    activeTypingTexts[i].UpdateText();
                }
                else
                {
                    activeTypingTexts.RemoveAt(i--);
                }
            }

        }
        if (isEnding)
        {
            EXIT_FlashText(Time.deltaTime);
        }

    }


    //corountiunes
    private IEnumerator LoopMusic(AudioClip ac)
    {

        AS.Stop();
        AS.clip = ac;

        WaitForSeconds refreshIntervalWait = new WaitForSeconds(ac.length);

        while (true)
        {
            AS.Play();
            //Debug.Log("play music" + ac.name);
            yield return refreshIntervalWait;

        }
    }
    private IEnumerator HoldForPilotAnnounceAndFadeThenLoadScene()
    {
        yield return new WaitForSeconds(pilotClip.length + LoadFadeTime);
        swissAlpsScene.allowSceneActivation = true;
    }
    private IEnumerator LoadSceneAsync()
    {
        swissAlpsScene = SceneManager.LoadSceneAsync("SwissAlps");

        swissAlpsScene.allowSceneActivation = false;

        //while (swissAlpsScene.progress <0.9f)
        //{
        //    Debug.Log("swissAlpsScene is not done");
        //    yield return null;
        //}

        yield return null;
        StartCoroutine(SkipButtonHold());
       

    }
    private IEnumerator HoldThenStartTypingText()
    {
        yield return new WaitForSeconds(timeDelayTilFirstText);

        activeTypingTexts.Add(new ActiveTypingText(Destination_Text, "Destination - Alpine Heliport, Switzerland.", 0.08f));

        yield return new WaitForSeconds(timeDelayTilSecondText);

        activeTypingTexts.Add(new ActiveTypingText(ETA_Text, "ETA - 1 min.", 0.09f));
    }
    private IEnumerator HoldForQuit()
    {
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
    private IEnumerator HoldThenFade()
    {
        yield return new WaitForSeconds(pilotClip.length - LoadFadeTime);
        fadeImage.gameObject.SetActive(true);
        isLoadFade = true;
    }
    private IEnumerator HoldThenTurnOffMenuMusic()
    {
        yield return new WaitForSeconds(0.5f);
        AS.volume = 0;
        StopCoroutine(c_menuMusic);
    }
    private IEnumerator EnableFullScreen()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        yield return new WaitForSeconds(60 * Time.deltaTime);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    private IEnumerator FlashSkipButton()
    {
        float beginValue = skip_alpha_start;
        float endValue = skip_alpha_end;
        float alternateBeginValue = 0.5f;
        bool firstTimeSwitch = true;

        skipColor.a = skip_alpha_start;
        skipText.color = skipColor;
        skipButton.gameObject.SetActive(true);

        float timer = skipButtonFlashTime;
        while(timer >=0)
        {
            skipColor.a = Mathf.Lerp(beginValue, endValue, animationCurve_easeInOut.Evaluate(1 - Mathf.Clamp(timer, 0, skipButtonFlashTime) / skipButtonFlashTime));
            skipText.color = skipColor;

            timer -= Time.deltaTime;

            if (timer < 0)
            {
                timer = skipButtonFlashTime;

                if (firstTimeSwitch)
                {
                    beginValue = alternateBeginValue;
                    firstTimeSwitch = false;
                }
                
                var temp = beginValue;
                beginValue = endValue;
                endValue = temp;

            }
            yield return null;
        }
        
    }
    private IEnumerator ForceFadeAndThenEnableSceneLoad()
    {
    
        imageFadeColor.a = 0;
        fadeImage.color = imageFadeColor;
        fadeImage.gameObject.SetActive(true);
        float timer = LoadFadeTime;
        while (timer>=0)
        {
            imageFadeColor.a = Mathf.Lerp(0, 1, 1 - timer / LoadFadeTime);
            fadeImage.color = imageFadeColor;

            timer -= Time.deltaTime;

            if (timer < 0)
            {
                swissAlpsScene.allowSceneActivation = true;
                break;
            }
            yield return null;
        }
     
    }
    private IEnumerator SkipButtonHold()
    {
        yield return new WaitForSeconds(3f);
        ShowSkipButton();
    }



    //private mthods
    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {


        if (scene.buildIndex == 0 || scene.name == "Menu")
        {

            if (c_menuMusic != null)
            {
                StopCoroutine(c_menuMusic);
            }

            c_menuMusic = StartCoroutine(LoopMusic(menuMusic));

        }
    }
    private void LoadGameScene()
    {
        PlaySound(seatBeltSound, 0.8f, seatBeltSound.length);
        PlaySound(pilotClip, 0.2f, pilotClip.length);

        StartCoroutine(LoadSceneAsync());

        StartCoroutine(HoldThenTurnOffMenuMusic());

        StartCoroutine(HoldThenStartTypingText());

        StartCoroutine(HoldForPilotAnnounceAndFadeThenLoadScene());
       
        StartCoroutine(HoldThenFade());
    }
    private void PA_FlashText(float dt)
    {
        PA_FlashTimer += dt;
        if (PA_FlashTimer > PA_FlashPeriod)
        {
            float temp = PA_alpha_start;
            PA_alpha_start = PA_alpha_end;
            PA_alpha_end = temp;
            PA_FlashTimer = 0;
        }

        PA_text_color.a = Mathf.Lerp(PA_alpha_start, PA_alpha_end, PA_FlashTimer / PA_FlashPeriod);
        PA_Text.color = PA_text_color;
    }
    private void EXIT_FlashText(float dt)
    {
        EXIT_FlashTimer += dt;
        if (EXIT_FlashTimer > EXIT_FlashPeriod)
        {
            float temp = EXIT_alpha_start;
            EXIT_alpha_start = EXIT_alpha_end;
            EXIT_alpha_end = temp;
            EXIT_FlashTimer = 0;
        }

        EXIT_text_color.a = Mathf.Lerp(EXIT_alpha_start, EXIT_alpha_end, EXIT_FlashTimer / EXIT_FlashPeriod);
        EXIT_Text.color = EXIT_text_color;
    }
    private void OnPlayButtonClicked()
    {
        if (isPlayPending)
            return;

        isPlayPending = true;
        isFadeTitleAndPlay = true;

        LoadGameScene();
    }
    private void LerpPlayButton(float dt)
    {
        playButtonMoveTimer += dt;
        if (playButtonMoveTimer >= playButtonMoveFlipTime)
        {
            playButtonMoveTimer = 0f;
            float temp = playButton_start;
            playButton_start = playButton_end;
            playButton_end = temp;
        }

        playButtonRect.anchoredPosition = new Vector2(playButtonInitial_X_Pos, Mathf.Lerp(playButton_start, playButton_end, animationCurve_easeInOut.Evaluate(playButtonMoveTimer / playButtonMoveFlipTime)));
    }
    private void OnQuitGame()
    {
        EXIT_Text.text = "Emergency Landing";
        isEnding = true;
        StartCoroutine(HoldForQuit());
    }
    private void LerpBGImage(float alphaStart, float alphaEnd, float dt)
    {
        if (loadFadeTimer > LoadFadeTime)
        {
            isLoadFade = false;
            AS.Stop();
            AS.clip = null;
            //loadingStencil.gameObject.SetActive(true);
            swissAlpsScene.allowSceneActivation = true;

        }

        loadFadeTimer += dt;
        imageFadeColor.a = Mathf.Lerp(alphaStart, alphaEnd, loadFadeTimer / LoadFadeTime);
        fadeImage.color = imageFadeColor;

        // Debug.Log(imageFadeColor.a);
    }
    private void LerpTitleAndPlayText(float dt)
    {
        if (titleFadeTimer > titleFadeTime)
        {
            TITLE_Text.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            PA_Text.gameObject.SetActive(true);
            Destination_Text.gameObject.SetActive(true);
            ETA_Text.gameObject.SetActive(true);

            isPA = true;
            return;
        }
            

        titleFadeTimer += dt;
        titleColor.a = Mathf.Lerp(1, 0, titleFadeTimer / titleFadeTime);
        playColor.a = Mathf.Lerp(1, 0, titleFadeTimer / titleFadeTime);
        TITLE_Text.color = titleColor;
        playText.color = playColor;
    }
    private void OnMenuVolumeChange(float targeVol)
    {
        menuVol = targeVol;
        SetMixerVolume(EFFECT_VOLUME, menuVol);
        PlayerPrefs.SetFloat(EFFECT_VOLUME, menuVol);
    }
    private void OnFullScreenButton()
    {
        screenState = !screenState;
        int n = screenState ? 1 : 0;
        PlayerPrefs.SetInt(TAE.FULLSCREEN, n) ;

        if (screenState)
        {
            StartCoroutine(EnableFullScreen());
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        FullScreenButton.GetComponent<Image>().sprite = screenState ? window : fullscreen;
    }
    private void OnSkipButtonClicked()
    {
        StartCoroutine(ForceFadeAndThenEnableSceneLoad());
    }
    private void ShowSkipButton()
    {
        if (PlayerPrefs.GetInt(TAE.ISEVERPLAYED, 0) != 0 || isTestingMode)
        {
            StartCoroutine(FlashSkipButton());
        }
    }

    //utils
    private void PlaySound(AudioClip ac, float volume, float duration)
    {
        GameObject audio = Instantiate(AudioPrefab);
        AudioSource AS = audio.GetComponent<AudioSource>();
        audio.GetComponent<MenuAS>().timer = duration + 0.1f;

        AS.volume = volume;
        AS.clip = ac;
        AS.spatialBlend = 0;

        AS.Play();
    }
    private void SetMixerVolume(string MixerGroup, float intensity)
    {
        AM.SetFloat(MixerGroup, Mathf.Log10(intensity) * 20);
    }



}

public class ActiveTypingText
{
    //intialisation
    internal TMP_Text tmpT;
    private string outputString;
    private float timePerTick;

    //runtime var
    private float timer;
    private int currentCharIndex;

    public ActiveTypingText(TMP_Text t, string s, float dt)
    {
        tmpT = t;
        outputString = s;
        timePerTick = dt;

        currentCharIndex = 0;
        timer = 0f;
    }

    internal void UpdateText()
    {
        if (tmpT == null)
            return;
        timer += Time.deltaTime;

        if (timer >= timePerTick)
        {
            timer = 0;
            currentCharIndex++;
            string s = outputString.Substring(0, currentCharIndex); // displayed up to the nth char in the string
            s += "<color=#00000000>" + outputString.Substring(currentCharIndex) + "</color>";//keep displayed letters at their final flexed form by keeping the remaining invisible
            tmpT.text = s;
        }

        if (currentCharIndex >= outputString.Length)
        {
            tmpT = null;
        }

    }

    ~ActiveTypingText() { }
}
