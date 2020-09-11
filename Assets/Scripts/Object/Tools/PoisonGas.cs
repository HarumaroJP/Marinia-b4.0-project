using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private int damage;
    [SerializeField] private float effectRange;
    [SerializeField] private int repeatRate;

    private StatusManager statusManager;
    private Collider[] col = new Collider[10];

    void Start()
    {
        statusManager = GameObject.FindWithTag("MainMenu").GetComponent<StatusManager>();
    }

    public void Initialize()
    {
        particle.Play();
        InvokeRepeating(nameof(CheckPoisonDamage), 1f, repeatRate);
    }

    private void CheckPoisonDamage()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, effectRange, col);
        if (size <= 0) return;

        for (int i = 0; i < size; i++)
        {
            if (col[i].CompareTag("Player"))
            {
                statusManager.SetStatusParam(StatusManager.Status.Health, -damage);
            }

            MonsterManager monsterManager = col[i].GetComponent<MonsterManager>();

            if (monsterManager != null)
            {
                monsterManager.SetStatusParam(-damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, effectRange);
    }
}