using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftWoodScript : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private MovingWood driftwood;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            animator.speed = 0.0f;

            driftwood.Method(0.0f);

        }

        // Debug.Log("hit!");

    }
}