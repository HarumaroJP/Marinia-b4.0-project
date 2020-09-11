using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class Furnace : MonoBehaviour, IUsable
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject FurnaceUI;
    [SerializeField] private MachineManager manager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private Image fireGage;
    [SerializeField] private Button endButton;
    [SerializeField] private bool isAlreadyPlace; //Developing Only
    [SerializeField] private bool isPlaying;
    [SerializeField] private bool isOpening;
    [SerializeField] private List<GameObject> fire_before;
    [SerializeField] private List<GameObject> fire_after;
    [SerializeField] private List<GameObject> fuels;
    [SerializeField] private List<ItemLibrary.ItemCountData> holdData;
    [SerializeField] private List<ItemLibrary.ItemCountData> holdData_end;
    [SerializeField] private List<ItemLibrary.ItemCountData> fuelData;
    [SerializeField] private ItemLibrary.canFireList[] canFireObjList_tmp;
    [SerializeField] private List<SaveManager.ListSaveType> holdData_jsonData;
    [SerializeField] private List<SaveManager.ListSaveType> holdData_end_jsonData;
    [SerializeField] private List<SaveManager.ListSaveType> fuelData_jsonData;
    private List<ItemLibrary.ItemCountData>[] tuple_beforeSave;
    private List<SaveManager.ListSaveType>[] tuple_afterSave;


    private Dictionary<ItemData, ItemData> canFireObjList = new Dictionary<ItemData, ItemData>();

    private void Awake()
    {
        foreach (ItemLibrary.canFireList data in canFireObjList_tmp)
        {
            canFireObjList.Add(data.key, data.value);
        }

        GameObject mainMenu = GameObject.FindWithTag("MainMenu");
        FurnaceUI = mainMenu.transform.Find("Furnace").gameObject;
        endButton = mainMenu.transform.Find("ItemGroupBackGround").GetChild(0).GetComponent<Button>();
        manager = FurnaceUI.GetComponent<MachineManager>();
        fireGage = FurnaceUI.transform.GetChild(4).GetComponent<Image>();

        itemManager.getData = GetObjectData;
        itemManager.setSaveData = LoadSaveData;

        Transform trans = FurnaceUI.transform.GetChild(5).GetChild(0);
        foreach (Transform t in trans)
        {
            fire_before.Add(t.gameObject);
        }

        trans = FurnaceUI.transform.GetChild(5).GetChild(1);
        foreach (Transform t in trans)
        {
            fire_after.Add(t.gameObject);
        }

        trans = FurnaceUI.transform.GetChild(5).GetChild(2);
        foreach (Transform t in trans)
        {
            fuels.Add(t.gameObject);
        }

        holdData = Enumerable.Repeat(new ItemLibrary.ItemCountData(null, 0), 3).ToList();
        holdData_end = Enumerable.Repeat(new ItemLibrary.ItemCountData(null, 0), 3).ToList();
        fuelData = Enumerable.Repeat(new ItemLibrary.ItemCountData(null, 0), 2).ToList();

        holdData_jsonData = Enumerable.Repeat(new SaveManager.ListSaveType(null, 0), 3).ToList();
        holdData_end_jsonData = Enumerable.Repeat(new SaveManager.ListSaveType(null, 0), 3).ToList();
        fuelData_jsonData = Enumerable.Repeat(new SaveManager.ListSaveType(null, 0), 2).ToList();

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
            FurnaceSaveType saveClassData = JsonUtility.FromJson<FurnaceSaveType>(jsonData);

            holdData_jsonData = saveClassData.holdData_jsonData;
            holdData_end_jsonData = saveClassData.holdData_end_jsonData;
            fuelData_jsonData = saveClassData.fuelData_jsonData;
        }

        OnBeforeLoad();
        OnCheckInFurnace();
    }

    public void Initialize()
    {
        InGameMenu.Instance.OnUI(false);
        InGameMenu.Instance.OnSimpleMenuOpen(0, true);

        isOpening = true;
        InitializeCoreData();

        endButton.onClick.RemoveListener(endEvent);
        endButton.onClick.AddListener(endEvent);

        foreach (ItemSlotScript slot in manager.slots_before)
        {
            slot.initializeEvent.RemoveAllListeners();
            slot.initializeEvent.AddListener(UpdateCoreData);
        }

        foreach (ItemSlotScript slot in manager.slots_after)
        {
            slot.initializeEvent.RemoveAllListeners();
            slot.initializeEvent.AddListener(UpdateCoreDataToFireEnd);
        }

        foreach (ItemSlotScript slot in manager.fuels)
        {
            slot.initializeEvent.RemoveAllListeners();
            slot.initializeEvent.AddListener(UpdateCoreDataToFuel);
        }

        OnCheckInFurnace();
    }


    private void OnCheckInFurnace()
    {
        bool flag = false;
        bool isfireObj = false;
        int counter = 0;
        if (fuelData.Count > 0)
        {
            foreach (ItemLibrary.ItemCountData data in holdData)
            {
                if (data.key == null) continue;

                if (canFireObjList.ContainsKey(data.key))
                {
                    flag = holdData_end.Any(elem => elem.key == canFireObjList[data.key]);
                    isfireObj = true;
                }
            }

            counter = holdData_end.Count(data => data.key != null);

            // IEnumerable<bool> flag =
            //     from data in holdData.Keys
            //     from data_end in holdData_end.Keys
            //     select data == data_end;
        } //かまどの中がパンクしてないか

        if (isfireObj && (flag || counter < 3) && !isPlaying)
            On();
        else
            fireGage.fillAmount = 0.0f;
    }

    private float progress;
    private float endValue;
    private Tweener fireLoop;

    private void On()
    {
        endValue = 30;
        fireLoop = DOTween.To(x => progress = x, 0, endValue, 2f).OnComplete(UpdateCore).SetUpdate(true);
        fireLoop.Play();
        particle.Play();
        isPlaying = true;
    }

    private void Update()
    {
        if (isPlaying && isOpening)
        {
            fireGage.fillAmount = Mathf.InverseLerp(0, 30, progress);
        }
    }


    public void UpdateCoreData(Vector3Int data, bool isAdd)
    {
// Debug.Log(nameof(UpdateCoreData));
        ItemLibrary.ItemCountData tmpData = holdData[data.z];
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        holdData[data.z] = tmpData;
        OnCheckInFurnace();
    }

    public void UpdateCoreDataToFireEnd(Vector3Int data, bool isAdd)
    {
        ItemLibrary.ItemCountData tmpData = holdData_end[data.z];
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        holdData_end[data.z] = tmpData;
        OnCheckInFurnace();
    }

    public void UpdateCoreDataToFuel(Vector3Int data, bool isAdd)
    {
        ItemLibrary.ItemCountData tmpData = fuelData[data.z];
        tmpData.value += isAdd ? data.y : -data.y;
        tmpData.key = tmpData.value == 0 ? null : ItemLibrary.Instance.FindItems(data.x);
        fuelData[data.z] = tmpData;
        OnCheckInFurnace();
    }

    public void InitializeCoreData()
    {
        int index = 0;
        foreach (GameObject tGameObject in fire_before)
        {
            Transform Trans = tGameObject.transform.GetChild(0);
            if (Trans.childCount > 0)
                Destroy(Trans.GetChild(0).gameObject);

            ItemSlotScript slot = Trans.GetComponent<ItemSlotScript>();
            slot.itemHold = holdData[index].key != null;
            ItemLibrary.Instance.InstantiateItems(holdData[index].key, holdData[index].value, Trans);
            index++;
        }

        index = 0;
        foreach (GameObject tGameObject in fire_after)
        {
            Transform Trans = tGameObject.transform.GetChild(0);
            if (Trans.childCount > 0)
                Destroy(Trans.GetChild(0).gameObject);

            ItemSlotScript slot = Trans.GetComponent<ItemSlotScript>();
            slot.itemHold = holdData[index].key != null;
            ItemLibrary.Instance.InstantiateItems(holdData_end[index].key, holdData_end[index].value, Trans);
            index++;
        }

        index = 0;
        foreach (GameObject tGameObject in fuels)
        {
            Transform Trans = tGameObject.transform.GetChild(0);
            if (Trans.childCount > 0)
                Destroy(Trans.GetChild(0).gameObject);

            ItemSlotScript slot = Trans.GetComponent<ItemSlotScript>();
            slot.itemHold = holdData[index].key != null;
            ItemLibrary.Instance.InstantiateItems(fuelData[index].key, fuelData[index].value, Trans);
            index++;
        }
    }

    void UpdateCore()
    {
        for (int index = 0;
            index < holdData.Count;
            index++)
        {
            ItemLibrary.ItemCountData data = holdData[index];
            if (data.key == null) break;
            if (canFireObjList.ContainsKey(data.key))
            {
                ItemData firedData = canFireObjList[data.key];
                int index_after = holdData_end.FindIndex(t => t.key == firedData);
                if (index_after != -1)
                {
                    ItemLibrary.ItemCountData hold_tmp = holdData_end[index_after];
                    hold_tmp.value += 1; //レベルによって変更する可能性あり
                    holdData_end[index_after] = hold_tmp;

                    if (isOpening)
                    {
                        ItemSlotScript itemSlotScript_after =
                            fire_after[index_after].GetComponentInChildren<ItemSlotScript>();
                        itemSlotScript_after.elemCounter.text =
                            (int.Parse(itemSlotScript_after.elemCounter.text) + 1).ToString();
                    }
                }
                else
                {
                    int index_create = holdData_end.FindIndex(t => t.value == 0);
                    ItemLibrary.ItemCountData hold_tmp = holdData_end[index_create];
                    hold_tmp.key = firedData;
                    hold_tmp.value += 1; //レベルによって変更する可能性あり
                    holdData_end[index_create] = hold_tmp;

                    if (isOpening)
                    {
                        ItemLibrary.Instance.InstantiateItems(firedData, 1,
                            fire_after[index_create].transform.GetChild(0).transform);
                    }
                }

                int index_before = holdData.FindIndex(t => t.key == data.key);
                ItemLibrary.ItemCountData tmpData = holdData[index_before];
                tmpData.value -= 1; //これも同様

                if (tmpData.value == 0)
                    tmpData.key = null;


                if (isOpening)
                {
                    ItemSlotScript itemSlotScript_before =
                        fire_before[index_before].GetComponentInChildren<ItemSlotScript>();

                    if (tmpData.value == 0)
                    {
                        Destroy(fire_before[index_before].transform.GetChild(0).transform.GetChild(0).gameObject);
                        itemSlotScript_before.itemHold = false;
                    }
                    else
                    {
                        itemSlotScript_before.elemCounter.text =
                            (int.Parse(itemSlotScript_before.elemCounter.text) - 1).ToString();
                    }
                }

                holdData[index_before] = tmpData;
            }
        }

        Off();
        OnCheckInFurnace();
    }

    private void endEvent()
    {
        isOpening = false;
        fireGage.fillAmount = 0.0f;
    }

    private void Off()
    {
        isPlaying = false;
        particle.Stop();
    }

    public void OnAfterSave()
    {
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < tuple_afterSave[j].Count; i++)
            {
                SaveManager.ListSaveType tmpData = tuple_afterSave[j][i];
                tmpData.itemID = tuple_beforeSave[j][i].key != null ? tuple_beforeSave[j][i].key.GetItemID() : 0;
                tmpData.value = tuple_beforeSave[j][i].value;
                tuple_afterSave[j][i] = tmpData;
            }
        }
    }

    public void OnBeforeLoad()
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

        for (int j = 0; j < 3; j++)
        {
            int tmpIndex = 0;
            foreach (SaveManager.ListSaveType data in tuple_afterSave[j])
            {
                tuple_beforeSave[j][tmpIndex] =
                    new ItemLibrary.ItemCountData(ItemLibrary.Instance.FindItems(data.itemID), data.value);
                tmpIndex++;
            }
        }
    }

    public string GetObjectData()
    {
        OnAfterSave();
        FurnaceSaveType saveInstance = new FurnaceSaveType
        {
            holdData_jsonData = this.holdData_jsonData,
            holdData_end_jsonData = this.holdData_end_jsonData,
            fuelData_jsonData = this.fuelData_jsonData
        };


        return JsonUtility.ToJson(saveInstance, false);
    }
}