using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "memo", menuName = "CreateMemo")]
public class MemoData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private Sprite text;

    public string GetTitle() => title;
    public Sprite GetText() => text;
}