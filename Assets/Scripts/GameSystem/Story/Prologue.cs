using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Prologue : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera firstCamera;
    [SerializeField] private CinemachineVirtualCamera blendCamera;
    [SerializeField] private Image titleImage1;
    [SerializeField] private Image titleImage2;
    private CinemachineTrackedDolly dolly;
    private bool canStartEpisode;

    private void Start()
    {
        dolly = firstCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        //TODO 開始コマンドを書く
        //・DOTweenを用いてなめらかなカメラ移動
        //・最後辺りにPriorityを切り替えてカメラをBlendする
        //・ロゴの表示
        //・時間があればストーリーの実装
        StartPrologue();
    }

    public void StartPrologue()
    {
        InGameMenu.Instance.OnUIWithoutTimeScale();
        InGameMenu.Instance.SetOverlay(true,false);
        AudioManager.Instance.PlayBgm(2, false);
        DOTween.To(() => dolly.m_PathPosition, x => dolly.m_PathPosition = x, 1f, 30f).Play().OnComplete(EndPrologue);
    }

    private void EndPrologue()
    {
        titleImage1.DOFade(1f, 2f).SetDelay(1.5f).Play();
        titleImage2.DOFade(1f, 2f).SetDelay(1.5f).Play().OnComplete(StartMainEpisode);
        blendCamera.Priority = 200;
    }

    private void StartMainEpisode()
    {
        Sequence selectedAnim = DOTween.Sequence();
        // selectedAnim.Append(titleImage1.rectTransform.DOScale());
        canStartEpisode = true;
    }

    private void Update()
    {
        if (canStartEpisode && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
        }
    }
}