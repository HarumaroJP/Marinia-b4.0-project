using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private GrassTouch grassTouch;
    [SerializeField] private Transform core;

    void Awake()
    {
        Vector3 scale = core.localScale;
        core.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(core.DOScale(scale, 1.2f));
        sequence.AppendCallback(() => grassTouch.anim.Play());
        sequence.Play();
    }
}