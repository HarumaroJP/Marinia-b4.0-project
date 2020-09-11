using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private Light light2;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            light.enabled = !light.enabled;
            light2.enabled = !light2.enabled;
        }
    }

    private void OnDisable()
    {
        light.enabled = false;
        light2.enabled = false;
    }
}