using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAllAmbientNoise : MonoBehaviour
{


    private GameObject[] AmbientAS;

    private void Awake()
    {
        AmbientAS = GameObject.FindGameObjectsWithTag("AmbientSounds");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject item in AmbientAS)
            {
                item.GetComponent<AudioSource>().Stop();
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject item in AmbientAS)
            {
                item.GetComponent<AudioSource>().Play();
            }
        }

    }

}
