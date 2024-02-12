using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAS : MonoBehaviour
{
    public float timer;
    private void Start()
    {
        Destroy(this.gameObject, timer);
    }
}

