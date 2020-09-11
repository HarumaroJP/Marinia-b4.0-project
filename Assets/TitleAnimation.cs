using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject bonfireImage;
    [SerializeField] private GameObject cloudImage;
    [SerializeField] private GameObject cloudImage2;
    [SerializeField] private GameObject cloud_event;

    private Tweener cloudAnim;

    void Start()
    {
        bonfireImage.transform.DOLocalRotate(new Vector3(0f, 0f, -9f), 4f).SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutBack).Play();
        cloudImage.transform.DOLocalMoveY(20f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Play();
        cloudImage2.transform.DOLocalMoveY(-13f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Play();

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(cloud_event.transform.DOLocalMoveY(-10f, 1f)
            .SetEase(Ease.OutBounce).SetDelay(3f));
        mySequence.SetLoops(-1, LoopType.Yoyo).Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}