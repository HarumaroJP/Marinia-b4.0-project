using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Orenix : MonoBehaviour, ISummonable
{
    [SerializeField] private MonsterManager manager;
    [SerializeField] private Animator Orenix_anim;
    [SerializeField] private NavMeshAgent Orenix_agent;
    [SerializeField] private Rigidbody rig;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private StatusManager statusManager;
    private Collider[] col = new Collider[10];

    [Header("Parameter")] [SerializeField] private bool isDeath;
    [SerializeField] private MonsterDatabase.MonsterParam tmpMonsterParam;
    [SerializeField] private float stopDistance;

    // Start is called before the first frame update
    void Awake()
    {
        tmpMonsterParam = manager.monsterData.GetMonsterParam();
        statusManager = GameObject.FindWithTag("MainMenu").GetComponent<StatusManager>();
        manager.entityHP = tmpMonsterParam.monsterHP;

        isDeath = true;
    }

    private bool tmpResult = false;
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Attack = Animator.StringToHash("Attack");

    void FixedUpdate()
    {
        if (isDeath) return;
        bool result = CheckPlayer();

        if (!result)
        {
            Orenix_agent.SetDestination(manager.target.position);
        }

        if (tmpResult != result)
        {
            Orenix_anim.SetBool(Attack, result);
            tmpResult = result;
        }

        bool canStop = Orenix_agent.remainingDistance <= stopDistance;
        if (BoolHasChanged(canStop))
        {
            if (canStop)
            {
                Orenix_anim.SetBool(Running, false);
                Orenix_agent.velocity = Vector3.zero;
                Orenix_agent.isStopped = true;
            }
            else
            {
                Play();
            }
        }

        //TODO
        //・モンスターの攻撃アニメーションがずれる
        //・モンスターが永遠に攻撃する
    }

    private bool tmpBoolean = true;

    private bool BoolHasChanged(bool value)
    {
        bool re = tmpBoolean != value;
        tmpBoolean = value;
        return re;
    }

    public void Play()
    {
        Orenix_anim.SetBool(Running, true);
        Orenix_agent.isStopped = false;
        isDeath = false;
    }

    public void Stop()
    {
        Orenix_anim.SetBool(Running, false);
        Orenix_agent.velocity = Vector3.zero;
        Orenix_agent.isStopped = true;
        isDeath = true;
    }

    public void OnDamage()
    {
        statusManager.SetStatusParam(StatusManager.Status.Health, -tmpMonsterParam.damage);
    }


    public void OnDeath()
    {
        isDeath = true;
        Orenix_agent.enabled = false;
        rig.isKinematic = false;
        // rig.AddTorque(transform.forward * 15, ForceMode.Impulse);
        Orenix_anim.SetBool(Running, false);

        Quaternion rotation = transform.rotation;
        Sequence layAnim = DOTween.Sequence();
        layAnim.Append(transform.DORotate(new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, -80f), 2f));
        layAnim.Append(transform.DOScale(Vector3.zero, 0.5f).SetDelay(1f).OnComplete(OnKill));
        layAnim.Play();
    }

    private void OnKill()
    {
        ItemLibrary.Instance.InstantiateDropBox(manager.monsterData, transform.position);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(attackPoint.position, tmpMonsterParam.attackRadius);
    }

    private bool CheckPlayer()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, tmpMonsterParam.attackRadius, col);
        if (size <= 0) return false;

        bool newResult = false;

        for (int i = 0; i < size; i++)
        {
            newResult = col[i].CompareTag("Player");
            if (newResult)
                break;
        }

        return newResult;
    }
}