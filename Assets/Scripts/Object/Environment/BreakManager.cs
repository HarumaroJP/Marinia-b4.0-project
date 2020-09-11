using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BreakManager : MonoBehaviour, IBreakable
{
    [SerializeField] private Collider rangeCollider;
    [SerializeField] private Rigidbody[] childList;

    public void Break()
    {
        foreach (Rigidbody rig in childList)
        {
            rig.isKinematic = false;
            rig.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject)).Play()
                .SetDelay(4f);
        }

        rangeCollider.enabled = false;
    }
}