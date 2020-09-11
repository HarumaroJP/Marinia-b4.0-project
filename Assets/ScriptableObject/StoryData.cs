using System;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "story", menuName = "CreateStory")]
public class StoryData : ScriptableObject
{
    [SerializeField] public AllFlagList story;

    // private void Awake()
    // {
    //     story.episodeEvent.Sort((a, b) => a.eventID - b.eventID);
    // }
    // private AllFlagList.StoryEventSaveType tmpEventElem;
    //
    // private void Awake()
    // {
    //     //First load only
    //     for (int index = 0; index < story.episodeEvent.Count; index++)
    //     {
    //         AllFlagList.StoryEventSaveType eventElem = story.episodeEvent[index];
    //         tmpEventElem = eventElem;
    //         tmpEventElem.isAlready = false;
    //         story.episodeEvent[index] = tmpEventElem;
    //     }
    // }

    public AllFlagList GetStory() => story;
}