using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : MonoBehaviour
{
    [SerializeField]
    private float decommisionTime;

    private void Start()
    {
        Destroy(gameObject, decommisionTime);
    }
     
    



}
