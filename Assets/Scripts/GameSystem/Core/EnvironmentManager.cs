using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private List<Collider> lightFlowerColliders;
    [SerializeField] private List<Light> lightFlowerLights;
    [SerializeField] private List<Renderer> renderers;
    
    public void SetFlowerUsable(bool trigger)
    {
        foreach (Collider col in lightFlowerColliders)
        {
            col.enabled = trigger;
        }
    }

    public void SetLightFlower(bool trigger)
    {
        foreach (Light light in lightFlowerLights)
        {
            light.enabled = trigger;
        }

        if (trigger)
            renderers[0].sharedMaterial.EnableKeyword("_EMISSION");
        else
            renderers[0].sharedMaterial.DisableKeyword("_EMISSION");
    }
}