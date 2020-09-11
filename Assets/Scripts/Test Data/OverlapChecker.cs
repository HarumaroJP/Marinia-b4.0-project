using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverlapChecker : MonoBehaviour
{
    [SerializeField] private Vector3 center;

    [SerializeField] private Vector3 halfExtents;
    //
    // private void Update()
    // {
    //     Collider[] size = Physics.OverlapBox(transform.position, halfExtents / 2, transform.rotation);
    //     foreach (Collider t in size.Where(col => col != null))
    //     {
    //         Debug.Log(t.gameObject.name);
    //     }
    // }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(center, halfExtents);
    }
}