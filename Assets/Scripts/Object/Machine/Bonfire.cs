using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Bonfire : MonoBehaviour, IUsable
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject BonfireUI;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private Image fireGage;
    [SerializeField] private Button endButton;
    [SerializeField] private bool isFirstLoad;
    [SerializeField] private bool isAlreadyPlace; //Developing Only
    [SerializeField] private bool isPlaying;
    [SerializeField] private bool isOpening;
    [SerializeField] private GameObject fire_before;
    [SerializeField] private GameObject fire_after;
    [SerializeField] private GameObject fuels;
    [SerializeField] private ItemLibrary.ItemCountData holdData;
    [SerializeField] private ItemLibrary.ItemCountData holdData_end;
    [SerializeField] private ItemLibrary.ItemCountData fuelData;
    private ItemSlotScript[] slots = new ItemSlotScript[3];
    [SerializeField] private ItemLibrary.canFireList[] canFireObjList_tmp;
    [SerializeField] private SaveManager.ListSaveType holdData_jsonData;
    [SerializeField] private SaveManager.ListSaveType holdData_end_jsonData;
    [SerializeField] private SaveManager.ListSaveType fuelData_jsonData;
    private ItemLibrary.ItemCountData[] tuple_beforeSave;
    private SaveManager.ListSaveType[] tuple_afterSave;

    private Dictionary<ItemData, ItemData> canFireObjList = new Dictionary<ItemData, ItemData>();

    private void Awake()
    {
        foreach (ItemLibrary.canFireList data in canFireObjList_tmp)
        {
            canFireObjList.Add(data.key, data.value);
        }


        GameObject mainMenu = GameObject.FindWithTag("MainMenu");
        BonfireUI = mainMenu.transform.Find("Bonfire").gameObject;
        endButton = mainMenu.transform.Find("ItemGroupBackGround").GetChild(0).GetComponent<Button>();
        fireGage = BonfireUI.transform.GetChild(4).GetComponent<Image>();
        fire_before = BonfireUI.transform.GetChild(0).gameObject;
        fire_after = BonfireUI.transform.GetChild(1).gameObject;
        fuels = BonfireUI.transform.GetChild(2).gameObject;

        slots[0] = fire_before.GetComponentInChildren<ItemSlotScript>();
        slots[1] = fire_after.GetComponentInChildren<ItemSlotScript>();
        slots[2] = fuels.GetComponentInChildren<ItemSlotScript>();

        itemManager.getData = GetObjectData;
        itemManager.setSaveData = LoadSaveData;

        holdData = new ItemLibrary.ItemCountData(null, 0);
        holdData_end = new ItemLibrary.ItemCountData(null, 0);
        fuelData = new ItemLibrary.ItemCountData(null, 0);

        holdData_jsonData = new SaveManager.ListSaveType(null, 0);
        holdData_end_jsonData = new SaveManager.ListSaveType(null, 0);
        fuelData_jsonData = new SaveManager.ListSaveType(null, 0);

        if (isAlreadyPlace)
        {
            LoadSaveData();
        }
    }

    public void LoadSaveData()
    {
        string jsonData = itemManager.GetSaveData();

        if (jsonData != String.Empty)
        {
            BonfireSaveType saveClassData = JsonUtility.FromJson<BonfireSaveType>(jsonData);

            isFirstLoad = false;
            holdData_jsonData = saveClassData.holdData_jsonData;
            holdData_end_jsonData = saveClassData.holdData_end_jsonData;
            fuelData_jsonData = saveClassData.fuelData_jsonData;
        }

        OnBeforeLoad();
        OnCheckInBonfire();
    }

    public void Initialize()
    {
        InGameMenu.Instance.OnUI(false);
        InGameMenu.Instance.OnSimpleMenuOpen(1, true);

        isOpening = true;

        endButton.onClick.RemoveListener(endEvent);
        endButton.onClick.AddListener(endEvent);

        foreach (ItemSlotScript t in slots)
        {
            t.initializeEvent.RemoveAllListeners();
        }

        slots[0].initializeEvent.AddListener(UpdateCoreData);
        slots[1].initializeEvent.AddListener(UpdateCoreDataToFuel);
        slots[2].initializeEvent.AddListener(UpdateCoreDataToFireEnd);

        InitializeCoreData();
        OnCheckInBonfire();
    }

    private void InitializeCoreData()
    {
        GameObject[] tuple_GameObject = {fire_before, fire_after, fuels};
        ItemLibrary.ItemCountData[] tuple_ItemCountData = {holdData, holdData_end, fuelData};

        foreach (GameObject obj in tuple_GameObject)
        {
            foreach (ItemLibrary.ItemCountData countData in tuple_ItemCountData)
            {
                Transform Trans = obj.transform.GetChild(0);
                if (Trans.childCount > 0)
                    Destroy(Trans.GetChild(0).gameObject);

                ItemSlotScript slot = Trans.GetComponent<ItemSlotScript>();
                slot.itemHold = holdData.key != null;
                ItemLibrary.Instance.InstantiateItems(countData.key, countData.value, Trans);
            }
        }
    }

    private float progress;
    private const int endValue = 12;
    private Tweener fireLoop;

    private void On()
    {
        fireLoop = DOTween.To(x => progress = x, 0, endValue, 2f).OnComplete(UpdateCore).SetUpdate(true);
        fireLoop.Play();
        particle.Play();
        isPlaying = true;
    }

    void UpdateCore()
    {
        ItemLibrary.ItemCountData data = holdData;
        if (data.key != null)
        {
            if (canFireObjList.ContainsKey(data.key))
            {
                ItemData firedData = canFireObjList[data.key];
                bool canCreate = holdData_end.key == firedData;

                if (canCreate)
                {
                    holdData_end.value += 1;

                    if (isOpening)
                    {
                        slots[1].elemCounter.text =
                            (int.Parse(slots[1].elemCounter.text) + 1).ToString();
                    }
                }
                else
                {
                    holdData_end.key = firedData;
                    holdData_end.value += 1; //レベルによって変更する可能性あり
                    if (isOpening)
                    {
                        ItemLibrary.Instance.InstantiateItems(firedData, 1, fire_after.transform.GetChild(0).transform);
                    }
                }

                holdData.value -= 1; //これも同様

                if (holdData.value == 0)
                    holdData.key = null;


                if (isOpening)
                {
                    if (holdData.value == 0)
                    {
                        Destroy(fire_before.transform.GetChild(0).transform.GetChild(0).gameObject);
                        slots[0].itemHold = false;
                    }
                    else
                    {
                        slots[0].elemCounter.text =
                            (int.Parse(slots[0].elemCounter.text) - 1).ToString();
                    }
                }
            }
        }

        Off();
        OnCheckInBonfire();
    }

    private void Update()
    {
        if (isPlaying && isOpening)
        {
            fireGage.fillAmount = Mathf.InverseLerp(0, endValue, progress);
        }
    }

    void UpdateCoreData(Vector3Int data, bool isAdd)
    {
        ItemLibrary.ItemCountData tmpData = holdData;
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        holdData = tmpData;

        OnCheckInBonfire();
    }

    void UpdateCoreDataToFireEnd(Vector3Int data, bool isAdd)
    {
        // Debug.Log(nameof(UpdateCoreData));
        ItemLibrary.ItemCountData tmpData = holdData_end;
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        holdData_end = tmpData;

        OnCheckInBonfire();
    }

    private void UpdateCoreDataToFuel(Vector3Int data, bool isAdd)
    {
        // Debug.Log(nameof(UpdateCoreData));
        ItemLibrary.ItemCountData tmpData = fuelData;
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        fuelData = tmpData;

        OnCheckInBonfire();
    }

    void OnCheckInBonfire()
    {
        bool isfireObj = false;
        // if (fuelData.value > 0)
        // {
        if (holdData.key != null)
        {
            isfireObj = canFireObjList.ContainsKey(holdData.key);
        }
        // }

        if (isfireObj && (holdData_end.key == null || holdData_end.key == canFireObjList[holdData.key]) && !isPlaying)
            On();
        else
            fireGage.fillAmount = 0.0f;
    }

    private void endEvent()
    {
        isOpening = false;
        fireGage.fillAmount = 0.0f;
    }

    void Off()
    {
        isPlaying = false;
        particle.Stop();
    }

    public void OnAfterSave()
    {
        tuple_beforeSave = new[]
        {
            holdData,
            holdData_end,
            fuelData
        };

        tuple_afterSave = new[]
        {
            holdData_jsonData,
            holdData_end_jsonData,
            fuelData_jsonData
        };

        for (int i = 0; i < 3; i++)
        {
            SaveManager.ListSaveType tmpData = tuple_afterSave[i];
            tmpData.itemID = tuple_beforeSave[i].key != null ? tuple_beforeSave[i].key.GetItemID() : 0;
            tmpData.value = tuple_beforeSave[i].value;
            tuple_afterSave[i] = tmpData;
        }

        holdData_jsonData = tuple_afterSave[0];
        holdData_end_jsonData = tuple_afterSave[1];
        fuelData_jsonData = tuple_afterSave[2];
    }

    public void OnBeforeLoad()
    {
        holdData = new ItemLibrary.ItemCountData(ItemLibrary.Instance.FindItems(holdData_jsonData.itemID),
            holdData_jsonData.value);
        holdData_end = new ItemLibrary.ItemCountData(ItemLibrary.Instance.FindItems(holdData_end_jsonData.itemID),
            holdData_end_jsonData.value);
        fuelData = new ItemLibrary.ItemCountData(ItemLibrary.Instance.FindItems(fuelData_jsonData.itemID),
            fuelData_jsonData.value);
    }

    public string GetObjectData()
    {
        OnAfterSave();

        BonfireSaveType saveInstance = new BonfireSaveType
        {
            holdData_jsonData = this.holdData_jsonData,
            holdData_end_jsonData = this.holdData_end_jsonData,
            fuelData_jsonData = this.fuelData_jsonData
        };

        return JsonUtility.ToJson(saveInstance, false);
    }
}