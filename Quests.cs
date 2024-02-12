using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class Quests : MonoBehaviour
{
    [SerializeField]
    protected QuestTypes quest;

    public static event Action<QuestTypes> QuestCompleted;

    protected bool IsQuestCompleted = false;

    protected AudioSource AS;

    [SerializeField]
    protected AudioClip ac;

    protected virtual void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    protected virtual void TriggerQuestComplete()
    {

        if (IsQuestCompleted)
            return;

        IsQuestCompleted = true;
        QuestCompleted?.Invoke(quest);
    }

    protected virtual void TriggerQuestComplete(QuestTypes qt)
    {
        QuestCompleted?.Invoke(qt);
    }

}

public enum QuestTypes { lift, chairs, bread, sit, piano, summit, drink, drunk };