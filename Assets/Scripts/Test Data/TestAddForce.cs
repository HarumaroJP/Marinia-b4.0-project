using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddForce : MonoBehaviour
{
    [SerializeField] private GameObject originObj;
    [SerializeField] private Rigidbody rig;
    [SerializeField] private Transform targetPoint;

    private void Start()
    {
        Shoot(targetPoint.position);
    }

    private void Shoot(Vector3 i_targetPosition)
    {
        // とりあえず適当に60度でかっ飛ばすとするよ！
        ShootFixedAngle(i_targetPosition, 45.0f);
    }

    private void ShootFixedAngle(Vector3 i_targetPosition, float i_angle)
    {
        float speedVec = ComputeVectorFromAngle(i_targetPosition, i_angle);
        if (speedVec <= 0.0f) return;
        Vector3 vec = ConvertVectorToVector3(speedVec, i_angle, i_targetPosition) * rig.mass;
        rig.AddForce(vec, ForceMode.Impulse);
    }

    private float ComputeVectorFromAngle(Vector3 i_targetPosition, float i_angle)
    {
        // xz平面の距離を計算。
        Vector3 position = originObj.transform.position;
        Vector2 startPos = new Vector2(position.x, position.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float g = Physics.gravity.y;
        float y0 = position.y;
        float y = i_targetPosition.y;

        float rad = i_angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Square = g * distance * distance / (2 * cos * cos * (y - y0 - distance * tan));

        if (v0Square <= 0.0f) return 0.0f;

        float v0 = Mathf.Sqrt(v0Square);
        return v0;
    }

    private Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition)
    {
        Vector3 startPos = originObj.transform.position;
        Vector3 targetPos = i_targetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        Vector3 dir = (targetPos - startPos).normalized;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = i_v0 * Vector3.right;

        vec = yawRot * Quaternion.AngleAxis(i_angle, Vector3.forward) * vec;

        return vec;
    }
}