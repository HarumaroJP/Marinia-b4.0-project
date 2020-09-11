using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private Control control;
    [SerializeField] private Slider sensi;
    [SerializeField] private Slider master;
    [SerializeField] private Slider bgm;
    [SerializeField] private Slider se;


    public void OnEnablePanel()
    {
        Debug.Log("OnEnable");
        sensi.value = Mathf.InverseLerp(0f, 100f, control.sensi);
        master.value = AudioManager.Instance.volume;
        bgm.value = AudioManager.Instance.bgmVolume;
        se.value = AudioManager.Instance.seVolume;
    }

    public void OnDisablePanel()
    {
        Debug.Log("OnDisable");
        control.sensi = Mathf.Lerp(0f, 100f, sensi.value);
        AudioManager.Instance.volume = master.value;
        AudioManager.Instance.bgmVolume = bgm.value;
        AudioManager.Instance.seVolume = se.value;
        AudioManager.Instance.ReloadAudioSource();
    }
}