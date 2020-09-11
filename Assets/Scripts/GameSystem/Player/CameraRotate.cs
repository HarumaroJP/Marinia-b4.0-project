using System;
using UnityEngine;

[ExecuteInEditMode, DisallowMultipleComponent]
public class CameraRotate : MonoBehaviour
{
    private Transform parentObj;

    [SerializeField] private float minPolarAngle, maxPolarAngle;
    [SerializeField] public float sensi = 5.0f;
    private Quaternion camera_rotate;

    private void Start()
    {
        parentObj = transform.parent;
    }

    void LateUpdate()
    {
        if (Mathf.Approximately(Time.timeScale, 1f))
        {
            camera_rotate = transform.rotation;
            UpdateAngle(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // UpdateDistance(Input.GetAxis("Mouse ScrollWheel"));

            // Vector3 lookAtPos = target.transform.position + offset;
            // UpdatePosition(lookAtPos);
            // transform.LookAt(lookAtPos);
        }
    }

    void UpdateAngle(float x, float y)
    {
        parentObj.Rotate(0, x, 0);
        transform.Rotate(-y, 0, 0);

        // x = azimuthalAngle - x * mouseXSensitivity;
        // azimuthalAngle = Mathf.Repeat(x, 360);
        //
        // y = polarAngle + y * mouseYSensitivity;
        // polarAngle = Mathf.Clamp(y, minPolarAngle, maxPolarAngle);
    }

    // void UpdateDistance(float scroll)
    // {
    //     scroll = distance - scroll * scrollSensitivity;
    //     distance = Mathf.Clamp(scroll, minDistance, maxDistance);
    // }

    // void UpdatePosition(Vector3 lookAtPos)
    // {
    //     float da = azimuthalAngle * Mathf.Deg2Rad;
    //     float dp = polarAngle * Mathf.Deg2Rad;
    //     transform.position = new Vector3(
    //         lookAtPos.x + distance * Mathf.Sin(dp) * Mathf.Cos(da),
    //         lookAtPos.y + distance * Mathf.Cos(dp),
    //         lookAtPos.z + distance * Mathf.Sin(dp) * Mathf.Sin(da));
    //
    //     // Vector3.Lerp(transform.position, new Vector3(
    //     //     lookAtPos.x + distance * Mathf.Sin(dp) * Mathf.Cos(da),
    //     //     lookAtPos.y + distance * Mathf.Cos(dp),
    //     //     lookAtPos.z + distance * Mathf.Sin(dp) * Mathf.Sin(da)), Time.deltaTime * camMoveSpeed);
    //     // Lerp版カメラ
    // }
}