using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Marinia : MonoBehaviour
{
    public static List<GameObject> ObjectList_Build = new List<GameObject>();
    public static List<GameObject> ObjectList_Hold = new List<GameObject>();
    public static List<GameObject> ObjectList_Natural = new List<GameObject>();
    public static List<GameObject> ObjectList_Environment = new List<GameObject>();
    public static List<GameObject> UIObjectList = new List<GameObject>();
    public static List<GameObject> MonsterObjectList = new List<GameObject>();
    public static List<string> eventMessageList_impact = new List<string>();
    public static List<string> eventMessageList_player = new List<string>();
    public static string playerName = "NoName";
    public static string coreName = "テティス";
    public static int maxNameCount = 7;
    public static float volume;
    public static float bgmVolume;
    public static float seVolume;

    public enum ItemType
    {
        Build,
        Hold,
        Natural,
        Environment
    }

    public enum EventType
    {
        Core,
        Player
    }

    public enum SetType
    {
        None,
        Sea,
        Ground
    }

    public static GameObject Find(string name, ItemType type)
    {
        switch (type)
        {
            case ItemType.Build:
                return ObjectList_Build.FirstOrDefault(data => data.name == name);

            case ItemType.Hold:
                return ObjectList_Hold.FirstOrDefault(data => data.name == name);

            case ItemType.Natural:
                return ObjectList_Natural.FirstOrDefault(data => data.name == name);

            case ItemType.Environment:
                break;
            default:
                break;
        }

        return null;
    }

    public static void SetVolumeParam(float master, float bgm, float se)
    {
        volume = master;
        bgmVolume = bgm;
        seVolume = se;
    }

    public static GameObject UI(string name)
    {
        return UIObjectList.FirstOrDefault(data => data.name == name);
    }

    public static (string, string) GetEvent(int eventId, EventType type)
    {
        switch (type)
        {
            case EventType.Core:
                return (coreName, eventMessageList_impact[eventId]);

            case EventType.Player:
                return (playerName, eventMessageList_player[eventId]);

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static GameObject GetMonster(int monsterId)
    {
        return MonsterObjectList.Find(x => x.name == ItemLibrary.Instance.FindMonsters(monsterId).GetFileName());
    }
//TODO もんすたーのゲームオブジェクトを読み込めるようにする

    // private const float raitoValue = 1.5f;
    // private readonly int currentWidth = Screen.width;
    // private readonly int currentHeight = Screen.height;
    // private readonly int setWidth = (int) (Screen.width / raitoValue);
    // private readonly int setHeight = (int) (Screen.height / raitoValue);
    //
    // private void Start()
    // {
    //     Screen.SetResolution(setWidth, setHeight, true);
    // }

    public void SetRegResolution()
    {
        // PlayerPrefs.SetInt("Screenmanager Resolution Height", currentHeight);
        // PlayerPrefs.SetInt("Screenmanager Resolution Width", currentWidth);
    }

    static Marinia _instance = null;

    // オブジェクト実体の参照（初期参照時、実体の登録も行う）
    static Marinia instance => _instance ? _instance : (_instance = FindObjectOfType<Marinia>());

    void Awake()
    {
        // ※オブジェクトが重複していたらここで破棄される

        // 自身がインスタンスでなければ自滅
        if (this != instance)
        {
            Destroy(gameObject);
            return;
        }

        // 以降破棄しない
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == instance) _instance = null;
    }
}