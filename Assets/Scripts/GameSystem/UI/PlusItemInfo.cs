using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlusItemInfo : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    private GameObject infoElement;
    [SerializeField] private List<RectTransform> tmpElemRectList = new List<RectTransform>();
    [SerializeField] private List<Sequence> seqList = new List<Sequence>();
    private int movingCount;

    // Start is called before the first frame update
    void Start()
    {
        infoElement = Marinia.UI("PlusInfoElem");
        // itemDatabase.SetItemInfoInitialize();
        Debug.Log(infoElement);
    }

    public void ShowInfo(ItemData data, int count)
    {
        movingCount++;
        // Debug.Log(nameof(ShowInfo) + data.GetItemName());
        RectTransform elemRect = Instantiate(infoElement, transform).GetComponent<RectTransform>();
        elemRect.transform.GetChild(0).GetComponent<Image>().sprite = data.GetIcon();
        elemRect.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + count;

        float to = (movingCount - 1) * -50f - 10f;
        Vector2 elemRectAnchoredPosition = elemRect.anchoredPosition;
        elemRectAnchoredPosition.y = to - 200f;
        elemRect.anchoredPosition = elemRectAnchoredPosition;


        elemRect.DOAnchorPosY(to, 0.7f).Play()
            .OnComplete(() =>
            {
                tmpElemRectList.Add(elemRect);
                movingCount--;
            });
    }

    private Vector2 tmpElemRect;

    private void Update()
    {
        for (int index = 0; index < tmpElemRectList.Count; index++)
        {
            RectTransform rect = tmpElemRectList[index];
            if (rect == null) break;
            if (rect.anchoredPosition.y >= 0f)
            {
                tmpElemRectList.RemoveAt(index);
                Sequence tmpSeq = DOTween.Sequence();
                CanvasGroup group = rect.GetComponent<CanvasGroup>();
                tmpSeq.Append(rect.DOAnchorPosY(200f, 1f));
                tmpSeq.Join(group.DOFade(0f, 0.5f));
                tmpSeq.Play().OnComplete(() =>
                {
                    Destroy(rect.gameObject);
                    seqList.RemoveAt(index);
                });
                seqList.Add(tmpSeq);

                break;
            }

            tmpElemRect = rect.anchoredPosition;
            tmpElemRect.y += 0.7f;
            rect.anchoredPosition = tmpElemRect;
        }
    }
}