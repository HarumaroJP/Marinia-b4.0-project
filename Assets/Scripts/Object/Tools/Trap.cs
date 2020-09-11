using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private Transform rotateAxis;
    [SerializeField] private Transform trapMouth_L;
    [SerializeField] private Transform trapMouth_R;
    [SerializeField] private float duration;
    [SerializeField] private int trapDamage;
    [SerializeField] private int[] canTrapEntityIDs;
    MonsterManager monster;
    private Transform trappedObject;
    private bool isMoving;
    private bool isClose;
    private Vector3 rotateTarget;
    private float time;
    private const float maxTime = 0.2f;

    void OnTrap()
    {
        if (isClose) return;
        // Debug.Log("trappppp");
        trappedObject = monster.transform;
        transform.SetParent(trappedObject, true);
        monster.SetStatusParam(-trapDamage);
        isMoving = true;
        isClose = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            time += Time.deltaTime;
            Vector3 position = rotateAxis.position;
            trapMouth_L.RotateAround(position, transform.right, duration);
            trapMouth_R.RotateAround(position, -transform.right, duration);
            if (time >= maxTime)
            {
                isMoving = false;
                time = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        monster = other.GetComponent<MonsterManager>();
        if (!(monster != null && canTrapEntityIDs.Any(id => id == monster.monsterData.GetMonsterID()))) return;
        OnTrap();
    }
}