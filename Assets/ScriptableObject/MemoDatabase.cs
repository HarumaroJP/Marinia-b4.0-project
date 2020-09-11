using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MemoDatabase", menuName = "CreateMemoDatabase")]
public class MemoDatabase : ScriptableObject
{
    [SerializeField] private List<MemoData> memoList = new List<MemoData>();

    public List<MemoData> GetMemoList()
    {
        return memoList;
    }
}