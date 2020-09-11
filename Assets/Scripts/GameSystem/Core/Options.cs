using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Options : MonoBehaviour
{
    //ゲームオプションパラメータ
    Slider SensiValue; //感度
    float max = 10f;
    Toggle MotionToggle; //モーションブラー
    [SerializeField] private ToggleGroup resGroup;
    [SerializeField] private GameObject resWarn;
    Toggle Res_defSelect;


    CameraRotate CameraRotate;
    MotionBlur MotionBlur;

    void Start()
    {
        CameraRotate = Camera.main.GetComponent<CameraRotate>();
        SensiValue = transform.GetChild(0).gameObject.GetComponent<Slider>();
        MotionToggle = transform.GetChild(1).gameObject.GetComponent<Toggle>();
        MotionBlur = ScriptableObject.CreateInstance<MotionBlur>();

        SensiValue.maxValue = max;
        resWarn.SetActive(false);
    }

    public void ShowResCheck()
    {
        if (Res_defSelect != resGroup.ActiveToggles().FirstOrDefault())
        {
            resWarn.SetActive(true);
        }
        else
        {
            resWarn.SetActive(false);
        }
    }

    public void OnDisplay()
    {
        Res_defSelect = resGroup.ActiveToggles().FirstOrDefault();
        SensiValue.value = CameraRotate.sensi;
    }

    public void OffDisplay()
    {
        CameraRotate.sensi = SensiValue.value;

        if (MotionToggle.isOn)
        {
            MotionBlur.enabled.Override(true);
        }
        else
        {
            MotionBlur.enabled.Override(false);
        }
        //
        // switch (resGroup.ActiveToggles().FirstOrDefault().ToString())
        // {
        //     case "Low (UnityEngine.UI.Toggle)":
        //         LiteRender._scale = 0.9f;
        //         break;
        //     case "Normal (UnityEngine.UI.Toggle)":
        //         LiteRender._scale = 1.5f;
        //         break;
        //     case "High (UnityEngine.UI.Toggle)":
        //         LiteRender._scale = 2.8f;
        //         break;
        //     default:
        //         Debug.LogError("存在しない解像度パラメータです");
        //         break;
        // }
        PostProcessManager.instance.QuickVolume(0, 1, MotionBlur);
    }
}