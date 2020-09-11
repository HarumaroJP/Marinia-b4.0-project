using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

// ReSharper disable All

public class EventManager : MonoBehaviour
{
    private static UnityEvent callBack = new UnityEvent();

    // [SerializeField] private GameObject dollyObj;
    // [SerializeField] private Prologue prologue;
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private Image blinkImage;
    [SerializeField] private Image blackFadePanel;
    [SerializeField] private SkySystem skySystem;
    [SerializeField] private Transform magic_Circle;
    [SerializeField] private PhaseSystem phaseSystem;
    [SerializeField] private Control control;

    [Header("PostProcessing Parameter"), SerializeField]
    private float saturation_min;

    [SerializeField] private float saturation_max;
    [SerializeField] private float chromatic_min;
    [SerializeField] private float chromatic_max;
    [SerializeField] private List<UnityEvent> textPairEventList;

    [Header("EventCameraList")] [SerializeField]
    private Camera mainCamera;

    private CinemachineBrain mainCameraBrain;
    [SerializeField] List<CinemachineVirtualCamera> eventVirtualCameraList = new List<CinemachineVirtualCamera>();
    [SerializeField] List<Camera> eventDefaultCameraList = new List<Camera>();

    private ColorGrading colorGrading;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private readonly int _FillAmount = Shader.PropertyToID("_FillAmount");
    private readonly int _Height = Shader.PropertyToID("_Height");
    private int healing1;
    public bool isEventRunning;

    // [SerializeField] private string[] impact_text = Marinia.eventMessageList_impact.ToArray();
    // [SerializeField] private string[] player_text = Marinia.eventMessageList_player.ToArray();

    void Awake()
    {
        mainCamera = Camera.main;
        mainCameraBrain = mainCamera.GetComponent<CinemachineBrain>();
        GameObject eventGroup = GameObject.FindWithTag("EventManager");

        foreach (PostProcessEffectSettings settings in volume.profile.settings)
        {
            if (settings is ColorGrading grading) colorGrading = grading;
            else if (settings is Bloom blm) bloom = blm;
            else if (settings is ChromaticAberration aberration) chromaticAberration = aberration;
        }

        InGameMenu.Instance.SetOverlay(false, false);
        healing1 = AudioManager.Instance.GetBgmIndex("1_healing1");

        //TODO 初期実行イベントを書く
        blinkImage.material.SetFloat(_FillAmount, 0f);
        blinkImage.material.SetFloat(_Height, 0f);

        SetCamera(true);
        SetPriorityDefaultCamera(0);

        Prologue();
        // DebugInitialize();
        // Blink();
        // TalkEventInitializer();
    }

