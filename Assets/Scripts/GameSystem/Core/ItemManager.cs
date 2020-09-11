using System;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    [SerializeField] public int itemID;
    [SerializeField] public GameObject parentObj;
    public string itemSaveData = String.Empty;

    public delegate string CallBackObjectData();

    public delegate void CallBackSaveData();

    public CallBackObjectData getData;

    public CallBackSaveData setSaveData;
    public string GetSaveData() => itemSaveData;

    public string GetCallObjectData() => getData?.Invoke();
}