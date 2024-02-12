using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summit : Quests
{
    private bool isQuestCompletedOnce;

    public static event Action<float> SetSensitivity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isQuestCompletedOnce)
            {
                isQuestCompletedOnce = true;
                TriggerQuestComplete();
            }

            ChangeCamSensitivity(PlayerPrefs.GetFloat("Sensitivity", 1.5f)* 0.8f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeCamSensitivity(PlayerPrefs.GetFloat("Sensitivity", 1.5f));
        }
    }

    private void ChangeCamSensitivity(float f)
    {
        SetSensitivity?.Invoke(f);
    }
}
