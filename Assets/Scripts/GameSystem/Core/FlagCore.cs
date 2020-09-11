using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FlagCore : SingletonMonoBehaviour<FlagCore>
{
    [SerializeField] public StoryDatabase storyDatabase;
    [SerializeField] public EventManager eventManager;
    [SerializeField] private StoryData storyDataList => storyDatabase.GetEventList();

    public List<string> callBackMethodActionList;

    [Header("現在の進捗リスト")] [SerializeField] public int currentGameProgress;
    [SerializeField] public int currentEventProgress;
    [SerializeField] public int currentCoreProgress;
    [SerializeField] public int currentPlayerProgress;

    [Header("最大の進捗リスト")] [SerializeField] public int maxGameProgress;
    [SerializeField] public int maxEventProgress;
    [SerializeField] public int maxCoreProgress;
    [SerializeField] public int maxPlayerProgress;

    [Serializable]
    public struct TextPairMethod
    {
        public int textId;
        public string methodName;
    }


    void Start()
    {
        Debug.Log(storyDataList.GetStory().episodeEvent.Count);

        for (int id = 1; id <= storyDataList.GetStory().episodeEvent.Count; id++)
        {
            AllFlagList.StoryEventSaveType eventInstance = GetEvent(id);
            eventInstance.isAlready = false;
            for (int i = 0; i < eventInstance.flag.Count; i++)
            {
                AllFlagList.FlagPair tmp = eventInstance.flag[i];
                tmp.flagResult = false;
                eventInstance.flag[i] = tmp;
            }

            int index = storyDataList.GetStory().episodeEvent
                .FindIndex(data => data.eventID == eventInstance.eventID);
            storyDataList.GetStory().episodeEvent[index] = eventInstance;
        }


        maxGameProgress = storyDataList.GetStory().episodeEvent.Count;
        // maxEventProgress = storyDataList.Sum(eventList => eventList.GetStory().episodeEvent.Count);
        // maxCoreProgress = storyDataList.Sum(eventList =>
        //     eventList.GetStory().episodeEvent.Count(type => type.eventType == Marinia.EventType.Core));
        //
        // maxPlayerProgress = storyDataList.Sum(eventList =>
        //     eventList.GetStory().episodeEvent.Count(type => type.eventType == Marinia.EventType.Player));
    }

    public bool CanBeRunning(int eventId)
    {
        Debug.Log(eventId);
        if (currentGameProgress == 0) return true;

        return GetEvent(GetBeforeEpisode().Last()).isAlready
               && GetEvent(GetCurrentEpisode().Last()).isAlready == false;
    }

    public bool IsFlagComplete(int eventId)
    {
        Debug.Log(eventId);
        List<AllFlagList.FlagPair> flagList = storyDataList.GetStory().episodeEvent
            .First(elem => elem.eventID == eventId).flag;

        return flagList.Count == 0 || flagList.All(x => x.flagResult);
    }

    public List<int> GetCurrentEpisode()
    {
        // Debug.Log(storyDatabase.episodeList.Count);
        return storyDatabase.episodeList[currentGameProgress].list;
    }

    public List<int> GetBeforeEpisode()
    {
        return storyDatabase.episodeList[currentGameProgress - 1].list;
    }

    public AllFlagList.StoryEventSaveType GetEvent(int eventId)
    {
        Debug.Log(eventId);
        return storyDataList.GetStory().episodeEvent.First(elem => elem.eventID == eventId);
    }

    public void SetEvent(AllFlagList.StoryEventSaveType eventInstance)
    {
        eventInstance.isAlready = true;

        int index = storyDataList.GetStory().episodeEvent
            .FindIndex(id => id.eventID == eventInstance.eventID);
        storyDataList.GetStory().episodeEvent[index] = eventInstance;

        switch (eventInstance.eventType)
        {
            case Marinia.EventType.Core:
                currentCoreProgress++;
                break;

            case Marinia.EventType.Player:
                currentPlayerProgress++;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetEpisodeFlag(int eventID, AllFlagList.FlagType flagType, bool flagResult)
    {
        Debug.Log(eventID);
        int index = storyDataList.GetStory().episodeEvent
            .FindIndex(id => id.eventID == eventID);
        Debug.Log("SetEpisodeFlag" + index);
        AllFlagList.StoryEventSaveType storyEventSaveType = storyDataList.GetStory().episodeEvent[index];
        int flagPairIndex = storyEventSaveType.flag.FindIndex(x => x.flagType == flagType);
        if (flagPairIndex == -1) return;
        storyEventSaveType.flag[flagPairIndex] = new AllFlagList.FlagPair(flagType, flagResult);
        if (CanBeRunning(eventID) && storyEventSaveType.forceExecute && storyEventSaveType.flag.All(x => x.flagResult))
        {
            InGameMenu.Instance.OnSimpleMenuClose(true);
            eventManager.RunEvent();
        }
    }

    private bool canExecute;

    public void CheckForceExecute()
    {
        CheckCanExecute();
        if (canExecute)
        {
            eventManager.RunEvent();
        }
    }

    public bool CheckCanExecute()
    {
        int nextEvent = currentGameProgress;
        if (nextEvent >= storyDatabase.episodeList.Count) return false;

        int nextEventId = GetCurrentEpisode().First();
        int index = storyDataList.GetStory().episodeEvent
            .FindIndex(id => id.eventID == nextEventId);
        AllFlagList.StoryEventSaveType storyEventSaveType = storyDataList.GetStory().episodeEvent[index];

        Debug.Log(CanBeRunning(nextEventId));
        Debug.Log(storyEventSaveType.forceExecute);
        Debug.Log(storyEventSaveType.flag.All(x => x.flagResult));
        if (storyEventSaveType.flag.Count == 0) return false;

        canExecute = CanBeRunning(nextEvent) && storyEventSaveType.forceExecute &&
                     storyEventSaveType.flag.All(x => x.flagResult);

        return canExecute;
    }

    /*
     * TODO
     * とりあえずイベント管理はできた
     * 指定されたタイミングでイベントを呼び出せるようにする
     */
}

[Serializable]
public class AllFlagList
{
    public List<StoryEventSaveType> episodeEvent;

    [Serializable]
    public enum FlagType
    {
        Memo1To3,
        Memo4To6,
        Memo7To9,
        Memo18,
        PlayerCanAttack,
        Phase1,
        TakeLightFlower,
        Phase2,
        TakeDecoy
    }

    [Serializable]
    public struct StoryEventSaveType
    {
        public int eventID;
        public List<FlagCore.TextPairMethod> textPairMethod;
        public Marinia.EventType eventType;
        public bool forceExecute;
        public bool isAlready;
        public List<FlagPair> flag;
    }

    [Serializable]
    public struct FlagPair
    {
        public FlagType flagType;
        public bool flagResult;

        public FlagPair(FlagType flagType, bool flagResult)
        {
            this.flagType = flagType;
            this.flagResult = flagResult;
        }
    }

    public AllFlagList(List<StoryEventSaveType> episodeEvent)
    {
        this.episodeEvent = episodeEvent;
    }
}