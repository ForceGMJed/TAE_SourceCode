using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;


public class CinematicChair : Quests
{
    public static event Action<bool, string> TriggerPrompt;
    public static event Action<bool> SitOnChair;
    public static event Action<float> SetSensitivity;
    public static event Action Achievement_Chair;

    private static bool isQuestCompletedOnce;
    private bool IsPlayerOnChair = false;
    private bool isPendingInteraction;

    [SerializeField]
    private Transform playerSeat;


    [SerializeField]
    private Transform offLoadLoc;

    private Transform player;

    private float sitDownTime;
    private float sitDownTimer;
    private Vector3 originalPlayerPos;

    [SerializeField]
    AnimationCurve fadeCurve;


    protected override void Awake()
    {
        base.Awake();

        sitDownTime = ac.length;
        sitDownTimer = 0f;

    }

    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += InteractChair;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= InteractChair;
    }

    private void InteractChair()
    {
        if (!isPendingInteraction)
            return;

        if (!isQuestCompletedOnce)
        {
            isQuestCompletedOnce = true;
            TriggerQuestComplete();

            Achievement_Chair?.Invoke();
        }

        if (IsPlayerOnChair)
        {//get off chair
            player.position = offLoadLoc.position;
            ChangeCamSensitivity(PlayerPrefs.GetFloat("Sensitivity",1.5f));
            SitOnChair?.Invoke(false);
            TriggerPrompt?.Invoke(false, "");
            isPendingInteraction = false;

            sitDownTimer = 0f;
        }
        else
        {//get on chair
            ChangeCamSensitivity(PlayerPrefs.GetFloat("Sensitivity", 1.5f) * 0.8f); 

            SitOnChair?.Invoke(true);

            TriggerPrompt?.Invoke(true, "Get up");
            isPendingInteraction = false;

            originalPlayerPos = player.position;
            StartCoroutine(LerpPlayerPos());

            AS.PlayOneShot(ac);

        }

        IsPlayerOnChair = !IsPlayerOnChair;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isPendingInteraction = true;
            TriggerPrompt?.Invoke(true, "Sit and relax");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            isPendingInteraction = false;
            TriggerPrompt?.Invoke(false, "");
        }
    }

    private void ChangeCamSensitivity(float f)
    {
        SetSensitivity?.Invoke(f);
    }

    private IEnumerator LerpPlayerPos()
    {
        while (sitDownTimer < sitDownTime)
        {
            sitDownTimer += Time.deltaTime;

            player.position = Vector3.Lerp(originalPlayerPos, playerSeat.position, fadeCurve.Evaluate(sitDownTimer/ sitDownTime));

            yield return null;
        }
        isPendingInteraction = true;
        sitDownTimer = 0f;
    }
}
