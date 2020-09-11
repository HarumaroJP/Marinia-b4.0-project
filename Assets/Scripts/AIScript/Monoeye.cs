using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monoeye : MonoBehaviour, ISummonable
{
    [SerializeField] private MonsterManager manager;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Rigidbody playerRig;
    [SerializeField] private StatusManager statusManager;
    [SerializeField] private Transform attackField;
    private Collider[] col = new Collider[10];

    [Header("Parameter")] [SerializeField] private int monsterHP;
    [SerializeField] private float Damage;
    [SerializeField] private Vector3 attackRadius;
    [SerializeField] private float attackRate;
    [SerializeField] private Vector3 blowHeightOffset;
    [SerializeField] private float power;

    [SerializeField] private bool isDeath;
    // int attackStateHash;

    void Start()
    {
        playerRig = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        statusManager = GameObject.FindWithTag("MainMenu").GetComponent<StatusManager>();
        manager.entityHP = monsterHP;
        // attackStateHash = Animator.StringToHash("Base Layer.Attack");
        InvokeRepeating(nameof(AttackReady), 1f, attackRate);
    }

    private readonly int attack1 = Animator.StringToHash("Attack");
    private readonly int running = Animator.StringToHash("Running");

    void FixedUpdate()
    {
        if (isDeath) return;

        if (manager.target != null)
            // navMeshAgent.SetDestination(manager.target.position);

        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //
        // if (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime >= 1f)
        // {
        //     Attack();
        // }

        // Debug.Log(!IsStop());
        animator.SetBool(running, !IsStop());
    }

    private bool IsStop()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = attackField.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, attackRadius);
    }

    private void AttackReady()
    {
        if (CheckPlayer())
        {
            AttackAnim();
        }
    }

    private bool CheckPlayer()
    {
        int size = Physics.OverlapBoxNonAlloc(attackField.position, attackRadius, col);

        bool newResult = false;
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                newResult = col[i].CompareTag("Player");
                if (newResult)
                    break;
            }
        }

        return newResult;
    }

    private void AttackAnim()
    {
        animator.SetTrigger(attack1);
    }

    public void Attack()
    {
        if (CheckPlayer())
        {
            statusManager.SetStatusParam(StatusManager.Status.Health, -Damage);
            playerRig.AddForce((transform.forward + blowHeightOffset) * power, ForceMode.Impulse);
        }
    }

    public void Play()
    {
        navMeshAgent.isStopped = false;
        isDeath = false;
    }

    public void Stop()
    {
        navMeshAgent.isStopped = true;
        isDeath = true;
    }
}