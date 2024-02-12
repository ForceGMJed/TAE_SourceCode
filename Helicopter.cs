using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;

public class Helicopter : MonoBehaviour
{
    public static event Action<bool, string> TriggerPrompt;
    public static event Action<bool> HeliStateTriggerGM;

    public static event Action Achievement_LeaveDrunk;
    public static event Action Achievement_LeaveSober;


    /// <summary>
    /// Listeners will set movement restriction states
    /// </summary>
    public static event Action<bool> HeliState;

    [SerializeField]
    private Transform Heli;

    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    [SerializeField]
    AnimationCurve fadeCurveDescend;
    [SerializeField]
    AnimationCurve fadeCurveAscend;

    [SerializeField]
    private float flightTime;
    private float flightTimer;

    [SerializeField]
    private Transform playerSeat;
    [SerializeField]
    private Transform offLoad;

    private bool isFlight;
    private Vector3 lerpStartPos;
    private Vector3 lerpEndPos;

    private bool IsPlayerOnHeli;
    private bool isPendingInteraction;
    private bool IsPlayFinishAllMission;
    private bool isInitialLand;
    private bool isForceRotorMaxSpin;

    private Transform player;

    private bool isRotorLerp;
    private bool isGameEnding;
    private bool isAllowRotorTriggerLerp;

    private float rotorLerpTimer;
    private float rotorLerpTime;

    [SerializeField]
    private Transform rotorBlades;
    [SerializeField]
    private float rotorMaxSpeed;

    private bool IsTestingMode;
    private bool IsDrunk;

    private void Awake()
    {
        Heli.transform.position = startPoint.position;
        lerpEndPos = endPoint.position;
        lerpStartPos = startPoint.position;
        IsPlayerOnHeli = true;
        isInitialLand = true;
        isGameEnding = false;
        isForceRotorMaxSpin = true;
    }
    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += InteractHeli;
        GameManager.PlayerReadyToLeave += SetExtractionState;
        GameManager.ExtractStartMoveHeli += OnExtractStartMoveHeli;
        GameManager.LerpHeliRotor += OnLerpHeliRotor;
        GameManager.TestingMode += OnTestingMode;
        GameManager.BeginGame += OnBeginGame;
        AlcoholStation.Drunk += OnDrunk;
        Dumpster.Vomit += OnVomit;
    }
    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= InteractHeli;
        GameManager.PlayerReadyToLeave -= SetExtractionState;
        GameManager.ExtractStartMoveHeli -= OnExtractStartMoveHeli;
        GameManager.LerpHeliRotor -= OnLerpHeliRotor;
        GameManager.TestingMode -= OnTestingMode;
        GameManager.BeginGame -= OnBeginGame;
        AlcoholStation.Drunk -= OnDrunk;
        Dumpster.Vomit -= OnVomit;
    }
    private void Update()
    {
        if (isForceRotorMaxSpin)
        {
            rotorBlades.RotateAround(rotorBlades.position, rotorBlades.up, rotorMaxSpeed * Time.deltaTime);
        }

        if (isFlight)
        {
            LerpHeli(Time.deltaTime);
        }


        if (isRotorLerp)
        {
            if (isGameEnding)
            {
                LerpRotor(0f, rotorMaxSpeed, Time.deltaTime);
            }
            else
            {
                LerpRotor(rotorMaxSpeed, 0f, Time.deltaTime);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (IsPlayerOnHeli)
            return;

        if (other.CompareTag("Player"))
        {
            player = other.gameObject.transform;
            isPendingInteraction = true;

            if (IsPlayFinishAllMission)
            {
                TriggerPrompt?.Invoke(true, "Depart");
            }
            else
            {
                TriggerPrompt?.Invoke(true, "Leave");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (IsPlayerOnHeli)
            return;

        if (other.CompareTag("Player"))
        {
            isPendingInteraction = false;
            TriggerPrompt?.Invoke(false, "");
        }
    }

    private void OnBeginGame()
    {
        isFlight = true;
        if (!IsTestingMode)
        {
            HeliState?.Invoke(true);
        }

    }
    private void OnTestingMode(bool obj)
    {
        // Debug.Log("IsTestingMode" + obj);
        IsTestingMode = obj;
        IsPlayerOnHeli = !obj;

    }
    private void LerpHeli(float dt)
    {
        flightTimer += dt;

        if (flightTimer > flightTime)
        {
            isFlight = false;
            isAllowRotorTriggerLerp = true;
            flightTimer = 0f;

            if (IsTestingMode)
                return;
            
            if (player == null)
            {
                player = playerSeat.GetChild(0);
            }
            if (isInitialLand)
            {
                TriggerPrompt?.Invoke(true, "Exit Heli");
                isPendingInteraction = true;
            }

            return;
        }

        if (isGameEnding)
        {
            Heli.transform.position = Vector3.Lerp(lerpStartPos, lerpEndPos, fadeCurveAscend.Evaluate(flightTimer / flightTime));

        }
        else
        {
            Heli.transform.position = Vector3.Lerp(lerpStartPos, lerpEndPos, fadeCurveDescend.Evaluate(flightTimer / flightTime));

        }
        

    }
    private void InteractHeli()
    {
        if (!isPendingInteraction)
            return;

        if (!isAllowRotorTriggerLerp)
            return;

        if (IsPlayerOnHeli)
        {//get off

            HeliStateTriggerGM?.Invoke(false);
            HeliState?.Invoke(false);
            TriggerPrompt?.Invoke(false, "");
            player.parent = null;
            player.position = offLoad.position;
            isPendingInteraction = false;
            IsPlayerOnHeli = false;
            isInitialLand = false;
            isForceRotorMaxSpin = false;
        }
        else
        {//get on , leave for menu

            if (IsDrunk)
            {
                Achievement_LeaveDrunk?.Invoke();
                TriggerPrompt?.Invoke(false, "");
            }
            else
            {
                Achievement_LeaveSober?.Invoke();
                TriggerPrompt?.Invoke(false, "");
            }


            IsPlayerOnHeli = true;
            player.parent = playerSeat;
            player.localPosition = Vector3.zero;
            HeliStateTriggerGM?.Invoke(true);
            HeliState?.Invoke(true);

        }
    }
    private void SetExtractionState()
    {
        IsPlayFinishAllMission = true;
    }
    private void OnExtractStartMoveHeli()
    {
        lerpStartPos = endPoint.position;
        lerpEndPos = startPoint.position;
        isFlight = true;

    }
    private void LerpRotor(float speedStart, float speedEnd, float dt)
    {
        if (rotorLerpTimer > rotorLerpTime)
        {//landing

            isRotorLerp = false;
            rotorLerpTimer = 0f;
            isAllowRotorTriggerLerp = true;

            if (isGameEnding)
            {
                isForceRotorMaxSpin = true;
            }
            return;
        }

        rotorLerpTimer += dt;
        rotorBlades.RotateAround(rotorBlades.position, rotorBlades.up, Mathf.Lerp(speedStart, speedEnd, rotorLerpTimer / rotorLerpTime) * dt);
    }
    public void OnLerpHeliRotor(bool gameEndingState, float lerpTime)
    {
        isAllowRotorTriggerLerp = false;
        rotorLerpTime = lerpTime;
        isGameEnding = gameEndingState;
        isRotorLerp = true;
    }


    private void OnVomit()
    {
        IsDrunk = false;
    }
    private void OnDrunk()
    {
        IsDrunk = true;
    }
}

