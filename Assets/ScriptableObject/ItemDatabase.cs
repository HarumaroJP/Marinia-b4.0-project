using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "CreateItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> itemList = new List<ItemData>();
    [SerializeField] private List<bool> itemHintList = new List<bool>();

    private void Awake()
    {
        SetItemInfoInitialize();
    }

    public List<ItemData> GetItemList()
    {
        return itemList;
    }

    public int GetItemCount()
    {
        return itemList.Count;
    }

    public void SetItemInfoInitialize()
    {
        itemHintList = Enumerable.Repeat(false, itemList.Count).ToList();
    }

    public bool SetItemHintInfo(int itemId)
    {
        bool result = itemHintList[itemId - 1];
        itemHintList[itemId - 1] = true;

        if (!result)
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                if (itemId == 30) FlagCore.Instance.SetEpisodeFlag(13, AllFlagList.FlagType.TakeLightFlower, true);
                // if (itemId == 23) 
            }).Play();
        }

        return result;
    }
}