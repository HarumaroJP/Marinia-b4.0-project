using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Bola : MonoBehaviour, ISummonable
{
    [SerializeField] private MonsterManager manager;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private StatusManager statusManager;
    [SerializeField] private Rigidbody rig;
    [SerializeField] private Animator animator;
    private Transform player;

    [Header("Parameter")] [SerializeField] private bool isDeath;
    [SerializeField] private float jumpDeg;
    [SerializeField] private MonsterDatabase.MonsterParam tmpMonsterParam;

    void Awake()
    {
        tmpMonsterParam = manager.monsterData.GetMonsterParam();
        statusManager = GameObject.FindWithTag("MainMenu").GetComponent<StatusManager>();
        player = GameObject.FindWithTag("Player").transform;
        agent.updatePosition = false;
        // agent.updateRotation = false;
        manager.entityHP = tmpMonsterParam.monsterHP;

        isDeath = true;
    }

    void FixedUpdate()
    {
        if (isDeath) return;
        agent.SetDestination(manager.target.position);
    }

    Collider[] col = new Collider[10];
    private static readonly int CanJump = Animator.StringToHash("canJump");

    private void CheckPlayer()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, tmpMonsterParam.attackRadius, col);
        if (size <= 0) return;

        for (int i = 0; i < size; i++)
        {
            if (col[i].CompareTag("Player"))
            {
                // Debug.Log("isPlayer");
                statusManager.SetStatusParam(StatusManager.Status.Health, -tmpMonsterParam.damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tmpMonsterParam.attackRadius);
    }


    public void Jump()
    {
        if (isDeath) return;
        transform.DOLookAt(player.position, 0.5f).Play();
        ShootFixedAngle(agent.nextPosition, jumpDeg);
        Invoke(nameof(CheckPlayer), 0.4f);
    }

    public void OnDeath()
    {
        isDeath = true;
        rig.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        agent.enabled = false;
        animator.SetBool(CanJump, false);
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(OnKill).SetDelay(1.5f).Play();
    }

    private void OnKill()
    {
        ItemLibrary.Instance.InstantiateDropBox(manager.monsterData, transform.position);
        Destroy(gameObject);
    }

    private void ShootFixedAngle(Vector3 iTargetPosition, float iAngle)
    {
        // Debug.Log(nameof(ShootFixedAngle));
        float speedVec = ComputeVectorFromAngle(iTargetPosition, iAngle);
        if (speedVec <= 0.0f) return;
        Vector3 vec = ConvertVectorToVector3(speedVec, iAngle, iTargetPosition) * rig.mass;
        rig.AddForce(vec, ForceMode.Impulse);
    }

    private float ComputeVectorFromAngle(Vector3 iTargetPosition, float iAngle)
    {
        Vector3 position = gameObject.transform.position;
        Vector2 startPos = new Vector2(position.x, position.z);
        Vector2 targetPos = new Vector2(iTargetPosition.x, iTargetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float g = Physics.gravity.y;
        float y0 = position.y;
        float y = iTargetPosition.y;

        float rad = iAngle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Square = g * distance * distance / (2 * cos * cos * (y - y0 - distance * tan));

        if (v0Square <= 0.0f) return 0.0f;

        float v0 = Mathf.Sqrt(v0Square);
        return v0;
    }

    private Vector3 ConvertVectorToVector3(float iV0, float iAngle, Vector3 iTargetPosition)
    {
        Vector3 startPos = gameObject.transform.position;
        Vector3 targetPos = iTargetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        Vector3 dir = (targetPos - startPos).normalized;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = iV0 * Vector3.right;

        vec = yawRot * Quaternion.AngleAxis(iAngle, Vector3.forward) * vec;

        return vec;
    }

    public void Play()
    {
        isDeath = false;
        agent.isStopped = false;
    }

    public void Stop()
    {
        isDeath = true;
        agent.isStopped = true;
    }
}