using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ItemStatus", menuName = "CreateElement")]
public class ItemData : ScriptableObject
{
    [SerializeField] private int itemID;

    [SerializeField] private string fileName;

    // アイテムの名前
    [SerializeField] private string itemName;

    [SerializeField] private List<ItemLibrary.RecipeMaterialModel> recipe;

    [SerializeField] private Vector3 lapCenter;

    [SerializeField] private Vector3 lapSize;


    public enum Class
    {
        Weapon,
        Tool,
        Building,
        Material,
        Food,
        Drink
    }

    public enum SetType
    {
        None,
        Sea,
        Ground
    }

    public enum ColliderType
    {
        None,
        Mesh,
        Box,
        Sphere
    }

    // アイテムの種類
    [SerializeField] private Class itemClass;

    //アイテムの配置可能場所
    [SerializeField] private SetType itemSetType;

    //アイテムのコライダータイプ
    [SerializeField] private ColliderType itemColliderType;

    // アイテムのアイコン
    [SerializeField] private Sprite itemIcon;

    // アイテムのHP
    [SerializeField] private int itemHP;

    [SerializeField] private int itemDamage;

    [SerializeField] private bool canDestroyFromAll;

    [SerializeField] private List<ItemData> canDestroyObject;

    [SerializeField] private string itemInfo;

    [SerializeField] private KeyCode[] takeKeys;

    [SerializeField] private bool changeChild;

    public string GetItemClass() => itemClass.ToString();

    public string GetItemSetType() => itemSetType.ToString();

    public string GetItemColliderType() => itemColliderType.ToString();

    public (Vector3, Vector3) GetOverlapData() => (lapCenter, lapSize / 2);

    public Sprite GetIcon() => itemIcon;
    public string GetItemName() => itemName;
    public string GetFileName() => fileName;
    public int GetItemID() => itemID;

    public int GetItemHP() => itemHP;

    public List<ItemLibrary.RecipeMaterialModel> GetItemRecipe() => recipe;

    public List<ItemData> GetCanDestroyObjectList() => canDestroyObject;

    public bool CanDestroyFromAll() => canDestroyFromAll;

    public string GetItemInfo() => itemInfo;

    public int GetItemDamage() => itemDamage;

    public (KeyCode[], bool) GetTakeKeyInfo() => (takeKeys, changeChild);
}