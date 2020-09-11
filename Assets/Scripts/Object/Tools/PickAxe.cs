using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// ReSharper disable All

public class PickAxe : MonoBehaviour, IToolable
{
    [SerializeField] private Animator animator;
    public Transform tmpObject;
    public bool canUse;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation;
    }

    void Update()
    {
        if (canUse)
        {
            if (Input.GetMouseButtonDown(0))
                animator.SetBool(Using, true);
            if (Input.GetMouseButtonUp(0))
                animator.SetBool(Using, false);
        }
    }

    public void SetTool()
    {
        canUse = true;
    }

    private bool hasBroke;
    private Vector3 tmpScale;
    private readonly int Using = Animator.StringToHash("Using");
    Tween delayCall;

    public void BreakRock(Transform hitting)
    {
        tmpObject = hitting;
        tmpScale = tmpObject.localScale;
        delayCall = DOVirtual.DelayedCall(3f, DestroyElement).Play();
        // anim.Append(tmpObject.transform.DOScale(tmpObject.transform.localScale / 2f, 3f).OnComplete(DestroyOre));

        hasBroke = false;
    }

    private void DestroyElement()
    {
        tmpObject.GetComponent<ItemManager>().parentObj.GetComponent<IBreakable>().Break();
        ItemLibrary.Instance.AddItemsForMenu(
            ItemLibrary.Instance.FindItems(tmpObject.GetComponent<ItemManager>().itemID), 1);
        hasBroke = true;
    }

    public void CancelBreak()
    {
        if (!hasBroke)
        {
            if (tmpObject != null)
                tmpObject.localScale = tmpScale;
            delayCall.Kill();
        }
    }

    public void InitializeTool()
    {
        if (!hasBroke && tmpObject != null)
            tmpObject.localScale = tmpScale;
        canUse = false;
        animator.SetBool(Using, false);
    }

    public void ToolInit()
    {
        transform.localPosition = defaultPosition;
        transform.localRotation = defaultRotation;
    }
}