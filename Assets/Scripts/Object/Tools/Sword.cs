using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IToolable
{
    [SerializeField] private Animator animator;
    [SerializeField] private SwordReceiver receiver;
    private static readonly int Trigger = Animator.StringToHash("trigger");
    private readonly int idle = Animator.StringToHash("Idle");
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        // Debug.Log("Start");
        defaultPosition = receiver.transform.parent.localPosition;
        defaultRotation = receiver.transform.parent.localRotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("clicked");
            animator.SetTrigger(Trigger);
        }
    }

    public void SendDamage(int trigger) => receiver.OnHitCheck(trigger == 1);

    public void SetTool()
    {
        
    }

    public void ToolInit()
    {
        // Debug.Log("init");
        animator.Play(idle);
        receiver.transform.parent.localPosition = defaultPosition;
        receiver.transform.parent.localRotation = defaultRotation;
    }
}