using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemMenuManager itemMenuManager;
    [SerializeField] private Image nowImage;
    [SerializeField] public int itemID;
    [SerializeField] public int index;
    [SerializeField] public ItemLibrary.InitEvent initializeEvent = new ItemLibrary.InitEvent();
    private Sprite nowSprite;
    public TextMeshProUGUI elemCounter;
    private ElementHandler elementHandler;
    [SerializeField] public bool itemHold = false;
    [SerializeField] private RectTransform slotRect;
    GameObject dragObject;
    private int on_highlight;

    void Start()
    {
        elementHandler = transform.parent.GetComponent<ElementHandler>();
        on_highlight = AudioManager.Instance.GetSeIndex("4_highlight");
    }

    public void SetArray(int itemId, int count, int index, bool isAdd) =>
        initializeEvent.Invoke(new Vector3Int(itemId, count, index), isAdd);

    //TODO 初期化関数を設定する

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // Debug.Log(nameof(OnPointerEnter));
        OnHighlightElement(true);

        if (itemHold && itemMenuManager != null)
        {
            itemMenuManager.SetImageAndText(itemID);
        }

        if (pointerEventData.pointerDrag == null) return;
        AudioManager.Instance.PlaySe(on_highlight);
        if (itemHold)
            return;
        Image droppedImage = pointerEventData.pointerDrag.GetComponent<Image>();
        nowImage.sprite = droppedImage.sprite;
        nowImage.color = Vector4.one * 0.6f;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        OnHighlightElement(false);

        if (pointerEventData.pointerDrag == null) return;
        nowImage.color = Vector4.zero; //仮のカラー指定
    }

    public void OnDrop(PointerEventData pointerEventData)
    {
        // Debug.Log("OnDrop");
        GameObject dragObj = pointerEventData.pointerDrag;
        if (dragObj.CompareTag("DragElement"))
        {
            // Debug.Log("OnDrop");
            ItemDragScript itemDragScript = dragObj.GetComponent<ItemDragScript>();
            int addCount = itemDragScript.isAll ? int.Parse(itemDragScript.holdItemCounter.text) : 1;
            if (itemHold)
            {
                if (itemDragScript.itemID == itemID)
                    elemCounter.text = (int.Parse(elemCounter.text) + addCount).ToString();
            }
            else
            {
                ItemData itemData =
                    ItemLibrary.Instance.FindItems(itemDragScript.itemID);

                GameObject createdObj = ItemLibrary.Instance.InstantiateItems(itemData, addCount, gameObject.transform);
                elemCounter = createdObj.GetComponentInChildren<TextMeshProUGUI>();
                itemID = itemData.GetItemID();
            }

            nowImage.color = Vector4.zero; //仮のカラー指定
        }
    }

    [SerializeField] private Vector2 highlightScale = new Vector2(60f, 60f);
    [SerializeField] private Vector2 originScale = new Vector2(50f, 50f);

    private void OnHighlightElement(bool isHighlight)
    {
        // Debug.Log(nameof(OnHighlightElement));
        TweenerCore<Vector2, Vector2, VectorOptions> anim = slotRect
            .DOSizeDelta(isHighlight ? highlightScale : originScale, 0.3f).SetEase(Ease.OutBack, 3f)
            .SetUpdate(true);
        anim.Play();
    }
}