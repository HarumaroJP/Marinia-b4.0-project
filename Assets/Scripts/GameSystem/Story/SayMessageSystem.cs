using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SayMessageSystem : SingletonMonoBehaviour<SayMessageSystem>
{
    [SerializeField] private EventManager eventManager;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image coreBackGroundImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private List<MessageType> textList = new List<MessageType>();
    [SerializeField] private float intervalTime;
    [SerializeField] private float coreImageFadeTime;
    public readonly int maxMainTextCount = 34;
    public readonly int maxNameTextCount = Marinia.maxNameCount;

    private int currentSayCount = 0;
    private int maxSayCount;
    private bool isRunning;
    private StringBuilder showText;

    [Serializable]
    public struct MessageType
    {
        public string name;
        public string text;
        public Marinia.EventType type;

        public MessageType(string item1, string item2, Marinia.EventType item3)
        {
            name = item1;
            text = item2;
            type = item3;
        }
    }


    public void StartEvent()
    {
        // Debug.Log(nameof(FadeBackGround));
        backGroundImage.DOFade(1f, 1f).Play().OnComplete(ShowText).SetUpdate(true);
        nameText.DOFade(1f, 1f).Play().SetUpdate(true);
        messageText.DOFade(1f, 1f).Play().SetUpdate(true);
    }

    public void ShowText()
    {
        if (textList.Count == 0) return;
        messageCor = StartCoroutine(SayCoroutine(textList[currentSayCount]));
    }

    public void AppendText(int id, Marinia.EventType eventType)
    {
        Debug.Log("AppendID = " + id);
        Debug.Log("Add " + Marinia.GetEvent(id, eventType));
        (string, string) eventItem = Marinia.GetEvent(id, eventType);
        textList.Add(new MessageType(eventItem.Item1, eventItem.Item2, eventType));
    }

    private bool trigger;
    private bool canSkip;
    private char[] intervalMarks = {'。', '、', '?', '!', '？', '！', '.'};
    private MessageType tmpMessage;
    private Coroutine messageCor;

    private IEnumerator SayCoroutine(MessageType messageType)
    {
        // Debug.Log(messageType.name + messageType.text);

        tmpMessage = messageType;
        nameText.text = messageType.name;
        messageText.text = String.Empty;
        showText = new StringBuilder();
        canSkip = true;

        switch (messageType.type)
        {
            case Marinia.EventType.Core when !trigger:
                coreBackGroundImage.DOFade(1f, coreImageFadeTime).SetUpdate(true).Play();
                trigger = !trigger;
                yield return new WaitForSecondsRealtime(coreImageFadeTime);
                break;

            case Marinia.EventType.Player when trigger:
                coreBackGroundImage.DOFade(0f, coreImageFadeTime).SetUpdate(true).Play();
                trigger = !trigger;
                yield return new WaitForSecondsRealtime(coreImageFadeTime);
                break;
        }

        foreach (char charElem in messageType.text)
        {
            showText.Append(charElem);
            messageText.text = showText.ToString();

            if (intervalMarks.Any(elem => elem == charElem))
                yield return new WaitForSecondsRealtime(intervalTime + 0.25f);
            else
                yield return new WaitForSecondsRealtime(intervalTime);
        }

        currentSayCount++;
        isRunning = true;
    }


    public void Update()
    {
        if (isRunning)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                isRunning = false;
                if (currentSayCount == EventManager.maxEventCount)
                {
                    OffImpactText();
                    return;
                }

                eventManager.MoveNextEvent();
            }
        }
        else
        {
            if (canSkip && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                StopCoroutine(messageCor);
                messageText.text = tmpMessage.text;
                currentSayCount++;
                isRunning = true;
                canSkip = false;
            }
        }
    }

    private void OffImpactText()
    {
        currentSayCount = 0;
        maxSayCount = 0;
        textList.Clear();
        // Debug.Log("Clear()");
        messageText.text = String.Empty;
        backGroundImage.DOFade(0f, 0.6f).OnComplete(eventManager.MoveNextEvent).SetUpdate(true).Play();
        nameText.DOFade(0f, 0.6f).Play().SetUpdate(true);
        messageText.DOFade(0f, 0.6f).Play().SetUpdate(true);
        isRunning = false;
    }
}