using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;

//deprecatred
public class Extraction : MonoBehaviour
{
    //public static event Action<bool, string> TriggerPrompt;
    //public static event Action Extract;

    //private bool isPendingInteraction;

    //private bool IsPlayFinishAllMission;
    //private bool IsPendingExtraction;
  

    //private void Awake()
    //{
    //    IsPendingExtraction = false;
    //}
    //private void OnEnable()
    //{

    //    StarterAssetsInputs.Interacted += InteractExtraction;
    //    GameManager.PlayerReadyToLeave += SetExtractionState;
    //    AlcoholStation.Drunk += OnDrunk;
    //    Dumpster.Vomit += OnVomit;
    //}
    //private void OnDisable()
    //{
    //    StarterAssetsInputs.Interacted -= InteractExtraction;
    //    GameManager.PlayerReadyToLeave -= SetExtractionState;
    //    AlcoholStation.Drunk -= OnDrunk;
    //    Dumpster.Vomit -= OnVomit;

    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (IsPendingExtraction)
    //        return;


    //    if (other.CompareTag("Player"))
    //    {
    //        isPendingInteraction = true;

    //        if (IsPlayFinishAllMission)
    //        {
    //            TriggerPrompt?.Invoke(true, "Depart");
    //        }
    //        else
    //        {
    //            TriggerPrompt?.Invoke(true, "Leave");
    //        }
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {

    //        isPendingInteraction = false;
    //        TriggerPrompt?.Invoke(false, "");
    //    }
    //}

    //private void InteractExtraction()
    //{
    //    if (!isPendingInteraction)
    //        return;

    //    if (IsPendingExtraction)
    //        return;

    //    if (IsPlayFinishAllMission)
    //    {
         
    //        Extract?.Invoke();
    //        TriggerPrompt?.Invoke(false, "");
    //    }
    //    else
    //    {

    //        Extract?.Invoke();
    //        TriggerPrompt?.Invoke(false, "");
    //    }

    //    IsPendingExtraction = true;

    //}
    //private void SetExtractionState()
    //{
    //    IsPlayFinishAllMission = true;
    //}



}


