using System.Collections;
using UnityEngine;


public class DoorSound : MonoBehaviour
{
    [SerializeField]
    private GameObject AudioPrefab;
    [SerializeField]
    private AudioClip doorMoveSound;

    private bool isAudioActive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isAudioActive)
                return;
            StartCoroutine(StartDoorSound());
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           // isAudioActive = false; 

        }
    }



    private IEnumerator StartDoorSound()
    {
        PlaySound(doorMoveSound, 0.05f);
        isAudioActive = true;
        yield return new WaitForSeconds(doorMoveSound.length);
        isAudioActive = false;
    }

    //utils
    private void PlaySound(AudioClip ac, float volume)
    {
        GameObject audio = Instantiate(AudioPrefab);
        AudioSource AS = audio.GetComponent<AudioSource>();
 
        AS.volume = volume;
        AS.clip = ac;
        AS.spatialBlend = 0;

        AS.Play();
    }
}
