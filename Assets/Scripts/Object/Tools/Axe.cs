using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Axe : MonoBehaviour, IToolable
{
    [SerializeField] private Animator animator;
    [SerializeField] private float woodBreakTime;
    [SerializeField] private float woodBreakInterval;
    public GameObject tmpObject;
    private Transform player;
    private ToolSetter toolSetter;
    private Transform particle;
    public bool canUse;
    public bool hasBroke;
    private readonly int Using = Animator.StringToHash("Using");
    private int axe_use;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation;
        player = GameObject.FindWithTag("Player").transform;
        toolSetter = GameObject.FindWithTag("Player").GetComponent<ToolSetter>();
        axe_use = AudioManager.Instance.GetSeIndex("6_axe_use");
    }

    public void SetTool()
    {
        canUse = true;
    }

    void Update()
    {
        if (canUse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool(Using, true);
                InvokeRepeating(nameof(AudioPlay), 0.1f, woodBreakInterval);
            }

            if (Input.GetMouseButtonUp(0))
            {
                animator.SetBool(Using, false);
                InitializeTool();
            }

            if (Input.GetMouseButton(0))
            {
                if (!animator.GetBool(Using))
                {
                    animator.SetBool(Using, true);
                    InvokeRepeating(nameof(AudioPlay), 0.1f, woodBreakInterval);
                }

                if (particle != null)
                {
                    particle.position = toolSetter.hitObj.point;
                    particle.LookAt(player);
                }
            }
        }
    }

    private float tmpBreakTime;
    Tween anim;

    private void AudioPlay()
    {
        AudioManager.Instance.PlaySe(axe_use);
    }

    public void BreakWood(Transform hitting)
    {
        tmpObject = hitting.gameObject;
        InvokeRepeating(nameof(DestroyWood), woodBreakTime, woodBreakTime);

        particle = hitting.GetChild(0);
    }

    private void DestroyWood()
    {
        IWooden iWooden = tmpObject.GetComponent<IWooden>();

        if (iWooden.IsAlive())
        {
            // Debug.Log("isAliveeee");
            if (iWooden.CheckCanBreak(-1))
            {
                ItemLibrary.Instance.AddItemsForMenu(ItemLibrary.Instance.FindItems(17), 1);
                if (iWooden.HasLeaves())
                    ItemLibrary.Instance.AddItemsForMenu(ItemLibrary.Instance.FindItems(18), 1);
            }
        }
        else
        {
            InitializeTool();
        }

        hasBroke = true;
    }

    public void CancelBreak()
    {
        if (!hasBroke)
            anim.Kill();
    }

    public void InitializeTool()
    {
        // Debug.Log("Initialize");
        canUse = false;
        animator.SetBool(Using, false);
        CancelInvoke();
    }

    public void ToolInit()
    {
        transform.localPosition = defaultPosition;
        transform.localRotation = defaultRotation;
    }
}