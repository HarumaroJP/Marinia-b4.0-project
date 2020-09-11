using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightFlower : MonoBehaviour
{
    [SerializeField] private GrassTouch grassTouch;
    [SerializeField] private Transform core;
    private Transform flowerPoint;
    public bool canCreate;

    void Awake()
    {
        Vector3 scale = core.localScale;
        core.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(core.DOScale(scale, 1.2f));
        sequence.AppendCallback(() => grassTouch.anim.Play());
        sequence.Play();

        flowerPoint = transform.GetChild(0);

        InvokeRepeating(nameof(CreateFlower), 1f, 10f);
    }

    void CreateFlower()
    {
        if (canCreate)
        {
            Transform flower = Instantiate(Marinia.Find("LightFlower", Marinia.ItemType.Environment), flowerPoint)
                .transform;
            Vector3 scale = flower.localScale;
            flower.localScale = Vector3.zero;
            flower.DOScale(scale, 2f).SetEase(Ease.OutBack).Play();
            gameObject.layer = 10;
            canCreate = false;
        }
    }
}