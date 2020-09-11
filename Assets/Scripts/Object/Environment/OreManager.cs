using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OreManager : MonoBehaviour, IBreakable
{
    public int myNumber;

    private void OnDestroy()
    {
        transform.parent.parent.GetComponent<IcoSpawn>()?.DestroyOre(myNumber);
    }

    public void Break()
    {
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject)).Play();
    }
}