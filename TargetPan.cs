using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetPan : MonoBehaviour
{
    public static event Action PanHit;

    [SerializeField]
    private float panLifeTime;

    [SerializeField]
    private GameObject AudioPrefab;

    [SerializeField]
    private AudioClip panhitSound;
    [SerializeField]
    private AudioClip panHitGroundSound;

    private bool IsDead;

    private void Start()
    {
        Destroy(transform.parent.gameObject,panLifeTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsDead)
            return;


        if (other.CompareTag("BreadPan"))
        {
            PlaySound(panhitSound, 0.9f);
            PanHit?.Invoke();

            IsDead = true;
            return;
        }
        
        if(other.CompareTag("Terrain"))
        {
            PlaySound(panHitGroundSound, 0.1f);
            IsDead = true;
        }
       
    }


    private void PlaySound(AudioClip ac, float volume)
    {
        GameObject audio = Instantiate(AudioPrefab);
        AudioSource AS = audio.GetComponent<AudioSource>();

        AS.volume = volume;
        AS.clip = ac;
        AS.spatialBlend = 0;

        AS.Play();
    }

    ~TargetPan(){ }

}
