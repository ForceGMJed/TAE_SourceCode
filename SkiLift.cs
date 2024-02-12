using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;


[RequireComponent(typeof(AudioSource))]
public class SkiLift : Quests
{
    private bool isPendingInteraction;
    private static bool isPlayerOnLift;
    private static bool isQuestcompletedOnce;

    private Transform player;
    [SerializeField]
    private Transform playerSeat;
    [SerializeField]
    private Transform offLoad;

    public static event Action<bool, string> TriggerPrompt;
    public static event Action<bool> BoardLift;



    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += LiftInteracted;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= LiftInteracted;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerOnLift)
            return;


        if (other.CompareTag("Player"))
        {
            player = other.gameObject.transform;
            isPendingInteraction = true;
            TriggerPrompt?.Invoke(true, "Enter");
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


    //private methods

    private void LiftInteracted()
    {
        if (!isPendingInteraction)
            return;


        if (!isQuestcompletedOnce)
        {
            TriggerQuestComplete();
        }
        isQuestcompletedOnce = true;


        if (isPlayerOnLift)
        {//get off the lift
            BoardLift?.Invoke(false);
            TriggerPrompt?.Invoke(false, "");
            player.parent = null;
            player.position = offLoad.position;
            isPendingInteraction = false ;
        }
        else
        {//get on the lift 
            BoardLift?.Invoke(true);
            TriggerPrompt?.Invoke(true, "Leave");
            player.parent = playerSeat;
            player.localPosition = Vector3.zero;
        }

        isPlayerOnLift = !isPlayerOnLift;
    }
}