    private void Prologue()
    {
        skySystem.sun.intensity = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        control.isPlaying = false;
        ParticleSystem.MainModule mainModule = magic_Circle.root.GetComponentInChildren<ParticleSystem>().main;
        ParticleSystem.EmissionModule emissionModule =
            magic_Circle.root.GetComponentInChildren<ParticleSystem>().emission;

        AudioManager.Instance.PlayBgm(2, false);

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Quick));
        seq.AppendInterval(1f);
        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        seq.AppendInterval(3f);
        seq.Append(magic_Circle.DOScale(1f, 2f).SetEase(Ease.OutBack));
        seq.Join(magic_Circle.DOLocalRotate(
                new Vector3(magic_Circle.localRotation.eulerAngles.x, magic_Circle.localRotation.eulerAngles.y, 90f),
                2f)
            .SetEase(Ease.OutBack));
        seq.AppendInterval(3f);
        seq.AppendCallback(() =>
        {
            emissionModule.rateOverTime = 50f;
            mainModule.startSize = 0.6f;
        });
        seq.AppendInterval(3f);
        seq.AppendCallback(() =>
        {
            InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Quick);
            AudioManager.Instance.StopBgm();
        });
        seq.AppendInterval(2f);
        seq.Play().OnComplete(() =>
        {
            control.transform.position = new Vector3(150.35f, -20.57f, 152.69f);
            control.transform.rotation = Quaternion.Euler(0f, 212.462f, 0f);
            control.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(magic_Circle.root.gameObject);
            Blink();
            blackFadePanel.color = Color.clear;
            blackFadePanel.gameObject.SetActive(false);
            skySystem.StartSkySystem();
        });
    }

    private void DebugInitialize()
    {
        blackFadePanel.color = Color.clear;
        blackFadePanel.gameObject.SetActive(false);
        skySystem.StartSkySystem();
        control.transform.position = new Vector3(150.35f, -20.57f, 152.69f);
        control.transform.rotation = Quaternion.Euler(0f, 212.462f, 0f);
        control.GetComponent<Rigidbody>().isKinematic = false;
        Destroy(magic_Circle.root.gameObject);
        // InGameMenu.Instance.SetOverlay(true, false);
        // InGameMenu.Instance.OnUI(true);
    }


    private void TalkEventInitializer()
    {
        // Debug.Log(nameof(TalkEventInitializer));
        RunEvent(StartOpening);
    }

    public void RunEvent(UnityAction func)
    {
        if (!isEventRunning && phaseSystem.isFighting) return;
        InGameMenu.Instance.SetOverlay(true, false);
        InGameMenu.Instance.OnUI(true);
        InGameMenu.Instance.OnSimpleMenuOpen(5, false);
        InGameMenu.Instance.OnSimpleMenuOpen(6, false);
        callBack.RemoveAllListeners();
        callBack.AddListener(func);

        if (FlagCore.Instance.IsFlagComplete(FlagCore.Instance.GetCurrentEpisode().First()))
        {
            // Debug.Log(FlagCore.Instance.GetCurrentEpisode());
            foreach (int id in FlagCore.Instance.GetCurrentEpisode())
            {
                AddEvent(id);
            }

            // Debug.Log("flagComplete");

            FlagCore.Instance.currentGameProgress++;
        }
        else
        {
            AddEvent(FlagCore.Instance.GetBeforeEpisode().Last());
            // Debug.Log("NOTflagComplete");
        }

        SayMessageSystem.Instance.StartEvent();
    }


    public void RunEvent()
    {
        if (!isEventRunning && phaseSystem.isFighting) return;
        // Debug.Log("RunEvent");
        InGameMenu.Instance.SetOverlay(true, false);

        InGameMenu.Instance.OnUI(true);
        InGameMenu.Instance.OnSimpleMenuOpen(5, false);
        InGameMenu.Instance.OnSimpleMenuOpen(6, false);
        callBack.RemoveAllListeners();

        if (FlagCore.Instance.IsFlagComplete(FlagCore.Instance.GetCurrentEpisode().First()))
        {
            foreach (int id in FlagCore.Instance.GetCurrentEpisode())
            {
                AddEvent(id);
            }

            FlagCore.Instance.currentGameProgress++;
        }
        else
        {
            // Debug.Log(FlagCore.Instance.GetCurrentEpisode().Last());
            AddEvent(FlagCore.Instance.GetBeforeEpisode().Last());
        }


        SayMessageSystem.Instance.StartEvent();
        isEventRunning = true;
    }

    [SerializeField] private int count;
    public static int maxEventCount;

    public void MoveNextEvent()
    {
        // Debug.Log(nameof(MoveNextEvent) + "before");
        // Debug.Log(count);
        // Debug.Log(maxEventCount);
        //
        // Debug.Log(nameof(MoveNextEvent) + "after");
        // Debug.Log(count);

        Debug.Log(count);
        Debug.Log(FlagCore.Instance.callBackMethodActionList.Count);
        bool hasMethod = false;

        if (FlagCore.Instance.callBackMethodActionList.Count > 0)
        {
            hasMethod = FlagCore.Instance.callBackMethodActionList[count] != String.Empty;

            if (hasMethod)
            {
                // Debug.Log(textPairEventList[0].GetPersistentMethodName(0));
                textPairEventList.Find(x =>
                    x.GetPersistentMethodName(0) == FlagCore.Instance.callBackMethodActionList[count]).Invoke();
            }
        }


        if (maxEventCount == 1 || count + 1 == maxEventCount)
        {
            count = 0;
            maxEventCount = 0;
            FlagCore.Instance.callBackMethodActionList.Clear();
            Debug.Log("owariiiii");

            if (!hasMethod)
                InGameMenu.Instance.SetOverlay(true, true);

            InGameMenu.Instance.OnSimpleMenuClose(5, true);
            InGameMenu.Instance.OnSimpleMenuClose(6, true);
            callBack.Invoke();
            DOVirtual.DelayedCall(2f, () =>
            {
                FlagCore.Instance.CheckForceExecute();
                isEventRunning = false;
            }).Play();
            return;
        }
        else if (!hasMethod)
        {
            //あとは作ったメソッドのコールバックにShowText()を挟む
            Debug.Log("!hasMethod");
            SayMessageSystem.Instance.ShowText();
        }

        count++;
    }

    private void AddEvent(int id)
    {
        Debug.Log("AddEvent");
        Debug.Log(!FlagCore.Instance.CanBeRunning(id));
        if (!FlagCore.Instance.CanBeRunning(id)) return;

        AllFlagList.StoryEventSaveType saveType = FlagCore.Instance.GetEvent(id);
        foreach (FlagCore.TextPairMethod t in saveType.textPairMethod)
        {
            SayMessageSystem.Instance.AppendText(t.textId, saveType.eventType);
            FlagCore.Instance.callBackMethodActionList.Add(t.methodName);
        }

        maxEventCount += saveType.textPairMethod.Count;
        FlagCore.Instance.SetEvent(saveType);
    }

    private void StartOpening()
    {
        // dollyObj.SetActive(true);
        // prologue.StartPrologue();

        Debug.Log(nameof(StartOpening));

        DOTween.To(() => colorGrading.saturation.value, x => colorGrading.saturation.value = x, saturation_max, 3f)
            .Play();
        DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x,
            chromatic_min, 3f).Play().OnComplete(() =>
        {
            chromaticAberration.active = false;
            colorGrading.active = false;
        });

        AudioManager.Instance.PlayBgm(healing1, true);
    }

    private Sequence blink;

    private void Blink()
    {
        Debug.Log("Blink");
        blinkImage.material.SetFloat(_FillAmount, 0.7f);
        blinkImage.material.SetFloat(_Height, 0.04f);
        blink = DOTween.Sequence();

        blink.AppendInterval(4f);
        blink.Append(blinkImage.material.DOFloat(0.5f, _FillAmount, 6f).SetEase(Ease.OutBack, 2f));
        blink.Join(blinkImage.material.DOFloat(0.16f, _Height, 6f).SetEase(Ease.OutBack, 2f));
        blink.AppendInterval(1f);
        blink.Append(blinkImage.material.DOFloat(0.6f, _FillAmount, 6f).SetEase(Ease.OutBack, 2f));
        blink.Join(blinkImage.material.DOFloat(0.08f, _Height, 6f).SetEase(Ease.OutBack, 2f));
        blink.AppendInterval(1f);
        blink.Append(blinkImage.material.DOFloat(0f, _FillAmount, 6f).SetEase(Ease.OutBack, 2f));
        blink.Join(blinkImage.material.DOFloat(0f, _Height, 6f).SetEase(Ease.OutBack, 2f));
        blink.Play().SetUpdate(true).OnComplete(TalkEventInitializer);
    }

    public void SetCamera(bool isDefault)
    {
        mainCameraBrain.enabled = !isDefault;
        foreach (CinemachineVirtualCamera camera in eventVirtualCameraList) camera.enabled = !isDefault;

        if (isDefault)
        {
            mainCamera.transform.localPosition = Vector3.zero;
        }
    }

    public void SetPriorityDefaultCamera(int index)
    {
        foreach (Camera camera in eventDefaultCameraList) camera.depth = 0f;
        eventDefaultCameraList[index].depth = index;
    }

    public void SetPriorityVirtualCamera(int index)
    {
        foreach (CinemachineVirtualCamera camera in eventVirtualCameraList) camera.Priority = 0;
        eventVirtualCameraList[index].Priority = 100;
    }

    public void FirstMobSpawner()
    {
        Sequence seq = DOTween.Sequence();
        InGameMenu.Instance.SetOverlay(false, false);
        PhaseSystem.Instance.CountdownPhase();
        SetPriorityVirtualCamera(0);
        SetCamera(false);

        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        seq.AppendInterval(4f);
        seq.AppendCallback(() => ViewCore(7f));
        seq.AppendInterval(8f);
        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(false, InGameMenu.FadeType.Normal));
        seq.AppendInterval(4f);
        seq.AppendCallback(() =>
        {
            SetCamera(true);
            InGameMenu.Instance.SetOverlay(true, true);
        });
        seq.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        seq.AppendInterval(4f);
        seq.Play();
    }

    public void ViewCore(float duration)
    {
        SetPriorityVirtualCamera(0);
        CinemachineTrackedDolly dolly = eventVirtualCameraList[0].GetCinemachineComponent<CinemachineTrackedDolly>();
        DOTween.To(() => dolly.m_PathPosition, x => dolly.m_PathPosition = x, 1f, duration).Play();
    }
}