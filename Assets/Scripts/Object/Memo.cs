using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Memo : MonoBehaviour
{
    [SerializeField] public int memoID;
    [SerializeField] private ItemManager itemManager;

    private void Awake()
    {
        itemManager.getData = GetObjectData;
        itemManager.setSaveData = LoadSaveData;
    }

    public void LoadSaveData()
    {
        string jsonData = itemManager.GetSaveData();

        if (jsonData != String.Empty)
        {
            MemoSaveType saveClassData = JsonUtility.FromJson<MemoSaveType>(jsonData);
            memoID = saveClassData.memoID;
        }
    }

    public string GetObjectData()
    {
        MemoSaveType saveInstance = new MemoSaveType
        {
            memoID = this.memoID
        };

        return JsonUtility.ToJson(saveInstance, false);
    }
}