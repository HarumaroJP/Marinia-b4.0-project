using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logChild : MonoBehaviour
{
    public bool touched = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
            touched = true;
    }
}
