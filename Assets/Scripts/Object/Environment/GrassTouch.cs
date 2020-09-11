using System;
using UnityEngine;

public class GrassTouch : MonoBehaviour
{
    [SerializeField] public Animation anim;
    
    private void OnTriggerEnter(Collider other)
    {
        anim.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        anim.Play();
    }
}