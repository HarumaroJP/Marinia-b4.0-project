using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PngSequenceAnimator : MonoBehaviour
{
    [SerializeField] private Image _targetImage;
    [SerializeField] private string _folderPath;
    [SerializeField] private int _startNumber;
    [SerializeField] private int _endNumber;
    [SerializeField] private float _frameInterval = 0.1f;
    [SerializeField] private Sprite[] _animatedPngSpriteList;
    private int _currentFrame = 0;
    private bool _isAnimation = false;

    private void Awake()
    {
        SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>("LoadingAtlas");
        _animatedPngSpriteList = new Sprite[spriteAtlas.spriteCount];
        for (int i = 0; i < _animatedPngSpriteList.Length; i++)
        {
            _animatedPngSpriteList[i] = spriteAtlas.GetSprite($"{i:0000}");
        }

        StartAnimation();
    }

    [ContextMenu("StartAnimation")]
    public void StartAnimation()
    {
        _isAnimation = true;
        StartCoroutine(UpdatePNG());
    }

    [ContextMenu("StopAnimation")]
    public void StopAnimation()
    {
        _isAnimation = false;
    }

    private IEnumerator UpdatePNG()
    {
        while (_isAnimation)
        {
            _targetImage.sprite = _animatedPngSpriteList[_currentFrame];
            _currentFrame++;
            if (_currentFrame >= _endNumber) _currentFrame = 0;
            yield return new WaitForSeconds(_frameInterval);
        }
    }
}