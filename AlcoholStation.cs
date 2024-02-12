using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class AlcoholStation : Quests
{
    public static event Action<bool, string> TriggerPrompt;
    public static event Action Drunk;
    public static event Action Achievements_Drink10Times;
    private bool isPendingInteraction = false;

    private bool isAlcoholBought = false;
    private bool isFirstTimeDrunk = false;
    private bool isDrinkingCD = false;

    private int drankTimes;
    private int sessionTotalDrankTimes;

    private Volume v;
    private ChromaticAberration CA;
    private VolumeProfile vp;

    [SerializeField]
    private float LDChangeSensitivity;
    [SerializeField]
    private float drinkingCD;
    private float t = 0;
    private float minLD;
    private float maxLD;

    [SerializeField]
    private AudioClip drinking;
    [SerializeField]
    private AudioClip buying;

    [SerializeField]
    private float drinkingVolume;
    [SerializeField]
    private float buyingVolume;

    [SerializeField]
    private GameObject AudioPrefab;

    private bool isStopCA;

    [SerializeField]
    private float barfReqDrankTimes;

    protected override void Awake()
    {
        v = GameObject.Find("DaySky").GetComponent<Volume>();
        vp = v.sharedProfile;
        vp.TryGet(out CA);
        StopCA();
    }

    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += BuyOrDrinkAlcohol;
        Dumpster.Vomit += StopCA;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= BuyOrDrinkAlcohol;
        Dumpster.Vomit -= StopCA;
    }

    private void Update()
    {
        if (!isAlcoholBought)
        {
            return;
        }
        if (!isStopCA)
        {
            SetCA(Time.deltaTime);
        }

    }

    private void StopCA()
    {
        isStopCA = true;
        CA.intensity.value = 0f;

        drankTimes = 0;
    }
    private void BuyOrDrinkAlcohol()
    {

        if (!isPendingInteraction)
            return;

        if (isDrinkingCD)
            return;

        if (isAlcoholBought)
        {
            DrinkOnce();
            return;
        }

        isAlcoholBought = true;
        TriggerQuestComplete(QuestTypes.drink);
        PlaySound(buying, buyingVolume);

        TriggerPrompt?.Invoke(true, "Drink your troubles away");
    }

    private void DrinkOnce()
    {
        drankTimes++;
        sessionTotalDrankTimes++;
        //SetCA(drankTimes);
        isStopCA = false;

        if (sessionTotalDrankTimes ==10)
        {
            Achievements_Drink10Times?.Invoke();
        }




        float absRange = Mathf.Clamp((1f / 5f) * drankTimes, 0f, 1f);
        minLD = 0;
        maxLD = absRange;

        PlaySound(drinking, drinkingVolume);

        if (drankTimes >= barfReqDrankTimes)
        {
            Drunk?.Invoke();

            if (!isFirstTimeDrunk)
            {
                isFirstTimeDrunk = true;

                TriggerQuestComplete(QuestTypes.drunk);
            }

        }

        StartCoroutine(DrinkingCoolDown());

    }

    IEnumerator DrinkingCoolDown()
    {

        TriggerPrompt?.Invoke(false, "");
        isDrinkingCD = true;

        WaitForSeconds t = new WaitForSeconds(drinkingCD);
        yield return t;

        isDrinkingCD = false;
        if (isPendingInteraction)
        {
            TriggerPrompt?.Invoke(true, "Drink your troubles away");
        }


    }

    //private void SetCA(int drankTimes)
    //{ 
    //    CA.intensity.value = Mathf.Clamp(0.15f * drankTimes,0,1);
    //}
    private void SetCA(float dt)
    {

        CA.intensity.value = Mathf.Lerp(minLD, maxLD, t);
        t += LDChangeSensitivity * dt;

        if (t > 1.0f)//flip direciton of lerp when max is reach
        {
            float temp = maxLD;
            maxLD = minLD;
            minLD = temp;
            t = 0.0f;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPendingInteraction = true;
            if (isAlcoholBought && !isDrinkingCD)
            {
                TriggerPrompt?.Invoke(true, "Drink your troubles away");
                return;
            }
            else if (!isAlcoholBought)
            {
                TriggerPrompt?.Invoke(true, "Buy Alcohol");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPendingInteraction = false;
            TriggerPrompt?.Invoke(false, "");
        }
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
