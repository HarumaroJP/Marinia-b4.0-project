using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToolSetter : MonoBehaviour
{
    private Transform SetObject;
    private Rigidbody SetObjectRig;
    private Collider[] SetObjectCol;
    [SerializeField] private List<Renderer> SetObjectRen;
    private RigCleaner rigCleaner;
    private List<MaterialSaveType> saveMaterials = new List<MaterialSaveType>();
    [SerializeField] private Material material_can;
    [SerializeField] private Material material_cant;
    [SerializeField] public Transform SelectingSpace;
    [SerializeField] private Transform playerObjectSpace;
    [SerializeField] private GameObject originObj;
    [SerializeField] private WorkbenchManager workbenchManager;
    [SerializeField] private MemoGroup memoGroup;
    [SerializeField] private Vector2 inSettingScrollSensi;
    [SerializeField] private float rayDistance;
    [SerializeField] private float rayDistance_setting;
    [SerializeField] private int errorRange;
    [SerializeField] private Image keyPanelImage;
    [SerializeField] private Image secKeyPanelImage;
    [SerializeField] private Sprite[] keySpriteList;
    [SerializeField] public int activeIndex = 0;
    [SerializeField] private int beforeIndex;
    [SerializeField] public GameObject[] itemSlotObjList = new GameObject[5];
    private Ray rayForSet;

    private Transform cam;
    public RaycastHit hitObj;
    bool inSetting = false;
    private Transform holdObj;
    private GameObject tmpObj;


    [Serializable]
    public struct MaterialSaveType
    {
        public Renderer renderer;
        public List<Material> material;

        public MaterialSaveType(Renderer renderer, List<Material> material)
        {
            this.renderer = renderer;
            this.material = material;
        }
    }

    void Start()
    {
        cam = Camera.main.transform;
    }

    private float tmpOFF = 0;
    private KeyCode checkKey;
    private int checkMouseButton;
    private bool isKey;
    private Material[] tmpMaterials;
    private UnityEvent setEvent = new UnityEvent();

    void Update()
    {
        if (inSetting)
        {
            setEvent.Invoke();

            if (Input.GetMouseButtonDown(1))
            {
                if (canPlaceResult)
                {
                    if (SetObjectRig != null)
                        SetObjectRig.isKinematic = false;

                    foreach (Collider col in SetObjectCol)
                    {
                        col.enabled = true;
                    }

                    SetLayer(SetObject, 0, true);

                    SetObject.SetParent(playerObjectSpace);
                }

                EndSetMode(canPlaceResult);
            }

            // Debug.Log("originObj : " + originObj.transform.eulerAngles);
            // Debug.Log("gameObject : " + cam.transform.eulerAngles);
        }
        else
        {
            if (CanTake()) //最初にオブジェクトを検出したときのみ true
            {
                ChangedLookObject(true);
            }
            else //オブジェクトの変更がない限り false
            {
                if (hitting == null)
                {
                    tmpObj = null;
                    return;
                }

                if (!InGameMenu.Instance.inAnyMode) return;

                if (CheckInputDown(isKey))
                {
                    Debug.Log("CheckInputDown");
                    CheckingKeyDown(tmpItemID);
                    // OffDisplayKeyPanel();
                }

                if (CheckInputUp(isKey))
                {
                    CheckingKeyUp(tmpItemID);
                    // TakeItemsByID(tmpItemID);
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    GetItems(hitting);
                    OffDisplayKeyPanel();
                }
            }
        }
    }

    private int layerMask_canTake = ~(1 << 10);

    private bool CanTake()
    {
        rayForSet = new Ray(cam.position, cam.TransformDirection(Vector3.forward));
        if (Physics.Raycast(rayForSet, out hitObj, rayDistance, layerMask_canTake))
        {
            GameObject obj = hitObj.transform.gameObject;
            bool result = tmpObj != obj;
            tmpObj = obj;
            return result;
        }
        else
        {
            // Debug.Log(tmpObj);
            // Debug.Log(hitting);
            if (tmpObj != null && hitting != null)
            {
                OffDisplayKeyPanel();
                SetLayer(hitting, 0, changeChild);
                InitializeTakingItemsByID(tmpItemID, true);
                tmpObj = null;
                tmpItemData = null;
            }

            return false;
        }
    }


    private bool tmpBoolean;

    private bool BoolHasChanged(bool value)
    {
        bool re = tmpBoolean != value;
        tmpBoolean = value;
        return re;
    }

    private void SetPlaceMaterial(bool canPlace)
    {
        foreach (Renderer renderer in SetObjectRen)
        {
            tmpMaterials = renderer.materials;
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                tmpMaterials[i] = canPlace ? material_can : material_cant;
            }

            renderer.materials = tmpMaterials;
        }
    }

    private void SetObject_Sea()
    {
        tmpOFF += Input.GetAxisRaw("Mouse ScrollWheel");
        tmpOFF = Mathf.Clamp(tmpOFF, inSettingScrollSensi.x, inSettingScrollSensi.y);

        Vector3 placePoint = rayForSet.GetPoint(rayDistance_setting);
        Vector3 camTransform = cam.transform.eulerAngles;
        Vector3 offset = cam.TransformVector(new Vector3(0f, 0f, tmpOFF * 2.5f));

        SetObject.SetPositionAndRotation(placePoint - offset, Quaternion.Euler(0f, camTransform.y, camTransform.z));
    }

    Vector3 placePoint;

    private void SetObject_Ground()
    {
        placePoint = hitObj.point + new Vector3(0f, 0.5f, 0f);
        Vector3 camTransform = cam.transform.eulerAngles;
        SetObject.SetPositionAndRotation(placePoint, Quaternion.Euler(0f, camTransform.y, camTransform.z));
    }

    private Transform hitting;
    private int tmpItemID;
    private ItemData tmpItemData;
    private bool init_check;
    private bool changeChild;

    private void ChangedLookObject(bool isDirectionChange)
    {
        // Debug.Log(hitting);
        // Debug.Log(init_check);
        if (hitting != null && init_check)
        {
            SetLayer(hitting, 0, changeChild);
            OffDisplayKeyPanel();
            InitializeTakingItemsByID(tmpItemID, isDirectionChange);
        }

        hitting = hitObj.collider != null ? hitObj.collider.transform : null;

        if (ReferenceEquals(hitting, null)) return;

        if (hitting.CompareTag("CanTake"))
        {
            // Debug.Log(hitObj);
            // Debug.Log(hitting);
            // Debug.Log(hitting.GetComponent<ItemManager>());
            tmpItemID = hitting.GetComponent<ItemManager>().itemID;
            // Debug.Log(tmpItemID);
            InGameMenu.Instance.isHitting = true;
            tmpItemData = ItemLibrary.Instance.FindItems(tmpItemID);
            init_check = tmpItemData.CanDestroyFromAll() || ItemLibrary.Instance.CanUseTool(tmpItemData, activeIndex);
            if (init_check)
            {
                TakeItemsByID(tmpItemID);
                SetLayer(hitting, 9, changeChild);
            }

            return;
        }

        if (hitting.CompareTag("DropBox"))
        {
            DisplayKeyPanel(KeyCode.F);
            init_check = true;
            return;
        }

        if (hitting.CompareTag("Core"))
        {
            DisplayKeyPanel(KeyCode.Mouse0);
            init_check = true;
            return;
        }

        tmpItemID = 0;
        init_check = false;
        InGameMenu.Instance.isHitting = false;
    }

    private void TakeItemsByID(int id)
    {
        ItemData itemData = ItemLibrary.Instance.FindItems(id);
        (KeyCode[], bool) keyInfo = itemData.GetTakeKeyInfo();

        if (keyInfo.Item1.Length <= 0) return;

        foreach (KeyCode keyCode in keyInfo.Item1)
        {
            DisplayKeyPanel(keyCode);
        }

        changeChild = keyInfo.Item2;

        if (itemSlotObjList[activeIndex] != null)
            itemSlotObjList[activeIndex].GetComponent<IToolable>()?.SetTool();
    }


    private bool CheckInputDown(bool isKey)
    {
        return isKey ? Input.GetKeyDown(checkKey) : Input.GetMouseButtonDown(checkMouseButton);
    }

    private bool CheckInputUp(bool isKey)
    {
        return isKey ? Input.GetKeyUp(checkKey) : Input.GetMouseButtonUp(checkMouseButton);
    }

    private bool tmpDestroyCheck;

    private void CheckingKeyDown(int id)
    {
        // Debug.Log(id);
        tmpDestroyCheck = tmpItemData != null &&
                          (tmpItemData.CanDestroyFromAll() ||
                           ItemLibrary.Instance.CanUseTool(tmpItemData, activeIndex));

        if (tmpDestroyCheck)
        {
            switch (id)
            {
                case 1:
                case 2:
                case 14:
                    hitting.GetComponentInChildren<ParticleSystem>()?.Play();
                    itemSlotObjList[activeIndex].GetComponent<PickAxe>().BreakRock(hitting);
                    break;

                case 16:
                    Transform particleSystem = hitting.transform.Find("Particle");
                    particleSystem.GetComponent<ParticleSystem>()?.Play();
                    itemSlotObjList[activeIndex].GetComponent<Axe>().BreakWood(hitting);
                    itemSlotObjList[activeIndex].GetComponent<Axe>().SetTool();
                    break;

                default:
                    break;
            }
        }

        Debug.Log(hitting);
        if (hitting.GetComponent<ItemManager>() == null) return;
        hitting.GetComponent<ItemManager>().parentObj.GetComponent<IUsable>()?.Initialize();
    }

    private void CheckingKeyUp(int id)
    {
        switch (id)
        {
            case 1:
            case 2:
            case 14:
                if (tmpDestroyCheck)
                {
                    hitting.GetComponentInChildren<ParticleSystem>()?.Stop();
                    itemSlotObjList[activeIndex].GetComponent<PickAxe>()?.CancelBreak();
                }

                break;
            case 16:
                if (tmpDestroyCheck)
                {
                    hitting.transform.GetChild(0)?.GetComponent<ParticleSystem>()?.Stop();
                    itemSlotObjList[activeIndex].GetComponent<Axe>()?.CancelBreak();
                }

                break;
        }
    }


    private void InitializeTakingItemsByID(int id, bool isDirectionChange)
    {
        int resultIndex = isDirectionChange ? activeIndex : beforeIndex;

        if (itemSlotObjList[resultIndex] == null) return;
        tmpDestroyCheck = tmpItemData != null &&
                          (tmpItemData.CanDestroyFromAll() ||
                           ItemLibrary.Instance.CanUseTool(tmpItemData, resultIndex));

        if (!tmpDestroyCheck) return;
        switch (id)
        {
            case 1:
            case 2:
            case 14:
                // Debug.Log("initialize");
                itemSlotObjList[resultIndex]?.GetComponent<PickAxe>().InitializeTool();
                hitting.GetComponentInChildren<ParticleSystem>()?.Stop();
                itemSlotObjList[resultIndex]?.GetComponent<PickAxe>().CancelBreak();
                break;

            case 16:
                itemSlotObjList[resultIndex]?.GetComponent<Axe>().InitializeTool();

                ParticleSystem tmp = hitting.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (tmp != null)
                {
                    tmp.Stop();
                }

                itemSlotObjList[resultIndex]?.GetComponent<Axe>().CancelBreak();
                break;
            default:
                break;
        }
    }

    private void GetItems(Transform obj)
    {
        if (!hitting.CompareTag("CanTake")) return;
        int itemID = obj.GetComponent<ItemManager>().itemID;
        if (itemID == 20)
        {
            Memo memo = obj.GetComponent<Memo>();
            memoGroup.SetMemoElement(memo.memoID);
        }
        else
        {
            ItemLibrary.Instance.AddItemsForMenu(ItemLibrary.Instance.FindItems(itemID), 1);
        }

        Destroy(obj.GetComponent<ItemManager>().parentObj);
    }

    [SerializeField] private Collider[] setting_HitChecker = new Collider[10];
    private (Vector3, Vector3) overlapData;
    private const int layerMask = ~(1 << 2 | 1 << 4 | 1 << 10);

    public void InitializedSetMode(ItemData data, GameObject obj, string type)
    {
        inSetting = true;
        tmpOFF = 0;
        SetObject = Instantiate(obj).transform;
        SetObjectRig = SetObject.GetComponent<Rigidbody>();
        SetObjectCol = SetObject.GetComponentsInChildren<Collider>();
        SetObjectRen = SetObject.GetComponentsInChildren<Renderer>().ToList();
        rigCleaner = SetObject.GetComponentInChildren<RigCleaner>();
        overlapData = data.GetOverlapData();
        SetLayer(SetObject, 0, changeChild);

        if (saveMaterials.Count > 0)
        {
            saveMaterials.Clear();
        }

        foreach (Collider col in SetObjectCol)
        {
            col.enabled = false;
        }

        foreach (Renderer renderer in SetObjectRen)
        {
            saveMaterials.Add(new MaterialSaveType(renderer, renderer.materials.ToList()));
        }

        setEvent.RemoveAllListeners();
        switch (type)
        {
            case "Sea":
                setEvent.AddListener(PlaceOnSea);
                break;

            case "Ground":
                SetPlaceMaterial(CanPlace_Ground());
                setEvent.AddListener(PlaceOnGround);
                break;

            default:
                break;
        }
    }

    public void EndSetMode(bool isSet)
    {
        // Debug.Log(ItemLibrary.FindItems(ItemLibrary.itemSlotIDList[activeIndex]));
        InGameMenu.Instance.inAnyMode = true;
        inSetting = false;

        if (!isSet)
            Destroy(SetObject.gameObject);
        else
        {
            foreach (MaterialSaveType saveType in saveMaterials)
            {
                saveType.renderer.materials = saveType.material.ToArray();
            }


            if (rigCleaner != null) rigCleaner.CanClean();
            PoisonGas poisonGas = SetObject.GetComponentInChildren<PoisonGas>();
            if (poisonGas != null) poisonGas.Initialize();

            ItemLibrary.Instance.RemoveItemsForMenu(ItemLibrary.Instance.itemSlotIDList[activeIndex].key, 1);
        }
    }


    public void ChangeObjectOnSlot(int changeIndex)
    {
        if (ItemLibrary.Instance.itemSlotIDList[activeIndex].value != 0)
        {
            itemSlotObjList[activeIndex].GetComponentInChildren<IToolable>()?.ToolInit();
            itemSlotObjList[activeIndex].SetActive(false);
        }


        if (ItemLibrary.Instance.itemSlotIDList[changeIndex].value != 0)
            itemSlotObjList[changeIndex].SetActive(true);

        beforeIndex = activeIndex;
        activeIndex = changeIndex;

        // if (ItemLibrary.Instance.itemSlotIDList[activeIndex].value != 0)
        ChangedLookObject(false);
    }

    public bool canPlaceResult;

    private void PlaceOnSea()
    {
        canPlaceResult = CanPlace_Sea();
        if (BoolHasChanged(canPlaceResult))
        {
            SetPlaceMaterial(canPlaceResult);
        }

        SetObject_Sea();
    }

    private void PlaceOnGround()
    {
        SetObject_Ground();
        canPlaceResult = CanPlace_Ground();
        // Debug.Log(canPlaceResult);
        if (BoolHasChanged(canPlaceResult))
        {
            SetPlaceMaterial(canPlaceResult);
        }
    }

    private bool CanPlace_Sea()
    {
        Quaternion originObjTransform =
            Quaternion.LookRotation(cam.transform.position - originObj.transform.position, Vector3.up);
        originObjTransform.x = originObjTransform.z = 0f;
        originObj.transform.rotation = originObjTransform;

        Vector3 camWorldTransform = cam.TransformDirection(Vector3.forward);
        rayForSet = new Ray(cam.position, camWorldTransform);
        //For Debugging
        // playerLookLine.SetPositions(new Vector3[]
        //     {cam.position + new Vector3(0f, -1f, 0f), cam.position + rayForSet.direction * rayDistance});
        Debug.DrawRay(rayForSet.origin, camWorldTransform * rayDistance_setting, Color.red);

        // Debug.Log(targetDeg - errorRange + " : " + targetDeg + errorRange);
        // Debug.Log(cam.transform.eulerAngles.y + " : " +  originObj.transform.eulerAngles.y);

        float eulerAngles_cam = cam.eulerAngles.y;
        float eulerAngles_origin = originObj.transform.eulerAngles.y;
        return Physics.Raycast(rayForSet, out hitObj, rayDistance_setting) &&
               hitObj.collider.CompareTag("Wall") &&
               eulerAngles_origin - errorRange < eulerAngles_cam &&
               eulerAngles_cam < eulerAngles_origin + errorRange;
    }

    private bool CanPlace_Ground()
    {
        Physics.OverlapBoxNonAlloc(SetObject.position + overlapData.Item1, overlapData.Item2, setting_HitChecker,
            SetObject.rotation,
            layerMask);

        bool noHit = setting_HitChecker.Where(col => col != null).All(col => col.CompareTag("Floor"));
        Array.Clear(setting_HitChecker, 0, 10);
        // Debug.Log(noHit);

        Vector3 camWorldTransform = cam.TransformDirection(Vector3.forward);
        rayForSet = new Ray(cam.position, camWorldTransform);

        Debug.DrawRay(rayForSet.origin, camWorldTransform * rayDistance_setting, Color.red);
        return Physics.Raycast(rayForSet, out hitObj, rayDistance_setting) && hitObj.collider.CompareTag("Floor") &&
               noHit;
    }

    private void OnDrawGizmos()
    {
        if (SetObject != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(SetObject.position, SetObject.rotation, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(overlapData.Item1, overlapData.Item2 * 2);
        }
    }


    private void SetLayer(Transform obj, int layerNum, bool changeChild)
    {
        obj.gameObject.layer = layerNum;

        if (!changeChild) return;
        foreach (Transform childTransform in obj)
        {
            SetLayer(childTransform, layerNum, true);
        }
    }

    private void DisplayKeyPanel(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Mouse0:
                keyPanelImage.sprite = keySpriteList[0];
                checkMouseButton = 0;
                isKey = false;
                break;

            case KeyCode.Mouse1:
                keyPanelImage.sprite = keySpriteList[1];
                checkMouseButton = 1;
                isKey = false;
                break;

            case KeyCode.F:
                secKeyPanelImage.sprite = keySpriteList[2];
                secKeyPanelImage.enabled = true;
                // checkKey = KeyCode.F;
                // isKey = true;
                break;

            default:
                break;
        }

        keyPanelImage.enabled = true;
    }

    private void OffDisplayKeyPanel()
    {
        // Debug.Log("OffDisplayKeyPanel");
        keyPanelImage.enabled = false;
        secKeyPanelImage.enabled = false;
    }
}