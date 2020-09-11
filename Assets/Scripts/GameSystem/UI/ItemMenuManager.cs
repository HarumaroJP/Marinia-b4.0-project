using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenuManager : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI textMesh;

    public void SetImageAndText(int itemID)
    {
        if (itemID == 0) return;
        ItemData data = ItemLibrary.Instance.FindItems(itemID);
        itemImage.sprite = data.GetIcon();
        textMesh.text = data.GetItemInfo();
    }

    public void SetMenuIDList(Vector3Int data, bool isAdd)
    {
        ItemLibrary.ItemCountData tmpData = ItemLibrary.Instance.itemMenuIDList[data.z];
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        ItemLibrary.Instance.itemMenuIDList[data.z] = tmpData;
    }
}