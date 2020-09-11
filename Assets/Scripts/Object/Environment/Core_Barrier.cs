using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Core_Barrier : MonoBehaviour
{
    [SerializeField] private MonsterManager manager;
    [SerializeField] private Renderer mainWave;
    [SerializeField] private Transform barrier;
    [SerializeField] private Transform subWave;
    private static readonly int _Threshold = Shader.PropertyToID("_Threshold");

    private void Awake()
    {
        manager.entityHP = manager.monsterData.GetMonsterParam().monsterHP;
        mainWave.sharedMaterial.SetFloat(_Threshold, 0f);
    }

    private float endValue;

    private void OnTriggerEnter(Collider other1)
    {
        // Debug.Log(nameof(OnTriggerEnter));

        if (other1.gameObject.GetComponent<SwordReceiver>() != null)
        {
            endValue += 0.05f;
            mainWave.sharedMaterial.DOFloat(endValue, _Threshold, 1f).Play().OnComplete(() =>
            {
                if (endValue >= 0.6f)
                {
                    OnDeath();
                }
            });
        }
    }

    public void ExpandBarrier()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            barrier.localScale = Vector3.zero;
            barrier.gameObject.SetActive(true);
        });
        sequence.Append(barrier.DOScale(1f, 1f).SetEase(Ease.OutBack));
        sequence.Play().OnComplete(() => InGameMenu.Instance.SetOverlay(true, true));
    }

    public void OnDeath()
    {
        subWave.position = Vector3.zero;
        barrier.gameObject.SetActive(false);
        FlagCore.Instance.SetEpisodeFlag(11, AllFlagList.FlagType.PlayerCanAttack, true);
    }
}