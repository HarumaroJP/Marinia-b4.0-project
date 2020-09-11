using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private Transform playerObject;

    [Serializable]
    public struct ListSaveType
    {
        public int itemID;
        public int value;

        public ListSaveType(ItemData data, int count)
        {
            itemID = data != null ? data.GetItemID() : 0;
            value = count;
        }
    }

    private void Start()
    {
        playerObject = GameObject.FindWithTag("PlayerObjects").transform;

        LoadAllGameObjects();
    }

    private List<SaveDataAsset.SavePlayerObjectType> loadedInstance;

    public void LoadAllGameObjects()
    {
        loadedInstance = MariniaSaveLoader.Instance.SavePlayerObjectData;
        LoadSaveAssets(loadedInstance);
    }

    private ItemManager manager;

    public void LoadSaveAssets(List<SaveDataAsset.SavePlayerObjectType> loadedInstance)
    {
        ItemLibrary.Instance.InitializeLibrary();
        foreach (SaveDataAsset.SavePlayerObjectType data in loadedInstance)
        {
            GameObject prefabObj =
                Marinia.Find( ItemLibrary.Instance.FindItems(data.itemID).GetFileName(), Marinia.ItemType.Build);

            Vector3 position = new Vector3(data._position[0], data._position[1], data._position[2]);
            Quaternion rotation =
                new Quaternion(data._rotation[0], data._rotation[1], data._rotation[2], data._rotation[3]);
            GameObject instanceObj = Instantiate(prefabObj, playerObject.transform);

            instanceObj.transform.SetPositionAndRotation(position, rotation);

            manager = instanceObj.GetComponent<ItemManager>();
            manager.itemSaveData = data.param;
            manager.setSaveData.Invoke();
        }
    }

    public void SaveChildAssets()
    {
        MariniaSaveLoader.Instance.SavePlayerObjectData.Clear();
        foreach (Transform data in playerObject)
        {
            ItemManager manager = data.GetComponent<ItemManager>();
            string paramData = manager.GetCallObjectData();
            int itemID = manager.itemID;
            MariniaSaveLoader.Instance.SavePlayerObjectData.Add(
                new SaveDataAsset.SavePlayerObjectType(itemID, data, paramData));
        }

        MariniaSaveLoader.Save();
    }
}