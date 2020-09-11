using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour, IUsable
{
    public MonsterData monsterData;

    public void Initialize()
    {
        foreach (MonsterDatabase.MonsterDropData data in monsterData.GetDropData())
        {
            ItemLibrary.Instance.AddItemsForMenu(
                ItemLibrary.Instance.FindItems(data.item.GetItemID()), Random.Range(data.minValue, data.maxValue + 1));
        }

        Destroy(gameObject);
    }
}