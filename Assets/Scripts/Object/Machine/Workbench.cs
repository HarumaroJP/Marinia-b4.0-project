using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Workbench : MonoBehaviour, IUsable
{
    [SerializeField] private Button endButton;
    [SerializeField] private GameObject WorkbenchUI;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private GameObject resultBox;
    [SerializeField] private WorkbenchManager manager;
    [SerializeField] private ItemLibrary.ItemCountData holdItem;
    [SerializeField] private SaveManager.ListSaveType holdData_jsonData;
    [SerializeField] private bool isFirstLoad;
    [SerializeField] private bool isAlreadyPlace;

    private void Awake()
    {
        itemManager.getData = GetObjectData;
        itemManager.setSaveData = LoadSaveData;
        GameObject mainMenu = GameObject.FindWithTag("MainMenu");
        WorkbenchUI = mainMenu.transform.Find("Workbench").gameObject;
        manager = WorkbenchUI.GetComponent<WorkbenchManager>();
        resultBox = WorkbenchUI.transform.GetChild(1).GetChild(0).gameObject;
        endButton = mainMenu.transform.Find("ItemGroupBackGround").GetChild(0).GetComponent<Button>();
    }

    public void LoadSaveData()
    {
        string jsonData = itemManager.GetSaveData();

        if (jsonData != String.Empty)
        {
            WorkbenchSaveType saveClassData = JsonUtility.FromJson<WorkbenchSaveType>(jsonData);

            holdData_jsonData = saveClassData.holdData_jsonData;
        }

        OnBeforeLoad();
        isFirstLoad = false;
    }

    public void Initialize()
    {
        InGameMenu.Instance.OnUI(false);
        InGameMenu.Instance.OnSimpleMenuOpen(2, true);

        manager.Initialize();

        if (resultBox.transform.childCount > 0)
            Destroy(resultBox.transform.GetChild(0).gameObject);

        ItemLibrary.Instance.InstantiateItems(holdItem.key, holdItem.value, resultBox.transform);

        endButton.onClick.RemoveListener(endEvent);
        endButton.onClick.AddListener(endEvent);
    }

    private void endEvent()
    {
        holdItem.key = manager.holdItemTmpData.key;
        holdItem.value = manager.holdItemTmpData.value;
    }

    public void OnAfterSave()
    {
        SaveManager.ListSaveType tmpData = holdData_jsonData;
        tmpData.itemID = holdItem.key != null ? holdItem.key.GetItemID() : 0;
        tmpData.value = holdItem.value;
        holdData_jsonData = tmpData;
    }

    public void OnBeforeLoad()
    {
        if (isFirstLoad)
        {
            holdItem = new ItemLibrary.ItemCountData(null, 0);
            holdData_jsonData = new SaveManager.ListSaveType(null, 0);
        }
        else
        {
            holdItem = new ItemLibrary.ItemCountData(ItemLibrary.Instance.FindItems(holdData_jsonData.itemID),
                holdData_jsonData.value);
        }
    }

    public string GetObjectData()
    {
        OnAfterSave();

        WorkbenchSaveType saveInstance = new WorkbenchSaveType
        {
            holdData_jsonData = this.holdData_jsonData
        };

        return JsonUtility.ToJson(saveInstance, false);
    }
}