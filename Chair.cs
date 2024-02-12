using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Chair : Quests
{
    private static bool isQuestCompletedOnce = false;
    [SerializeField]
    private bool isMakeNoise = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isQuestCompletedOnce)
            {
                TriggerQuestComplete();
                isQuestCompletedOnce = true;
                IsQuestCompleted = true;
            }

            if (!isMakeNoise)
                return;

            AS.PlayOneShot(ac);


        }
    }

}
