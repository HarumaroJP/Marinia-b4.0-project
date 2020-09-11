using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkySystem : MonoBehaviour
{
    [SerializeField] private EnvironmentManager environmentManager;
    [SerializeField] public Light sun;
    [SerializeField] private Renderer waterRenderer;
    [SerializeField] private Material skyBoxMainMaterial;
    [SerializeField] private SkyInfo[] skyBoxTextures;
    [SerializeField] private float updateSkyBoxInterval;
    [SerializeField] private float blendDuration;
    private int tmpSkyNumber;
    private int skyNumber => (int) Mathf.Repeat(tmpSkyNumber, 3);
    private int skyNumberPlus => (int) Mathf.Repeat(skyNumber + 1, 3);

    [Serializable]
    public struct SkyInfo
    {
        public Cubemap skyBox;
        public float intensity;
        public float waveVisibility;
        public Color waveColor;
        public bool isLightFlowers;
    }

    // Update is called once per frame
    private void Start()
    {
        RenderSettings.skybox = skyBoxMainMaterial;
    }

    private readonly int tex = Shader.PropertyToID("_Tex");
    private readonly int tex2 = Shader.PropertyToID("_Tex2");
    private readonly int BlendCubeMaps = Shader.PropertyToID("_BlendCubeMaps");
    private readonly int WaterColor = Shader.PropertyToID("_WaterColor");
    private static readonly int TextureVisibility = Shader.PropertyToID("_TextureVisibility");
    private float blendTime;
    private float blendWaveVisibility;
    private Color blendColor;

    public void StopSkySystem()
    {
        CancelInvoke();
    }

    public void StartSkySystem()
    {
        InvokeRepeating(nameof(OnSkyBoxBlender), updateSkyBoxInterval - 10f, updateSkyBoxInterval);
        skyBoxMainMaterial.SetTexture(tex, skyBoxTextures[skyNumber].skyBox);
        skyBoxMainMaterial.SetTexture(tex2, skyBoxTextures[skyNumber + 1].skyBox);
        skyBoxMainMaterial.SetFloat(BlendCubeMaps, 0f);
        blendColor = skyBoxTextures[skyNumber].waveColor;
        waterRenderer.material.SetColor(WaterColor, blendColor);
        blendWaveVisibility = skyBoxTextures[skyNumber].waveVisibility;
        waterRenderer.material.SetFloat(TextureVisibility, blendWaveVisibility);
        sun.intensity = skyBoxTextures[skyNumber].intensity;
        InvokeRepeating(nameof(ChangeSkyBox), updateSkyBoxInterval, updateSkyBoxInterval);
    }

    private void OnSkyBoxBlender()
    {
        // Debug.Log(nameof(OnSkyBoxBlender));
        sun.DOIntensity(skyBoxTextures[skyNumberPlus].intensity, blendDuration).Play();
        DOTween.To(() => blendColor, color => blendColor = color, skyBoxTextures[skyNumberPlus].waveColor,
            blendDuration).Play();
        DOTween.To(() => blendWaveVisibility, visibility => blendWaveVisibility = visibility,
            skyBoxTextures[skyNumberPlus].waveVisibility, blendDuration).Play();
        DOTween.To(() => blendTime, x => blendTime = x, 1f, blendDuration).OnUpdate(BlendSkyBox).Play();

        environmentManager.SetLightFlower(skyBoxTextures[skyNumberPlus].isLightFlowers);
    }


    private void BlendSkyBox()
    {
        skyBoxMainMaterial.SetFloat(BlendCubeMaps, blendTime);
        waterRenderer.material.SetColor(WaterColor, blendColor);
        waterRenderer.material.SetFloat(TextureVisibility, blendWaveVisibility);
    }


    void ChangeSkyBox()
    {
        blendTime = 0f;
        skyBoxMainMaterial.SetFloat(BlendCubeMaps, 0f);
        tmpSkyNumber++;

        skyBoxMainMaterial.SetTexture(tex, skyBoxTextures[skyNumber].skyBox);
        skyBoxMainMaterial.SetTexture(tex2, skyBoxTextures[skyNumberPlus].skyBox);
    }
}