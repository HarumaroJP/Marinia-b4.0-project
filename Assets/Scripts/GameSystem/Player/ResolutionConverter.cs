using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


/// <summary>
/// 解像度を変更するコンポーネント
/// </summary>
public class ResolutionConverter : MonoBehaviour
{
    [SerializeField] private float _scale = 1.0f;
    [SerializeField] private int _depth = 24;
    [SerializeField] private CameraEvent cameraEvent;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera subCamera;
    [SerializeField] private RawImage image;
    private Camera _backBufferCamera;
    private RenderTexture _frameBufferMain;
    private CommandBuffer _commandBuffer;

    private void Awake()
    {
        Initialize();
        Setup();
    }

    /// <summary>
    /// 初期化する
    /// </summary>
    private void Initialize()
    {
        GameObject backBufferCameraGo = new GameObject("Back Buffer Camera");
        _backBufferCamera = backBufferCameraGo.AddComponent<Camera>();
        _backBufferCamera.cullingMask = 0;
        _backBufferCamera.transform.parent = transform;
        _backBufferCamera.clearFlags = CameraClearFlags.Nothing;
        _backBufferCamera.useOcclusionCulling = false;
        _backBufferCamera.allowHDR = false;
        _backBufferCamera.allowMSAA = false;
        _backBufferCamera.allowDynamicResolution = false;
    }

    /// <summary>
    /// 今のScale値を元にセットアップする
    /// </summary>
    private void Setup()
    {
        // メインカメラのレンダーターゲットにスケーリングした解像度のRenderTextureを指定する
        if (_frameBufferMain != null)
        {
            _frameBufferMain.Release();
            Destroy(_frameBufferMain);
            _frameBufferMain = null;
        }

        int width = (int) (Screen.width * _scale);
        int height = (int) (Screen.height * _scale);

        _frameBufferMain = new RenderTexture(width, height, _depth, RenderTextureFormat.ARGB32)
        {
            useMipMap = false, filterMode = FilterMode.Bilinear
        };
        _frameBufferMain.Create();
        mainCamera.targetTexture = _frameBufferMain;
        subCamera.targetTexture = _frameBufferMain;

        // バックバッファ描画用のカメラにCommandBufferを設定する
        if (_commandBuffer != null)
        {
            _backBufferCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);
            _commandBuffer = null;
        }

        _commandBuffer = new CommandBuffer {name = "Blit to back buffer"};
        RectTransform rect = image.rectTransform;
        rect.sizeDelta = new Vector2(Screen.width, Screen.height);
        _commandBuffer.Blit((RenderTargetIdentifier) _frameBufferMain, BuiltinRenderTextureType.CameraTarget);
        _backBufferCamera.AddCommandBuffer(cameraEvent, _commandBuffer);
    }
}