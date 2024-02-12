using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;

public class PowerBox:MonoBehaviour
{
    public static event Action<bool, string> TriggerPrompt;
    public static event Action<bool> SetTargetPracticeState;
    public static event Action Achievement_ChallengeAccepted;

    private bool isPendingInteraction;
    private bool isFinishAllObjective;
    private bool isPowerBoxOff;
    private bool isFirstTimePowerOn;

    [SerializeField]
    private GameObject AudioPrefab;

    [SerializeField]
    private AudioClip ErrorClip;
    [SerializeField]
    private AudioClip PowerOnClip;
    [SerializeField]
    private AudioClip PowerDownClip;


    private void Awake()
    {
        isFinishAllObjective = false;
        isPowerBoxOff = true;
        isFirstTimePowerOn = true;
    }

    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += InteractPowerBox;
        GameManager.EnableArcade += OnArcadeEnable;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= InteractPowerBox;
        GameManager.EnableArcade -= OnArcadeEnable;
  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPendingInteraction = true;

            if (!isFinishAllObjective)
            {
                TriggerPrompt?.Invoke(true, "???");
                return;
            }

            if (isPowerBoxOff)
            {
                TriggerPrompt?.Invoke(true, "Start Trial");
            }
            else 
            {
                TriggerPrompt?.Invoke(true, "End Trial ");
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

    //private methods

    private void InteractPowerBox()
    {
        if (!isPendingInteraction)
            return;

        if (!isFinishAllObjective)
        {
            PlaySound(ErrorClip, 0.1f);
            //Debug.Log("PlaySound(ErrorClip");
            return;
        }

        if (isFirstTimePowerOn)
        {
            Achievement_ChallengeAccepted?.Invoke();
        } 

        if (isPowerBoxOff)
        {
            PlaySound(PowerOnClip, 0.4f);
            SetTargetPracticeState?.Invoke(true);

            isPowerBoxOff = false;
            //Debug.Log("PlaySound(PowerOnClip");
        }
        else
        {
            PlaySound(PowerDownClip, 0.15f);
            SetTargetPracticeState?.Invoke(false);

            isPowerBoxOff = true;
            //Debug.Log("PlaySound(PowerDownClip");
        }

        TriggerPrompt?.Invoke(false, "");
        isPendingInteraction = false; 

    }

    private void OnArcadeEnable()
    {
        isFinishAllObjective = true;
    }
    
    //utils

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
