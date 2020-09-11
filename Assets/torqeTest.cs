using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class torqeTest : MonoBehaviour
{
    [SerializeField] private Rigidbody rig;
    public float speed = 10;

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        rig.AddForce(x * speed, 0, z * speed);
    }

    void OnTriggerEnter(Collider hit)
    {
        // Debug.Log("called!!!");
    }
}
