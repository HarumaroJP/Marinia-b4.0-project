using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RockSpawnPoint : SingletonMonoBehaviour<RockSpawnPoint>
{
    [SerializeField] private GameObject rock;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 point1;
    [SerializeField] private Vector3 point2;
    [SerializeField] private float repeatRate;
    [SerializeField] private int maxCount;
    [SerializeField] private int currentCount;
    Vector3 halfExtents = new Vector3(5f, 5f, 5f);
    Vector3 pointPosition;
    RaycastHit hitInfo;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, repeatRate);
    }

    private void Spawn()
    {
        if (currentCount >= maxCount) return;
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(point2.x, point1.x);
            float z = Random.Range(point2.z, point1.z);

            pointPosition = new Vector3(x, 200f, z);

            bool isHit = Physics.BoxCast(pointPosition, halfExtents, Vector3.down, out hitInfo);
            if (hitInfo.collider != null && hitInfo.collider.CompareTag("Floor"))
            {
                Rock rockData = Instantiate(rock, hitInfo.point, Quaternion.identity, parent).GetComponent<Rock>();
                rockData.Appear();
                currentCount++;
                break;
            }
        }
    }

    public void DestroyRock()
    {
        currentCount--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(pointPosition, Vector3.down * 500f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(hitInfo.point, halfExtents);
    }
}