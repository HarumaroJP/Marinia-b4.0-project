using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    [SerializeField] private float effectRange;
    [SerializeField] private int repeatRate;
    [SerializeField] private float DestroyTime;
    private GameObject player;
    private Collider[] col = new Collider[10];

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        InvokeRepeating(nameof(CheckEffectDecoy), 1f, repeatRate);
        Invoke(nameof(KillDecoy), DestroyTime);
    }

    private void CheckEffectDecoy()
    {
        // Debug.Log(nameof(CheckEffectDecoy));
        int size = Physics.OverlapSphereNonAlloc(transform.position, effectRange, col);
        if (size <= 0) return;

        for (int i = 0; i < size; i++)
        {
            MonsterManager monsterManager = col[i].GetComponent<MonsterManager>();

            if (monsterManager != null && monsterManager.canDecoy)
            {
                // Debug.Log(nameof(monsterManager.SetTarget));
                monsterManager.SetTarget(gameObject);
            }
        }
    }

    private void KillDecoy()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, effectRange, col);
        if (size <= 0) return;

        for (int i = 0; i < size; i++)
        {
            MonsterManager monsterManager = col[i].GetComponent<MonsterManager>();

            if (monsterManager != null && monsterManager.canDecoy)
            {
                monsterManager.SetTarget(player);
            }
        }

        Destroy(gameObject);
    }

    //TODO それぞれのモンスターにMonsterManagerを参照させるようにする。

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, effectRange);
    }
}