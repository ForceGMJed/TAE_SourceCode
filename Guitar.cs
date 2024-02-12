using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Guitar : MonoBehaviour
{
    public static event Action Achievement_Guitar;

    private bool isAchievementTriggered;

    private void Awake()
    {
        isAchievementTriggered = false; 
    }
    private void OnTriggerEnter(Collider other)
    {

        if (isAchievementTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            isAchievementTriggered = true;
            Achievement_Guitar?.Invoke();
        }
    }
}
