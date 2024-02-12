using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StarterAssets;

[RequireComponent(typeof(AudioSource))]
public class BreadLauncher : Quests
{
    private static bool isQuestCompletedOnce = false;

    public static event Action<bool, string> TriggerPrompt;
    public static event Action<bool> SetTrialStateOnTrigger;
    public static event Action Achievement_HundredBread;
    public static event Action BreadThrown;

    private bool isPendingInteraction = false;

    private Transform player;
    private Vector3 shootingVector;

    [SerializeField]
    private GameObject breadPrefab;

    [SerializeField]
    private float BaseThrowStrength;

    private float throwStrength;

    public Camera Cam { get; private set; }

    private int sessionTotalBreadThrow;

    private void Start()
    {
       
        throwStrength = BaseThrowStrength;
    }
    private void OnEnable()
    {
        StarterAssetsInputs.Interacted += LaunchBread;
        TargetPractice.SetBreadToTrialStrength += SetBreadToTrialStrength;

    }

    private void OnDisable()
    {
        StarterAssetsInputs.Interacted -= LaunchBread;
        TargetPractice.SetBreadToTrialStrength -= SetBreadToTrialStrength;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Cam==null)
            {
                Cam = Camera.main;
            }

            player = other.transform;
            isPendingInteraction = true;
            TriggerPrompt?.Invoke(true, "Throw bread");
            SetTrialStateOnTrigger?.Invoke(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            isPendingInteraction = false;
            TriggerPrompt?.Invoke(false, "");
            SetTrialStateOnTrigger?.Invoke(false);

        }
    }

    private void LaunchBread()
    {
        if (!isPendingInteraction)
            return;

        if (!isQuestCompletedOnce)
        {
            TriggerQuestComplete();
            isQuestCompletedOnce = true;
        }

        SetBreadPhysics(Instantiate(breadPrefab, transform));
        AS.PlayOneShot(ac);
        BreadThrown?.Invoke(); 
        sessionTotalBreadThrow++;
        if (sessionTotalBreadThrow==100)
        {
            Achievement_HundredBread?.Invoke();
        }

    }

    private void SetBreadPhysics(GameObject g)
    {
        Rigidbody rb = g.GetComponent<Rigidbody>();
        shootingVector = Cam.transform.forward;

        g.transform.position = Cam.transform.position + shootingVector * 0.5f;
        rb.AddForce(shootingVector * throwStrength);
        rb.AddTorque(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30));

    }

    private void SetBreadToTrialStrength(bool state)
    {
        throwStrength = state ? BaseThrowStrength * 3f : BaseThrowStrength;
    }

 
}
