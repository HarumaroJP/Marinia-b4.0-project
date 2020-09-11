using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TitlePanelManager : MonoBehaviour
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject SaveDataPanel;
    [SerializeField] private GameObject NewGamePanel;
    [SerializeField] private GameObject fadeCanvas;
    [SerializeField] private RectTransform bola_image;
    [SerializeField] private RectTransform orenix_image;
    [SerializeField] private Image nameAlert;
    [SerializeField] private TMP_InputField textMesh;
    [SerializeField] private FadeOutSystem fadeOutSystem;
    [SerializeField] private float moveDistance;
    [SerializeField] private Vector2 nextDistance1;
    [SerializeField] private Vector2 nextDistance2;

    private Vector2 defaultDistance1, defaultDistance2;

    private void Start()
    {
        defaultDistance1 = bola_image.anchoredPosition;
        defaultDistance2 = orenix_image.anchoredPosition;
    }

    public void OnStartGame()
    {
        if (textMesh.text == String.Empty || textMesh.text.All(x => x == ' '))
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(nameAlert.DOFade(1f, 0.5f));
            sequence.AppendInterval(2f);
            sequence.Append(nameAlert.DOFade(0f, 0.5f));
            sequence.Play();
            return;
        }

        AudioManager.Instance.PlaySeByName("5_start");
        Marinia.playerName = textMesh.text;
        fadeCanvas.SetActive(true);
        Invoke(nameof(StartGame), 1.5f);
    }

    public void OnSaveSelect(bool value)
    {
        MainPanel.SetActive(!value);
        SaveDataPanel.SetActive(value);

        AnimateReset();
    }

    public void OnNewGame(bool value)
    {
        MainPanel.SetActive(!value);
        NewGamePanel.SetActive(value);

        AnimateReset();
    }

    public void OnLoadThisGame()
    {
        fadeCanvas.SetActive(true);
        fadeOutSystem.FadeCheck = true;
        SceneManager.LoadScene("TreeIsland");
        //For Only Developer
    }

    public void GameEnd()
    {
        Invoke(nameof(Quit), 1.5f);
        //For Only Developer
    }

    private void StartGame()
    {
        fadeOutSystem.FadeCheck = true;
    }

    public void AnimateButton1(bool isPoint)
    {
        bola_image.DOAnchorPos(isPoint ? nextDistance1 : defaultDistance1, 0.4f).SetEase(Ease.OutBack).Play();
    }
    
    public void AnimateButton2(bool isPoint)
    {
        orenix_image.DOAnchorPos(isPoint ? nextDistance2 : defaultDistance2, 0.4f).SetEase(Ease.OutBack).Play();
    }

    private void AnimateReset()
    {
        bola_image.anchoredPosition = defaultDistance1;
        orenix_image.anchoredPosition = defaultDistance2;
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}