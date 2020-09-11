using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchManager : MonoBehaviour
{
    [SerializeField] private GameObject MaterialList;
    [SerializeField] private List<ItemDataInUI> ElementList;
    [SerializeField] public ItemDataInUI ResultBox;
    [SerializeField] private GameObject LeftArrow;
    [SerializeField] private GameObject RightArrow;
    [SerializeField] private RectTransform MainUIRect;
    [SerializeField] private GridLayoutGroup grid;

    // private Dictionary<ItemData, List<ItemLibrary.RecipeMaterialModel>> itemRecipes =
    //     new Dictionary<ItemData, List<ItemLibrary.RecipeMaterialModel>>();

    private GameObject materialElement;

    [Serializable]
    public struct ItemDataInUI
    {
        public ItemData key;
        public GameObject value;
    }

    private RectTransform firstElementRect;

    private void Awake()
    {
        // foreach (ItemData v in items)
        // {
        //     itemRecipes.Add(v, v.GetItemRecipe());
        // }

        // foreach (KeyValuePair<ItemData, List<ItemLibrary.RecipeMaterialModel>> t in itemRecipes)
        // {
        //     Debug.Log(t.Key.GetItemName() + " : " + t.Value);
        // }

        materialElement = Marinia.UI("MaterialElement");
        firstElementRect = ElementList[0].value.GetComponentInChildren<RectTransform>();
    }

    public void Initialize()
    {
        LeftArrow.SetActive(false);
        ShowRecipes(ElementList[0].key);
        OnHighlight(firstElementRect);
    }

    public List<ItemLibrary.RecipeMaterialModel> FindRecipes(ItemData data)
    {
        // foreach (KeyValuePair<ItemData, List<ItemLibrary.RecipeMaterialModel>> t in itemRecipes)
        // {
        //     Debug.Log(t.Key.GetItemName() + " : " + t.Value);
        // }

        return data.GetItemRecipe();
    }

    Vector2 originSize = new Vector2(50f, 50f);
    Vector2 expandSize = new Vector2(65f, 65f);
    private RectTransform tmpRect;
    private Tweener tmpAnim;
    private Tweener dataAnim;
    private ItemData selectItem;
    public ItemLibrary.ItemCountData holdItemTmpData;
    private bool canCreate;

    public void ShowRecipes(ItemData data)
    {
        List<ItemLibrary.RecipeMaterialModel> recipe = FindRecipes(data);
        selectItem = data;
        canCreate = true;

        foreach (Transform obj in MaterialList.transform)
        {
            Destroy(obj.gameObject);
        }

        foreach (ItemLibrary.RecipeMaterialModel item in recipe)
        {
            // Debug.Log(materialElement);
            GameObject instance = Instantiate(materialElement, MaterialList.transform);
            instance.GetComponent<Image>().sprite = item.key.GetIcon();
            int holdCount = ItemLibrary.Instance.FindItemCount(item.key);
            int requireCount = item.value;
            TextMeshProUGUI textMesh = instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMesh.text = $"{holdCount}/{requireCount}";
            if (requireCount - holdCount > 0)
            {
                textMesh.color = Color.red;
                canCreate = false;
            }
            else
            {
                Color textMeshColor = textMesh.color;
                ColorUtility.TryParseHtmlString("#333333", out textMeshColor);
                textMesh.color = textMeshColor;
            }
        }
    }

    public void CreateItem()
    {
        if (ResultBox.value.transform.childCount == 0)
            ResultBox.key = null;

        if (canCreate && (ResultBox.key == null || ResultBox.key == selectItem))
        {
            List<ItemLibrary.RecipeMaterialModel> recipe = FindRecipes(selectItem);
            foreach (ItemLibrary.RecipeMaterialModel model in recipe)
            {
                ItemLibrary.Instance.RemoveItemsForMenu(model.key, model.value);
            }

            if (ResultBox.key == null)
            {
                ItemLibrary.Instance.InstantiateItems(selectItem, 1, ResultBox.value.transform);
                holdItemTmpData.key = selectItem;
                holdItemTmpData.value = 1;
                ItemLibrary.Instance.itemDatabase.SetItemHintInfo(holdItemTmpData.key.GetItemID());
            }
            else
            {
                TextMeshProUGUI textMesh = ResultBox.value.GetComponentInChildren<TextMeshProUGUI>();
                textMesh.text = (int.Parse(textMesh.text) + 1).ToString();
                holdItemTmpData.value += 1;
            }

            ResultBox.key = selectItem;
            ShowRecipes(selectItem);
        }
    }

    public void UpdateGridLayoutGroup()
    {
        grid.enabled = true;
        Invoke(nameof(OffGrid), 0.01f);
    }

    private void OffGrid()
    {
        grid.enabled = false;
    }

    Vector2 offset = new Vector2(245f, 0f);
    private const float duration = 0.5f;
    private int tabNum = 1;
    private int maxNum = 0;

    public void MoveElementUI(bool isLeft)
    {
        int counter = ElementList.Count(x => x.value.activeSelf);
        Debug.Log(counter);
        maxNum = Mathf.CeilToInt(counter / 6f);

        Debug.Log(maxNum);

        if (isLeft)
        {
            Tweener anim = MainUIRect.DOAnchorPos(MainUIRect.anchoredPosition + offset, duration)
                .SetEase(Ease.OutBack, 1f).SetUpdate(true);

            tabNum--;
            anim.Play();
        }
        else
        {
            Tweener anim = MainUIRect.DOAnchorPos(MainUIRect.anchoredPosition - offset, duration)
                .SetEase(Ease.OutBack, 1f).SetUpdate(true);

            tabNum++;
            anim.Play();
        }

        LeftArrow.SetActive(tabNum != 1);
        RightArrow.SetActive(tabNum != maxNum);
    }

    public void OnHighlight(RectTransform data)
    {
        // Debug.Log("OnHighlight");
        if (tmpRect != null)
        {
            tmpAnim = tmpRect.DOSizeDelta(originSize, 1f).SetEase(Ease.OutBack, 3f).SetUpdate(true);
        }

        dataAnim = data.DOSizeDelta(expandSize, 1f).SetEase(Ease.OutBack, 3f).SetUpdate(true);

        tmpAnim.Play();
        dataAnim.Play();

        tmpRect = data;
    }
}