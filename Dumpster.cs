using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class Dumpster : MonoBehaviour
{
    public static event Action<bool, string> TriggerPrompt;
    public static event Action<float> SetSensitivity;
    public static event Action<bool> Barfing;
    public static event Action Vomit;

    private bool IsPlayerBarfing = false;
    private bool isPendingInteraction;
    private bool canBarf;

    private AudioSource AS;
    private Volume volume;
    private VolumeProfile vp;
    private ChromaticAberration CA;

    private float BarfCounter;
    private float currentCA;

    [SerializeField]
    private AudioClip barfSoundFirst;
    [SerializeField]
    private AudioClip barfSoundSecond;

    [SerializeField]
    private GameObject AudioPrefab;

    [SerializeField]
    private float  barfSoundDelay;
    private float barfTime;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
        volume = GameObject.Find("DaySky").GetComponent<Volume>();
        vp = volume.sharedProfile;
        vp.TryGet(out CA);

        barfTime = barfSoundFirst.length+barfSoundSecond.length + barfSoundDelay;
    }

    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += InteractDumpster;
        AlcoholStation.Drunk += EnableBarfing;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= InteractDumpster;
        AlcoholStation.Drunk -= EnableBarfing;
    }
    private void Update()
    {
        if (!IsPlayerBarfing)
            return;

        CompleteBarf(Time.deltaTime);
    }

    private void EnableBarfing()
    {
        canBarf = true;
    }

    private void CompleteBarf(float deltaTime)
    {

        if (BarfCounter > barfTime)
        {
            IsPlayerBarfing = false;
            ChangeCamSensitivity(PlayerPrefs.GetFloat("Sensitivity", 1.5f));
            Barfing?.Invoke(false);
            canBarf = false;
            return;
        }

        BarfCounter += deltaTime;
        CA.intensity.value = Mathf.Lerp(currentCA, 0f, BarfCounter / barfTime);

    }

    private void InteractDumpster()
    {
        if (!isPendingInteraction)
            return;

        if (!canBarf)
        {
            return;
        }
        if (!IsPlayerBarfing)
        {
            ChangeCamSensitivity(PlayerPrefs.GetFloat("Sensitivity", 1.5f)*0.2f);
            Barfing?.Invoke(true);
            Vomit?.Invoke();
            currentCA = CA.intensity.value;
            BarfCounter = 0f;
            IsPlayerBarfing = true;
            StartCoroutine(Barf());
            TriggerPrompt?.Invoke(false, "");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBarf)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (!IsPlayerBarfing)
            {
                TriggerPrompt?.Invoke(true, "Barf");
            }
            isPendingInteraction = true;

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

    private void ChangeCamSensitivity(float f)
    {
        SetSensitivity?.Invoke(f);
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

    private IEnumerator Barf()
    {
        PlaySound(barfSoundFirst,0.5f);
        yield return new WaitForSeconds(barfSoundFirst.length + barfSoundDelay);
        PlaySound(barfSoundSecond, 0.3f);

    }
}
