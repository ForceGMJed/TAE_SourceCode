using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TargetPractice : MonoBehaviour
{

    public static event Action StartTrial;
    public static event Action EndTrial;
    public static event Action<bool> SetBreadToTrialStrength;
    public static event Action ForceSkyScoreOff;

    [SerializeField]
    private GameObject AudioPrefab;

    [SerializeField]
    private Transform InactiveLocation;
    [SerializeField]
    private Transform ActiveLocation;


    [SerializeField]
    private GameObject PanPrefab;
    [SerializeField]
    private AudioClip StageMoveSound;
    [SerializeField]
    private AudioClip PanThrowSound;
    [SerializeField]
    private AudioClip PanThrowSound_sizzle;


    [SerializeField]
    private float BaseThrowingCD;
    [SerializeField]
    private float BaseThrowingStrength;

    private Transform startT;
    private Transform finishT;
    private float StageTransitTime;
    private float lerpTimer;
    private bool IsLerpStage;
    private bool IsStageActive;
    private bool isEnablePanToss;
    private float throwTimer;
    private bool IsTrialActive;
    private float throwPeriod;
    private float currentThrowingCD;

    [SerializeField]
    private Transform stageLoc;

    private void Awake()
    {
        transform.position = InactiveLocation.position;
        startT = InactiveLocation;
        finishT = ActiveLocation;
        lerpTimer = 0f;
        IsLerpStage = false;
        IsStageActive = false;
        throwTimer = 0f;
        StageTransitTime = StageMoveSound.length;
        throwPeriod = 0f;
        currentThrowingCD = BaseThrowingCD;
    }


    private void Update()
    {
        if (IsLerpStage)
        {
            LerpStage(Time.deltaTime);
        }

        if (isEnablePanToss)
        {
            LaunchPans(Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        PowerBox.SetTargetPracticeState += SetTargetPracticeState;
        BreadLauncher.SetTrialStateOnTrigger += SetTrialStateOnTrigger;
        GameManager.SetPanThrowState += SetPanThrowState;
        GameManager.TrialLegitEnd += TrialLegitEnd;
    }


    private void OnDisable()
    {
        PowerBox.SetTargetPracticeState -= SetTargetPracticeState;
        BreadLauncher.SetTrialStateOnTrigger -= SetTrialStateOnTrigger;
        GameManager.SetPanThrowState -= SetPanThrowState;
        GameManager.TrialLegitEnd -= TrialLegitEnd;
    }


    //events listener
    private void SetPanThrowState(float time)
    {
        isEnablePanToss = true;
        throwPeriod = time;
    }
    private void SetTargetPracticeState(bool state)
    {
        PlaySound(StageMoveSound, 0.4f);
        ForceSkyScoreOff?.Invoke();
        if (state)
        {
            startT = InactiveLocation;
            finishT = ActiveLocation;
            IsStageActive = true;
            //PlaySound(PowerUpSound, 0.5f);
        }
        else
        {
            startT = ActiveLocation;
            finishT = InactiveLocation;
            IsStageActive = false;
            //PlaySound(PowerDownSound, 0.5f);
        }

        IsLerpStage = true;
    }
    private void SetTrialStateOnTrigger(bool state)
    {
        if (!IsStageActive)
            return;

        if (state)
        {
            StartTrial?.Invoke();
            SetBreadToTrialStrength?.Invoke(true);
            IsTrialActive = true;
        }
        else
        {
            if (IsTrialActive)
            {
                EndTrial?.Invoke();
                isEnablePanToss = false;
                SetBreadToTrialStrength?.Invoke(false);
            }
            
        }

    }
    private void TrialLegitEnd()
    {
        IsTrialActive = false;
        SetBreadToTrialStrength?.Invoke(false);
    }


    //pprivate methods
    private void LaunchPans(float dt)
    {
        throwTimer += dt;
        throwPeriod -= dt;

        if (throwPeriod <= 0)
        {
            isEnablePanToss = false;
            currentThrowingCD = BaseThrowingCD;
            return;
        }

        if (throwPeriod < 10f)
        {
            currentThrowingCD = BaseThrowingCD * 0.75f;
        }

        if (throwTimer >= currentThrowingCD)
        {
            float spawnOffset = UnityEngine.Random.Range(-1f, 1f);
            GameObject pan = Instantiate(PanPrefab, stageLoc.position + stageLoc.up*1.5f + stageLoc.forward * spawnOffset, Quaternion.identity);
            SetUpPanPhysics(pan, spawnOffset);
            throwTimer = 0f;
            PlaySound(PanThrowSound, 0.2f);
            PlaySound(PanThrowSound_sizzle, 0.3f);
        }
   
    }
    private void SetUpPanPhysics(GameObject go, float spawnOffset)
    {

        Rigidbody rb;
        Vector3 targetVector;

        rb = go.GetComponent<Rigidbody>();
        targetVector = stageLoc.up + stageLoc.forward * UnityEngine.Random.Range(0, 0.3f) * (-spawnOffset/Mathf.Abs(spawnOffset));
        rb.AddForce(targetVector.normalized * BaseThrowingStrength);
        rb.AddTorque(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));

    }
   

    //utils
    private void LerpStage(float dt)
    {
        lerpTimer += dt;

        if (lerpTimer > StageTransitTime)
        {
            IsLerpStage = false;
            lerpTimer = 0f;
            return;
        }

        transform.position = Vector3.Lerp(startT.position, finishT.position, lerpTimer / StageTransitTime);
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


}
