using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RigCleaner : MonoBehaviour
{
    [SerializeField] private Rigidbody rig;

    void Start()
    {
        if (rig == null) Destroy(this);
    }

    public void CanClean()
    {
        InvokeRepeating(nameof(Clean), 3f, 3f);
    }

    void Clean()
    {
        if (!(rig.velocity.magnitude <= 0f)) return;
        // Debug.Log("Clean");
        Destroy(rig);
        Destroy(this);
    }
}