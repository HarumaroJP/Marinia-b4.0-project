using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingFlower_Core : MonoBehaviour
{
    private LightFlower lightFlower;

    private void Start()
    {
        lightFlower = transform.parent.parent.GetComponent<LightFlower>();
    }

    private void OnDestroy()
    {
        lightFlower.canCreate = true;
        lightFlower.gameObject.layer = 0;
    }
}