using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "monster", menuName = "CreateMonster")]
public class MonsterData : ScriptableObject
{
    [SerializeField] private int monsterID;
    [SerializeField] private string filename;
    [SerializeField] private MonsterDatabase.MonsterParam monsterParam;
    [SerializeField] private List<MonsterDatabase.MonsterDropData> dropList;



    public int GetMonsterID() => monsterID;
    public string GetFileName() => filename;
    public MonsterDatabase.MonsterParam GetMonsterParam() => monsterParam;
    public List<MonsterDatabase.MonsterDropData> GetDropData() => dropList;
}