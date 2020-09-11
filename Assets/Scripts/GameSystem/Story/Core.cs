using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour, IUsable
{
    [SerializeField] private EventManager eventManager;

    public void Initialize()
    {
        eventManager.RunEvent();
    }
}