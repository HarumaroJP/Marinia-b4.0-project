using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "CreateMonsterDatabase")]
public class MonsterDatabase : ScriptableObject
{
    [Serializable]
    public struct MonsterDropData
    {
        public ItemData item;
        public int minValue;
        public int maxValue;
    }
    
    [Serializable]
    public struct MonsterParam
    {
        public int monsterHP;
        public float damage;
        public float attackRadius;
        public float attackRate;
    }
    
    [SerializeField] private List<MonsterData> monsterList = new List<MonsterData>();

    public List<MonsterData> GetMonsterList()
    {
        return monsterList;
    }

    public int GetMonsterCount()
    {
        return monsterList.Count;
    }
}