using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rock : MonoBehaviour, IBreakable
{
    [SerializeField] private ParticleSystem particleSystem;
    private Transform childObj;
    [SerializeField] private int haveRocks = 3;
    [SerializeField] private float currentSize = 1.5f;
    private bool isMoving;

    private void Awake()
    {
        childObj = transform.GetChild(1);
    }

    public void Appear()
    {
        particleSystem.Play();
        childObj.DOLocalMoveY(currentSize, 3f).Play().OnComplete(() => particleSystem.Stop());
    }

    public void Break()
    {
        if (isMoving) return;
        isMoving = true;
        currentSize -= 3f;
        childObj.DOLocalMoveY(currentSize, 3f).Play()
            .OnStart(() => particleSystem.Play())
            .OnComplete(() =>
            {
                particleSystem.Stop();
                isMoving = false;

                if (haveRocks == 0)
                {
                    RockSpawnPoint.Instance.DestroyRock();
                    Destroy(gameObject);
                }
            });
        haveRocks--;
    }
}