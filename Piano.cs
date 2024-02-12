using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(AudioSource))]
public class Piano : Quests
{
    public static event Action<bool, string> TriggerPrompt;

    private bool isQuestCompletedOnce;
    private bool isPendingInteraction;
    private bool IsPlayingMusic;

    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += PlayPiano;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= PlayPiano;
    }

    private void PlayPiano()
    {
        if (!isPendingInteraction)
            return;

        if (!isQuestCompletedOnce)
        {
            isQuestCompletedOnce = true;
            TriggerQuestComplete();
        }

        StartCoroutine(PlayMusic());

    }

    IEnumerator PlayMusic()
    {
        isPendingInteraction = false;
        TriggerPrompt?.Invoke(false, "");
        IsPlayingMusic = true;

        AS.PlayOneShot(ac);

        WaitForSeconds t = new WaitForSeconds(ac.length);

        yield return t;

        IsPlayingMusic = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsPlayingMusic)
                return;
            isPendingInteraction = true;
            TriggerPrompt?.Invoke(true, "Play tune");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsPlayingMusic)
                return;

            isPendingInteraction = false;
            TriggerPrompt?.Invoke(false, "");
        }
    }
}
