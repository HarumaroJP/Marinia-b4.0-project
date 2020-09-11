using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public class ItemLibrary : SingletonMonoBehaviour<ItemLibrary>
{
    [Serializable]
    public class InitEvent : UnityEvent<Vector3Int, bool>
    {
    }

    [SerializeField] public ItemDatabase itemDatabase;
    [SerializeField] private MemoDatabase memoDatabase;
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private ToolSetter toolSetter;
    [SerializeField] private PlusItemInfo plusItemInfo;
    [SerializeField] private GameObject dropBox;
    [SerializeField] public List<ItemCountData> itemSlotIDList;
    [SerializeField] public List<ItemCountData> itemMenuIDList;
    [SerializeField] public int maxItemCount;
    [SerializeField] public int minItemCount;
    GameObject ItemListIndex;
    [SerializeField] private GameObject[] menuItems;
    [SerializeField] private GameObject[] slotItems;
    GameObject[][] allItems;

    [Serializable]
    public struct ItemCountData
    {
        public ItemData key;
        public int value;

        public ItemCountData(ItemData a, int b)
        {
            this.key = a;
            this.value = b;
        }
    }

    [Serializable]
    public struct memoHoldType
    {
        public int index;
        public string title;
        public string memo;
    }

    [Serializable]
    public struct canFireList
    {
        public ItemData key;
        public ItemData value;
    }

    [Serializable]
    public struct RecipeMaterialModel
    {
        public ItemData key;
        public int value;
    }

    [SerializeField] List<ItemCountData> itemsForInitialize = new List<ItemCountData>();

    // private readonly Dictionary<ItemData, int> itemCount = new Dictionary<ItemData, int>();


    public void InitializeLibrary()
    {
        ItemListIndex = Marinia.UI("ItemListIndex");
        // menuItems = GameObject.FindGameObjectsWithTag("MenuElem");
        // slotItems = GameObject.FindGameObjectsWithTag("ItemElem");
        allItems = new GameObject[][] {slotItems, menuItems};

        // AllItemGetter();
        InitializedItems();
        SortItems();
        InGameMenu.Instance.InitializeAllMenus();
    }

    public void AllItemGetter() //Debugging Only 
    {
        foreach (ItemData element in itemDatabase.GetItemList())
        {
            ItemCountData itemCountData = new ItemCountData {key = element, value = 10};
            itemsForInitialize.Add(itemCountData);
        }
    }

    public ItemData FindItems(int itemId)
    {
        if (itemId == 0)
            return null;
        return itemDatabase.GetItemList().Find(itemIdTemp => itemIdTemp.GetItemID() == itemId);
    }

    public MemoData FindMemos(int memoId)
    {
        return memoDatabase.GetMemoList()[memoId];
    }

    public MonsterData FindMonsters(int monsterId)
    {
        return monsterDatabase.GetMonsterList()[monsterId];
    }

    public int FindItemCount(ItemData data)
    {
        int count = 0;
        foreach (ItemCountData t in itemMenuIDList.Where(t => data == t.key))
        {
            count += t.value;
        }

        foreach (ItemCountData t in itemSlotIDList.Where(t => data == t.key))
        {
            count += t.value;
        }

        return count;
    }

    public bool CanUseTool(ItemData hittingItem, int activeIndex)
    {
        ItemData ToolData = itemSlotIDList[activeIndex].key;
        return ToolData != null && ToolData.GetCanDestroyObjectList().Contains(hittingItem);
    }

    public void SetArrayOnSlot(int itemId, int count, int index, bool isAdd)
    {
// Debug.Log("SetArrayOnSlot()");
        if (isAdd)
        {
            ItemCountData itemCountData = itemSlotIDList[index];

            // Debug.Log(itemCountData.key);
            // Debug.Log(itemCountData.value);

            if (itemCountData.value == 0)
            {
                string itemName = FindItems(itemId).GetFileName();

                toolSetter.itemSlotObjList[index] = Instantiate(
                    Marinia.Find(itemName, Marinia.ItemType.Hold),
                    toolSetter.SelectingSpace, false);

                if (index != InGameMenu.Instance.selectingValue)
                    toolSetter.itemSlotObjList[index].SetActive(false);
            }

            itemCountData.key = FindItems(itemId);
            itemCountData.value += count;
            itemSlotIDList[index] = itemCountData;
            // Debug.Log("Created");
        }

        else
        {
            ItemCountData itemCountData = itemSlotIDList[index];
            itemCountData.value -= count;
            // Debug.Log(itemCountData.value);
            if (itemCountData.value == 0)
            {
                itemCountData.key = null;
                GameObject tmpObj = toolSetter.itemSlotObjList[index];
                Array.Clear(toolSetter.itemSlotObjList, index, 1);
                Destroy(tmpObj);
            }

            itemSlotIDList[index] = itemCountData;


            // Debug.Log("Destroyed");
        }
    }

    public void UpdateElements(int[] list, int listnum, int itemID)
    {
        list[listnum] = itemID;
    }

//Menu用のCreate & Destoryメソッドを作る
//ドラッグエレメント側も忘れずに

    public void AddItemsForMenu(ItemData item, int count)
    {
        int tmpCount = count;
        bool canLoop = true;
        bool isHave = itemMenuIDList.Any(x => x.key == item) || itemSlotIDList.Any(x => x.key == item); //tmp
        plusItemInfo.ShowInfo(item, count);
        itemDatabase.SetItemHintInfo(item.GetItemID());
        
// Debug.Log(item.GetItemID());
// Debug.Log(isHave);
// itemMenuIDList.Contains<>(item);
        bool isSlot = true;
        while (canLoop)
        {
            if (isHave)
            {
                foreach (GameObject[] itemArrays in allItems)
                {
                    int tmpIndex = 0;
                    List<ItemCountData> tmpList = isSlot ? itemSlotIDList : itemMenuIDList;
                    foreach (GameObject allMenuElement in itemArrays)
                    {
                        if (tmpList[tmpIndex].key == null) continue;
                        // Debug.Log(tmpIndex + " 回目 : " + tmpList[tmpIndex].key.GetItemID() + " == " + item.GetItemID());
                        if (tmpList[tmpIndex].key.GetItemID() == item.GetItemID())
                        {
                            int tmpItemCount = tmpList[tmpIndex].value;
                            if (tmpItemCount < maxItemCount)
                            {
                                // Debug.Log(tmpItemCount + " " + tmpCount);
                                int resultCount = Mathf.Clamp(tmpItemCount + tmpCount, minItemCount, maxItemCount);
                                allMenuElement.GetComponentInChildren<TextMeshProUGUI>().text = resultCount.ToString();

                                AddItems(tmpIndex, item, tmpCount, isSlot);
                                tmpCount = -(maxItemCount - (tmpItemCount + tmpCount));
                                if (tmpCount <= minItemCount) return;
                            }

                            ItemCountData tmpData = tmpList[tmpIndex];
                            tmpData.value += tmpCount;
                            tmpList[tmpIndex] = tmpData;
                        }

                        tmpIndex++;
                    }

                    isSlot = false;
                }

                isHave = false;
            }
            else
            {
                isSlot = true;
                foreach (GameObject[] itemArrays in allItems)
                {
                    int tmpIndex = 0;
                    List<ItemCountData> tmpList = isSlot ? itemSlotIDList : itemMenuIDList;
                    foreach (GameObject allMenuElement in itemArrays)
                    {
                        if (tmpList[tmpIndex].value == 0)
                        {
                            InstantiateItems(item, count, allMenuElement.transform);
                            if (isSlot)
                            {
                                SetArrayOnSlot(item.GetItemID(), count, tmpIndex, true);
                            }
                            else
                            {
                                ItemCountData itemCountData =
                                    itemMenuIDList[tmpIndex];
                                itemCountData.key = item;
                                itemCountData.value = count;
                                itemMenuIDList[tmpIndex] = itemCountData;
                            }

                            // AddItems(tmpIndex, item, count, isSlot);

                            return;
                        }

                        tmpIndex++;
                    }

                    isSlot = false;
                }

                canLoop = false;
            }
        }

        // Debug.LogError("アイテム欄がいっぱいです");
    }

    public void RemoveItemsForMenu(ItemData item, int count)
    {
        int tmpCount = count;
        bool isHave = itemMenuIDList.Any(elem => elem.key == item) || itemSlotIDList.Any(elem => elem.key == item);

        bool isSlot = true;
        if (isHave)
        {
            foreach (GameObject[] itemArrays in allItems)
            {
                int tmpIndex = 0;
                foreach (GameObject allMenuElement in itemArrays)
                {
                    if (OnCheckSlotOrMenu(isSlot, tmpIndex) == item.GetItemID())
                    {
                        // Debug.Log("isHave");
                        TextMeshProUGUI itemText = allMenuElement.GetComponentInChildren<TextMeshProUGUI>();
                        int tmpItemText = int.Parse(itemText.text);
                        // Debug.Log(tmpItemText);

                        int resultCount = Mathf.Clamp(tmpItemText - tmpCount, minItemCount, maxItemCount);

                        itemText.text = resultCount.ToString();

                        bool canDestroy = itemText.text == "0";
                        if (isSlot)
                        {
                            ItemCountData tmp = itemSlotIDList[tmpIndex];
                            tmp.key = canDestroy ? null : tmp.key;
                            tmp.value = canDestroy ? 0 : resultCount;
                            itemSlotIDList[tmpIndex] = tmp;
                        }
                        else
                        {
                            ItemCountData tmp = itemMenuIDList[tmpIndex];
                            tmp.key = canDestroy ? null : tmp.key;
                            tmp.value = canDestroy ? 0 : resultCount;
                            itemMenuIDList[tmpIndex] = tmp;
                        }

                        // Debug.Log(itemText.text);
                        if (canDestroy)
                        {
                            // Debug.Log("itemCount is zero");
                            allMenuElement.GetComponent<ItemSlotScript>().itemHold = false;
                            foreach (Transform child in allMenuElement.transform)
                            {
                                Destroy(child.gameObject);
                            }

                            if (allMenuElement.CompareTag("ItemElem"))
                                SetArrayOnSlot(0, 0, tmpIndex, false);
                        }

                        tmpCount = minItemCount - (tmpItemText - tmpCount);
                        if (tmpCount <= minItemCount) return;
                    }

                    tmpIndex++;
                }

                isSlot = false;
            }
        }

        // Debug.LogError("不正なアイテムリクエストです");
    }

    private int OnCheckSlotOrMenu(bool isSlot, int index)
    {
        ItemData t = isSlot ? itemSlotIDList[index].key : itemMenuIDList[index].key;
        return t == null ? 0 : t.GetItemID();
    }

    private void DebugIDList()
    {
        // foreach (ItemCountData data in itemMenuIDList)
        // {
        //     if (data.key != null)
        //         Debug.LogError(data.key.GetItemName() + " " + data.value);
        // }
        //
        // foreach (ItemCountData data in itemSlotIDList)
        // {
        //     if (data.key != null)
        //         Debug.LogError(data.key.GetItemName() + " " + data.value);
        // }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            DebugIDList();
        }
    }


    private void AddItems(int index, ItemData item, int count, bool isSlot) //Private Only
    {
        List<ItemCountData> tmpElemData = isSlot ? itemSlotIDList : itemMenuIDList;
        ItemCountData elemData = tmpElemData[index];
        elemData.key = item;
        elemData.value += count;
        tmpElemData[index] = elemData;
    }

    public void RemoveItems(ItemData item, int count) //Private Only
    {
        if (itemMenuIDList.Any(elem => elem.key == item))
        {
            ItemCountData elemData = itemMenuIDList.FirstOrDefault(x => x.key == item);
            int index = itemMenuIDList.IndexOf(elemData);
            elemData.value -= count;

            if (elemData.value <= 0)
            {
                itemMenuIDList.Remove(elemData);
            }

            itemMenuIDList[index] = elemData;
        }
        else
        {
            Debug.LogError("存在しないアイテムです");
        }
    }

    public void GetItemList()
    {
// foreach (KeyValuePair<ItemData, int> item in itemCount)
// {
//     Debug.Log(item.Key.GetItemName() + ":" + item.Value);
// }
        Debug.LogError("No Code");
    }

    private void InitializedItems()
    {
        int counter = 0;
        foreach (ItemCountData item in itemsForInitialize)
        {
            if (0 < item.value)
            {
                itemMenuIDList.Add(item);
            }

            counter++;
        }

        for (int i = itemMenuIDList.Count;
            i < 18;
            i++)
        {
            itemMenuIDList.Add(new ItemCountData());
        }

        itemSlotIDList = Enumerable.Repeat(new ItemCountData(), 5).ToList();
    }


    public void SortItems()
    {
        int insertElementsCount = 0;
        // foreach (GameObject menuElement in menuItems)
        // {
        //     foreach (Transform child in menuElement.transform)
        //     {
        //         Destroy(child.gameObject);
        //         child.DetachChildren();
        //     }
        // }
        foreach (ItemCountData items in itemMenuIDList)
        {
            InstantiateItems(items.key, items.value, menuItems[insertElementsCount].transform);
            insertElementsCount++;
        }

// GetItemList();
    }

    ItemDragScript itemDragScript;
    ItemSlotScript itemSlotScript;

    public GameObject InstantiateItems(ItemData itemElem, int count, Transform target)
    {
        if (itemElem == null) return null;

// Debug.Log(itemElem.GetItemID());
        GameObject itemElement =
            Instantiate(ItemListIndex, target, false) as GameObject;

        itemDragScript = itemElement.GetComponent<ItemDragScript>();
        itemDragScript.itemID = itemElem.GetItemID();
        itemDragScript.isSlotting = target.CompareTag("ItemElem");
        itemElement.GetComponent<Image>().sprite = itemElem.GetIcon();
        itemSlotScript = target.GetComponent<ItemSlotScript>();
        TextMeshProUGUI itemText = itemElement.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        itemText.text = count.ToString();
        if (itemSlotScript != null)
        {
            itemSlotScript.itemHold = true;
            itemSlotScript.itemID = itemElem.GetItemID();
            itemSlotScript.elemCounter = itemText;
        }

        ElementHandler handler =
            target.parent.GetComponent<ElementHandler>();
        if (handler != null)
            handler.AddElement(itemElement.GetComponent<Image>());
        return itemElement;
    }

    public GameObject InstantiateDropBox(MonsterData monsterData, Vector3 position)
    {
        GameObject obj = Instantiate(dropBox);
        obj.transform.position = position + new Vector3(0f, 0.5f, 0f);
        obj.GetComponent<DropBox>().monsterData = monsterData;
        PhaseSystem.Instance.MonsterDeathChecker(monsterData, 1);
        return obj;
    }
}