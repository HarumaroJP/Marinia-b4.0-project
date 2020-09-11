using System.Collections;
using UnityEngine;

public class SetMaterial : MonoBehaviour
{
    bool checkbool = false;
    public Material geTmaterial;
    public enum Mode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    void Start()
    {
        geTmaterial = GetComponent<MeshRenderer>().material;
    }

    public static void SetBlendMode(Material material, Mode blendMode)
    {
        material.SetFloat("_Mode", (float)blendMode);  // <= これが必要

        switch (blendMode)
        {
            case Mode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case Mode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case Mode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case Mode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    public void OnChangeMaterialOpacity(bool i)
    {
        if (i)
            SetBlendMode(geTmaterial, Mode.Transparent);
        else
            SetBlendMode(geTmaterial, Mode.Opaque);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!checkbool)
            {
                SetBlendMode(geTmaterial, Mode.Transparent);
                checkbool = true;
            }
            else
            {
                SetBlendMode(geTmaterial, Mode.Opaque);
                checkbool = false;
            }
        }
    }
}
