using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Robot_AI : MonoBehaviour
{
    [SerializeField] private Animator Robot_anim;
    [SerializeField] private NavMeshAgent Robot;
    [SerializeField] private Transform target;
    [SerializeField] private float stopDistance;
    bool tmpBool;

    bool canMove => (stopDistance < Vector3.Distance(transform.position, target.position));

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        tmpBool = !canMove;
    }

    void Update()
    {
        MoveRobot(canMove);

        if (tmpBool != canMove)
        {
            if (canMove)
            {
                StopRobotSystem();
            }
            else
            {
                StartRobotSystem();
            }
        }

        tmpBool = canMove;
    }

    void StartRobotSystem()
    {
        // Robot_anim.speed = 1;
        Robot_anim.SetBool("Arm_Start", true);
    }

    void StopRobotSystem()
    {
        // Robot_anim.speed = -1;
        Robot_anim.SetBool("Arm_Start", false);
    }

    void MoveRobot(bool canMove)
    {
        if (Robot.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            Robot.isStopped = !canMove;
            Robot.SetDestination(target.position);
        }

        // Debug.Log("MoveRobot()");
        // Debug.Log(canMove);
    }
}