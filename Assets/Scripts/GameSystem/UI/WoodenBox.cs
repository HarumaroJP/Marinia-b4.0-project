using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WoodenBox : MonoBehaviour, IUsable
{
    [SerializeField] private List<ItemLibrary.ItemCountData> holdData;
    [SerializeField] private List<SaveManager.ListSaveType> holdData_jsonData;
    [SerializeField] private List<Transform> elems;
    private ItemSlotScript[] slots = new ItemSlotScript[4];
    [SerializeField] private bool isAlreadyPlace;
    [SerializeField] private bool isFirstLoad;
    [SerializeField] private ItemManager itemManager;

    private void Awake()
    {
        itemManager.getData = GetObjectData;
        itemManager.setSaveData = LoadSaveData;

        Transform tmpElemList = GameObject.FindWithTag("MainMenu").transform.Find("WoodenBox").GetChild(0);
        foreach (Transform elem in tmpElemList)
        {
            elems.Add(elem);
        }

        int tmpIndex = 0;
        foreach (Transform t in elems)
        {
            slots[tmpIndex] = t.GetComponentInChildren<ItemSlotScript>();
            tmpIndex++;
        }

        if (isAlreadyPlace)
        {
            LoadSaveData();
        }

        else
        {
            holdData = Enumerable.Repeat(new ItemLibrary.ItemCountData(null, 0), 8).ToList();
            holdData_jsonData = Enumerable.Repeat(new SaveManager.ListSaveType(null, 0), 8).ToList();
        }
    }

    public void Initialize()
    {
        InGameMenu.Instance.OnUI(false);
        InGameMenu.Instance.OnSimpleMenuOpen(7, true);

        int tmpIndex = 0;
        foreach (Transform t in elems)
        {
            Transform child = t.GetChild(0);
            if (child.childCount > 0)
                Destroy(child.GetChild(0).gameObject);


            slots[tmpIndex].itemHold = holdData[tmpIndex].key != null;
            ItemLibrary.Instance.InstantiateItems(holdData[tmpIndex].key, holdData[tmpIndex].value, child);
            tmpIndex++;
        }

        foreach (ItemSlotScript t in slots)
        {
            t.initializeEvent.RemoveAllListeners();
            t.initializeEvent.AddListener(UpdateChestData);
        }
    }

    public void UpdateChestData(Vector3Int data, bool isAdd)
    {
        ItemLibrary.ItemCountData tmpData = holdData[data.z];
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        holdData[data.z] = tmpData;
    }

    void LoadSaveData()
    {
        string jsonData = itemManager.GetSaveData();
        if (jsonData != String.Empty)
        {
            Chest_SSaveType saveClassData = JsonUtility.FromJson<Chest_SSaveType>(jsonData);
            holdData_jsonData = saveClassData.holdData_jsonData;
        }

        OnBeforeLoad();
        isFirstLoad = false;
    }

    private void OnAfterSave()
    {
        for (int i = 0;
            i < holdData_jsonData.Count;
            i++)
        {
            SaveManager.ListSaveType tmpData = holdData_jsonData[i];
            tmpData.itemID = holdData[i].key != null ? holdData[i].key.GetItemID() : 0;
            tmpData.value = holdData[i].value;
            holdData_jsonData[i] = tmpData;
        }
    }

    private void OnBeforeLoad()
    {
        foreach (SaveManager.ListSaveType data in holdData_jsonData)
        {
            holdData.Add(new ItemLibrary.ItemCountData(ItemLibrary.Instance.FindItems(data.itemID), data.value));
        }
    }

    public string GetObjectData()
    {
        OnAfterSave();

        Chest_SSaveType saveInstance = new Chest_SSaveType
        {
            holdData_jsonData = this.holdData_jsonData,
        };
        return JsonUtility.ToJson(saveInstance, false);
    }
}