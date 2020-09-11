//  SaveData.cs
//  http://kan-kikuchi.hatenablog.com/entry/Json_SaveData
//
//  Created by kan.kikuchi on 2016.11.21.

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// クラスを丸ごとJsonで保存するデータクラス
/// </summary>
[Serializable]
public class MariniaSaveLoader
{
    [SerializeField] private static SaveManager saveManager;

    private static MariniaSaveLoader _instance = null;

    public static MariniaSaveLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                Load();
            }

            return _instance;
        }
    }

    //SaveDataをJsonに変換したテキスト(リロード時に何度も読み込まなくていいように保持)
    [SerializeField] private static string _jsonText = "";


    //=================================================================================
    //保存されるデータ(public or SerializeFieldを付ける)
    //=================================================================================


    public List<SaveDataAsset.SavePlayerObjectType> SavePlayerObjectData;

    //引数のオブジェクトをシリアライズして返す
    public static string Serialize<T>(T obj)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, obj);
        return Convert.ToBase64String(memoryStream.GetBuffer());
    }

    //引数のテキストを指定されたクラスにデシリアライズして返す
    public static T Deserialize<T>(string str)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));
        return (T) binaryFormatter.Deserialize(memoryStream);
    }

    //=================================================================================
    //取得
    //=================================================================================

    /// <summary>
    /// データを再読み込みする。
    /// </summary>
    public void Reload()
    {
        JsonUtility.FromJsonOverwrite(GetJson(), this);
    }

    //データを読み込む。
    public static void Load()
    {
        _instance = JsonUtility.FromJson<MariniaSaveLoader>(GetJson());
    }

    //保存しているJsonを取得する
    private static string GetJson()
    {
        //既にJsonを取得している場合はそれを返す。
        if (!string.IsNullOrEmpty(_jsonText))
        {
            return _jsonText;
        }

        //Jsonを保存している場所のパスを取得。
        string filePath = "Assets/SaveData/savedata.json";

        //Jsonが存在するか調べてから取得し変換する。存在しなければ新たなクラスを作成し、それをJsonに変換する。
        if (File.Exists(filePath) && File.ReadAllText(filePath) != String.Empty)
        {
            _jsonText = File.ReadAllText(filePath);
        }
        else
        {
            _jsonText = JsonUtility.ToJson(new MariniaSaveLoader());
        }

        return _jsonText;
    }

    //=================================================================================
    //保存
    //=================================================================================

    /// <summary>
    /// データをJsonにして保存する。
    /// </summary>
    public static void Save()
    {
        Debug.Log("Saving...");
        _jsonText = JsonUtility.ToJson(Instance);
        Debug.Log(_jsonText);
        File.WriteAllText("Assets/SaveData/savedata.json", String.Empty);
        StreamWriter streamWriter = new StreamWriter("Assets/SaveData/savedata.json");
        streamWriter.Write(_jsonText);
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("Saved!");
    }

    //=================================================================================
    //削除
    //=================================================================================

    /// <summary>
    /// データを全て削除し、初期化する。
    /// </summary>
    public void Delete()
    {
        _jsonText = JsonUtility.ToJson(new MariniaSaveLoader());
        Reload();
    }

    //=================================================================================
    //保存先のパス
    //=================================================================================

    //保存する場所のパスを取得。
    private static string GetSaveFilePath()
    {
        string filePath = "SaveData";

        //確認しやすいようにエディタではAssetsと同じ階層に保存し、それ以外ではApplication.persistentDataPath以下に保存するように。
#if UNITY_EDITOR
        filePath += ".json";
#else
    filePath = Application.persistentDataPath + "/" + filePath;
#endif

        return filePath;
    }
}