using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public MonsterData monsterData;
    [SerializeField] public float entityHP;
    [SerializeField] public int maxEntityHP;
    [SerializeField] public bool canDecoy;
    [SerializeField] private UnityEvent onDeath = new UnityEvent();

    private void Start()
    {
        maxEntityHP = (int) entityHP;
        target = GameObject.FindWithTag("Player").transform;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.transform;
    }

    public void SetStatusParam(float value)
    {
        // Debug.Log(value);
        // Debug.Log(entityHP);
        if (entityHP + value > 0 && entityHP + value < maxEntityHP)
        {
            Debug.Log("itaiiiiii");
            entityHP += value;
            entityHP = Mathf.Clamp(entityHP, 0f, maxEntityHP);
        }
        else if (entityHP + value <= 0)
        {
            onDeath.Invoke();
        }
    }
}