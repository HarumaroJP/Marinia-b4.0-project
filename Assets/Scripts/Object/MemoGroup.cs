using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoGroup : MonoBehaviour
{
    [SerializeField] private MemoSlotScript[] memoGroupSlots;
    [SerializeField] private List<bool> memoHoldData = new List<bool>();
    [SerializeField] private Image textImage;

    public void Start()
    {
        memoHoldData = Enumerable.Repeat(false, memoGroupSlots.Length).ToList();
    }

    public void SetMemoElement(int id)
    {
        Debug.Log(id);
        Debug.Log(memoHoldData.Count);
        memoGroupSlots[id].SetMemo();
        memoHoldData[id] = true;
        CheckFlag();
    }

    private void CheckFlag()
    {
        if (memoHoldData.GetRange(0, 3).All(x => x))
            FlagCore.Instance.SetEpisodeFlag(7, AllFlagList.FlagType.Memo1To3, true);
        if (memoHoldData[18]) FlagCore.Instance.SetEpisodeFlag(7, AllFlagList.FlagType.Memo18, true);
    }

    public void MemoOpen(int id)
    {
        MemoData data =  ItemLibrary.Instance.FindMemos(id);
        Debug.Log(id);
        Debug.Log(data);
        // string[] splicedText = data.GetText().Split(nameChecker);
        // for (int i = 0; i < splicedText.Length; i++)
        // {
        //     if (splicedText[i] == "PlayerName") splicedText[i] = playerName;
        // }
        //
        // textMesh.text = String.Concat(splicedText);
        textImage.sprite = data.GetText();
        InGameMenu.Instance.OnSimpleMenuOpen(4, false);
    }
}