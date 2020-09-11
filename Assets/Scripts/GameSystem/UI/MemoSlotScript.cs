using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int memoID;
    [SerializeField] private bool isHold;
    [SerializeField] private RectTransform slotRect;
    [SerializeField] private Image MemoImage;
    [SerializeField] private MemoGroup memoGroup;
    [SerializeField] private Vector2 highlightScale = new Vector2(60f, 60f);
    [SerializeField] private Vector2 originScale = new Vector2(50f, 50f);

    public void SetMemo()
    {
        MemoImage.color = Color.white;
        isHold = true;
    }

    public void OpenMemo()
    {
        if (!isHold) return;
        memoGroup.MemoOpen(memoID);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        OnHighlightElement(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        OnHighlightElement(false);
    }

    private void OnHighlightElement(bool isHighlight)
    {
        // Debug.Log(nameof(OnHighlightElement));
        TweenerCore<Vector2, Vector2, VectorOptions> anim = slotRect
            .DOSizeDelta(isHighlight ? highlightScale : originScale, 0.3f).SetEase(Ease.OutBack, 3f)
            .SetUpdate(true);
        anim.Play();
    }
}