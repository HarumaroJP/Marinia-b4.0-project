using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    Camera cam;
    [SerializeField] private Vector2 baseAspectValue = new Vector2(0, 0);
    [SerializeField] private float resPercent = 1;
    float baseAspect;

    void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
        baseAspect = baseAspectValue.x / baseAspectValue.y;
        // Screen.SetResolution(1600, 1000, false);
    }
    void Update()
    {
        if (cam.aspect != baseAspect)
            ChangeCameraAspect();
    }

    void ChangeCameraAspect()
    {
        float nowAspect = (float)Screen.width / (float)Screen.height;
        float changedAspect;

        if (nowAspect < baseAspect)
        {
            changedAspect = nowAspect / baseAspect;
            // cam.rect = new Rect((1 - changedAspect) * 0.5f, 0, changedAspect, 1);
            int res_w = (int)(Screen.width * changedAspect * resPercent);
            int res_h = (int)(Screen.height * changedAspect * resPercent);
        }
        else
        {
            changedAspect = baseAspect / nowAspect;
            // cam.rect = new Rect(0, (1 - changedAspect) * 0.5f, 1, changedAspect);
            int res_w = (int)(Screen.width * changedAspect * resPercent);
            int res_h = (int)(Screen.height * changedAspect * resPercent);
        }
    }
}
