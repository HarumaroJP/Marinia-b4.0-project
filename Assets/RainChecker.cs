using System;
using UnityEngine;

public class RainChecker : MonoBehaviour
{
    [SerializeField] private GameObject rain;
    

    private void OnTriggerEnter(Collider other)
    {
        rain.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        rain.SetActive(true);
    }
}