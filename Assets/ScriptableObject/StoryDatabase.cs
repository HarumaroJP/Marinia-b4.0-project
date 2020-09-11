using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryDatabase", menuName = "CreateStoryDatabase")]
public class StoryDatabase : ScriptableObject
{
    [SerializeField] public List<ValueList> episodeList;

    [SerializeField] public StoryData eventList;

    public StoryData GetEventList() => eventList;
}

[System.SerializableAttribute]
public class ValueList
{
    public List<int> list;


    public ValueList(List<int> list)
    {
        this.list = list;
    }
}