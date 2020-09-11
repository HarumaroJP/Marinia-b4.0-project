using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWood : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;

    // 目的地
    private Vector3 destination;
    private Vector3 direction;
    [SerializeField] private float moveSpeed = 0.01f;
    private bool arrived;
    GameObject DriftWood;

    void Start()
    {
        DriftWood = GameObject.Find("DriftWood");
        animator = DriftWood.GetComponent<Animator>();
        destination = new Vector3(185f, 40.7f, 125.4f);
        direction = (destination - transform.position).normalized;
        transform.LookAt(new Vector3(destination.x, transform.position.y, destination.z));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(0, 0, moveSpeed);
    }

    public void Method(float driftSpeed)
    {
        moveSpeed = driftSpeed;
    }
}