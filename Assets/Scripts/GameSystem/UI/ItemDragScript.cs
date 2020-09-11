using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragScript : MonoBehaviour, IBeginDragHandler,
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

        if (targetObj == null) return;
        if (targetObj != data.lastPress)
        {
            ItemSlotScript targetSlotScript = targetObj.GetComponent<ItemSlotScript>();
            if (targetSlotScript == null) return;
            if (targetSlotScript.itemHold && targetSlotScript.itemID != itemID) return;

            ItemSlotScript itemSlotScript = parentObj.GetComponent<ItemSlotScript>();
            bool isSlot = targetObj.CompareTag("ItemElem");
            int addCount = isAll ? int.Parse(holdItemCounter.text) : 1;
            int index_add = targetSlotScript.index;

            holdItemCounter.text = (int.Parse(holdItemCounter.text) - addCount).ToString();
            if (itemSlotScript != null)
            {
                int index_remove = itemSlotScript.index;
                if (isSlotting)
                {
                    ItemLibrary.Instance.SetArrayOnSlot(itemID, addCount, index_remove, false);
                    // Debug.Log("Destroy from Slot!");
                }
                else if (!isSlotting)
                {
                    if (itemSlotScript != null)
                        itemSlotScript.SetArray(itemID, addCount, index_remove, false);
                    // Debug.Log("Destroy from Menu!");
                }
            }


            if (holdItemCounter.text == "0")
            {
                if (itemSlotScript != null)
                    itemSlotScript.itemHold = false;
                Destroy(this.gameObject);
            }

            if (isSlot)
            {
                ItemLibrary.Instance.SetArrayOnSlot(itemID, addCount, index_add, true);
                // Debug.Log("Create on Slot!");
            }
            else
            {
                // if (itemSlotScript != null)
                targetSlotScript.SetArray(itemID, addCount, index_add, true);
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
        dragObject.transform.localScale = new Vector3(1.1f, 1.1f, 1f);

        dragObjImage.sprite = sourceImage.sprite;
        // dragObjImage.color = dragObjImage.color;
        // dragObjImage.material = dragObjImage.material;

        sourceImage.color = Vector4.one * 0.6f; //暗くする
    }
}