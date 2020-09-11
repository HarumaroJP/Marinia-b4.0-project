using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : SingletonMonoBehaviour<InGameMenu>
{
    [SerializeField] private EventForDeveloper EventForDeveloper;
    [SerializeField] private ToolSetter toolSetter;
    [SerializeField] private Control control;
    [SerializeField] private Option option;
    [SerializeField] private Camera overlayCamera;
    [SerializeField] private Image[] slotImages;
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject ItemGroup;
    [SerializeField] private GameObject ItemGroupBackGround;
    [SerializeField] private GameObject reticule;
    [SerializeField] private GameObject overlay;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] ObjectUIs;
    [SerializeField] private Image[] TabImages;
    [SerializeField] private GameObject[] TabUIElements;
    [SerializeField] private Sprite[] TabImages_ON;
    [SerializeField] private Sprite[] TabImages_OFF;
    [SerializeField] private Color highlight_color;
    [SerializeField] Image FadePanel;

    [Header("UI")] [SerializeField] private float down_time;

    [Header("アイテムスロット")] [SerializeField] private int scrollSensibility;
    [SerializeField] private float scrollData;
    [SerializeField] public int selectingValue;

    [Header("イベント実行中か")] public bool inAnyMode = true;
    public bool isHitting;


    string Input_str;
    int nowPoint;
    int tmpValue;


    void Start()
    {
        // Canvas.ForceUpdateCanvases(); //キャンバスの強制更新。相対的な親サイズ取得のため。
        slotImages[0].color = highlight_color;
        tmpImage = slotImages[selectingValue];
    }

    public void InitializeAllMenus()
    {
        TabUIElements[1].SetActive(false);
        TabUIElements[2].SetActive(false);
        MainMenu.SetActive(false);
        SetItemGroup(false);
        ItemGroupBackGround.SetActive(false);

        foreach (GameObject obj in ObjectUIs)
        {
            obj.SetActive(false);
        }

        // Debug.Log("Menu Initialized");
    }

    void Update()
    {
        // Debug.Log(Input.inputString);
        KeyCheck();
    }

    private int tmpIndex = 0;

    public void TabChange(int index)
    {
        if (tmpIndex != index)
        {
            TabImages[tmpIndex].sprite = TabImages_OFF[tmpIndex];
            TabUIElements[tmpIndex].SetActive(false);
        }

        TabImages[index].sprite = TabImages_ON[index];
        TabUIElements[index].SetActive(true);
        tmpIndex = index;
    }

    private int openingIndex;

    public void OnSimpleMenuOpen(int value, bool isItemGroupOpen)
    {
        openingIndex = value;
        SetItemGroup(isItemGroupOpen);
        ItemGroupBackGround.SetActive(isItemGroupOpen);
        if (value != -1)
            ObjectUIs[value].SetActive(true);
    }

    public void OnSimpleMenuClose(bool isGameStart)
    {
        if (isGameStart) OffUI(false);

        SetItemGroup(false);
        ItemGroupBackGround.SetActive(false);
        if (openingIndex != -1)
            ObjectUIs[openingIndex].SetActive(false);
    }

    public void OnSimpleMenuClose(int value, bool isGameStart)
    {
        if (isGameStart) OffUI(false);

        SetItemGroup(false);
        ItemGroupBackGround.SetActive(false);
        if (value != -1)
            ObjectUIs[value].SetActive(false);
    }

    public void OnMenuOpen()
    {
        TabChange(0);
        MainMenu.SetActive(true);
        SetItemGroup(true);
        OnUI(false);

        option.OnEnablePanel();
    }

    public void OnMenuClose()
    {
        option.OnDisablePanel();
        MainMenu.SetActive(false);
        SetItemGroup(false);
        OffUI(false);
    }

    public void SetItemGroup(bool value)
    {
        ItemGroup.SetActive(value);
    }

    void KeyCheck()
    {
        if (inAnyMode)
        {
            switch (Input.inputString)
            {
                case "e":
                case "E":
                    Input_str = Input.inputString;
                    OnMenuOpen();
                    // ItemLibrary.LoadItems();
                    break;

                case "f":
                case "F":
                    ItemData holdItem = ItemLibrary.Instance.itemSlotIDList[toolSetter.activeIndex].key;
                    if (holdItem != null && !isHitting)
                    {
                        GameObject obj = Marinia.Find(holdItem.GetFileName(), Marinia.ItemType.Build);
                        if (obj != null)
                        {
                            Input_str = Input.inputString;
                            toolSetter.InitializedSetMode(holdItem, obj, holdItem.GetItemSetType());
                            inAnyMode = false;
                        }
                    }

                    break;

                case "/":
                    Input_str = Input.inputString;
                    EventForDeveloper.OpenCommandLine();
                    OnUI(false);
                    break;

                default:
                    // Debug.Log(Input_str);
                    break;
            }

            MoveSlot();
        }
        else if (!inAnyMode)
        {
            if (String.Compare(Input.inputString, Input_str, StringComparison.OrdinalIgnoreCase) != 0) return;
            switch (Input.inputString)
            {
                case "e":
                case "E":
                    OnMenuClose();
                    break;

                case "f":
                case "F":
                    Input_str = Input.inputString;
                    toolSetter.EndSetMode(false);
                    inAnyMode = true;
                    break;

                case "/":
                    EventForDeveloper.CloseCommandLine();
                    OffUI(false);
                    break;

                default:
                    break;
            }

            Input_str = "";
        }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     SceneManager.LoadScene("Title");
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        // }
    }

    private Image tmpImage;
    private int idleHash = Animator.StringToHash("Idle");
    private Animator toolAnimator;

    private void MoveSlot()
    {
        // ItemData data = ItemLibrary.Instance.itemSlotIDList[selectingValue].key;
        // if (ValueHasChanged(selectingValue, 1))
        // {
        //     if (data != null && data.GetItemClass() == "Tool")
        //         toolAnimator = toolSetter.itemSlotObjList[selectingValue].GetComponent<Animator>();
        //     else
        //         toolAnimator = null;
        // }
        //
        // Debug.Log(toolAnimator != null);
        // Debug.Log(toolAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != idleHash);
        // if (toolAnimator != null && toolAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != idleHash) return;

        scrollData += Input.GetAxis("Mouse ScrollWheel") * scrollSensibility;
        // int.TryParse(Input.inputString, out int tmpNum);
        // if (tmpNum < 6)
        // {
        //     slotImages[tmpNum].color = highlight_color;
        //     nowPoint = selectingValue = tmpNum;
        // }

        if (scrollData != 0)
        {
            int setValue = Mathf.FloorToInt(Mathf.Repeat(scrollData, 5));

            selectingValue = setValue;
            if (selectingValue >= 5)
            {
                selectingValue = 4;
            }
        }

        if (scrollData >= 5f)
            scrollData = 0f;

        // Debug.Log(selectingValue);

        if (ValueHasChanged(selectingValue))
        {
            tmpImage.color = Vector4.one;
            slotImages[selectingValue].color = highlight_color;
            tmpImage = slotImages[selectingValue];
            toolSetter.ChangeObjectOnSlot(selectingValue);
        }


        // Debug.Log(scrollData);
    }

    bool ValueHasChanged(int value)
    {
        bool re = tmpValue != value;
        tmpValue = value;
        return re;
    }

    public enum FadeType
    {
        Normal,
        Quick,
        Wave
    }

    public void OnFadeDown(bool isFade, FadeType type)
    {
        FadePanel.gameObject.SetActive(true);

        switch (type)
        {
            case FadeType.Normal:
                Debug.Log("OnFadeDown" + isFade);
                FadePanel.color = new Color(0f, 0f, 0f, !isFade ? 0f : 1f);
                FadePanel.DOFade(isFade ? 0f : 1f, 4f).OnComplete(() =>
                {
                    if (isFade)
                        FadePanel.gameObject.SetActive(false);
                }).Play().SetUpdate(true);
                break;

            case FadeType.Quick:
                FadePanel.color = new Color(0f, 0f, 0f, !isFade ? 0f : 1f);
                if (!isFade)
                    FadePanel.gameObject.SetActive(false);
                break;

            case FadeType.Wave:
                FadePanel.DOFade(0f, 4f).SetEase(Ease.Flash)
                    .OnComplete(() => { FadePanel.gameObject.SetActive(false); }).Play();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void OnUI(bool timeLerp)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (timeLerp)
            DOTween.To(() => Time.timeScale, t => Time.timeScale = t, 0f, 1f).Play().SetUpdate(true);
        else
            Time.timeScale = 0f;
        inAnyMode = false;
        control.isPlaying = false;
    }

    public void OffUI(bool timeLerp)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (timeLerp)
            DOTween.To(() => Time.timeScale, t => Time.timeScale = t, 1f, 1f).Play().SetUpdate(true);
        else
            Time.timeScale = 1f;
        inAnyMode = true;
        control.isPlaying = true;
    }

    public void OnUIWithoutTimeScale()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inAnyMode = false;
        control.isPlaying = false;
    }

    public void OffUIWithoutTimeScale()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inAnyMode = true;
        control.isPlaying = true;
    }

    public TweenCallback SetOverlay(bool lerp, bool trigger)
    {
        float endValue = trigger ? 1f : 0f;
        if (lerp)
        {
            canvasGroup.DOFade(endValue, 1f).Play().SetUpdate(true).OnStart(() =>
            {
                if (trigger)
                {
                    overlayCamera.enabled = true;
                    reticule.SetActive(true);
                    overlay.SetActive(true);
                }
            }).OnComplete(() =>
            {
                if (!trigger)
                {
                    overlayCamera.enabled = false;
                    reticule.SetActive(false);
                    overlay.SetActive(false);
                }
            });
        }
        else
        {
            canvasGroup.DOFade(endValue, 0f).Play();
            overlayCamera.enabled = false;
            reticule.SetActive(false);
            overlay.SetActive(false);
        }

        return null;
    }
}