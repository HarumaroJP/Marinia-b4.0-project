using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggier : MonoBehaviour, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    [SerializeField] public int itemID;
    public bool isSlotting = false;
    public bool isAll;
    private GameObject itemSlot;
    private GameObject dragObject;
    private GameObject parentObj;
    Image sourceImage;
    [SerializeField] public TextMeshProUGUI holdItemCounter;

    void Awake()
    {
        itemSlot = GameObject.FindWithTag("MainMenu");
        sourceImage = GetComponent<Image>();
        parentObj = transform.parent.gameObject;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            isAll = true;
        CreateDragElement(isAll);
        dragObject.transform.position = data.position;

        // Debug.Log("OnBeginDrag()");
    }

    public void OnDrag(PointerEventData data)
    {
        dragObject.transform.position = data.position;
        // Debug.Log("OnDrag()");
    }

    public void OnEndDrag(PointerEventData data)
    {
        sourceImage.color = Vector4.one;
        GameObject targetObj = data.pointerEnter;
        Destroy(dragObject);
        // Debug.Log(data.pointerEnter);

        if (targetObj == null) return;
        if (targetObj != data.lastPress)
        {
            int compNum;
            switch (targetObj.tag)
            {
                case "ItemElem":
                    compNum = 0;
                    break;
                case "MenuElem":
                    compNum = 1;
                    break;
                case "DragElement":
                    compNum = 2;
                    break;
                default:
                    return;
            }

            ItemDragScript itemDragScript = targetObj.gameObject.GetComponent<ItemDragScript>();
            int addCount = isAll ? int.Parse(holdItemCounter.text) : 1;

            if (compNum == 2)
                if (itemDragScript.itemID != itemID)
                    return;
                else
                {
                    if (itemDragScript.itemID == itemID)
                        itemDragScript.holdItemCounter.text =
                            (int.Parse(itemDragScript.holdItemCounter.text) + addCount).ToString();
                }

            int itemCounter = int.Parse(holdItemCounter.text) - addCount;
            holdItemCounter.text = itemCounter.ToString();

            if (itemCounter == 0)
            {
                if (isSlotting)
                {
                    // ItemLibrary.DestroyObjectOnSlot(parentObj.transform.GetSiblingIndex());
                    // Debug.Log("Destroy from Slot!");
                }
                else if (!isSlotting)
                {
                    // ItemLibrary.ClearElements(parentObj.transform.GetSiblingIndex());
                    // Debug.Log("Destroy from Menu!");
                }

                parentObj.GetComponent<ItemSlotScript>().itemHold = false;
                Destroy(this.gameObject);
            }

            if (compNum == 0)
            {
                // ItemLibrary.SetArrayOnSlot(siblingNum, itemID);
                // Debug.Log("Create on Slot!");
            }
            else if (compNum == 1)
            {
                // ItemLibrary.CreateElements(siblingNum, itemID);
                // Debug.Log("Create on Menu!");
            }
        }

        // Debug.Log("OnEndDrag()");
    }

    void CreateDragElement(bool isAll)
    {
        dragObject = Instantiate(Marinia.UI("ItemListIndex_drag"), itemSlot.transform, true);
        dragObject.transform.SetAsLastSibling();

        Image dragObjImage = dragObject.GetComponent<Image>();
        dragObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
            isAll ? holdItemCounter.text : "1";
        // dragObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        dragObjImage.sprite = sourceImage.sprite;
        // dragObjImage.color = dragObjImage.color;
        // dragObjImage.material = dragObjImage.material;

        sourceImage.color = Vector4.one * 0.6f; //暗くする
    }

    // public void OnHighlightElement(bool isHighlight)
    // {
    //     isHighlight ? 
    // }
}