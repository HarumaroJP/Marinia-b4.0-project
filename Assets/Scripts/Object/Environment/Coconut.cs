using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coconut : MonoBehaviour
{
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private Rigidbody rig;
    [SerializeField] private MeshRenderer mat;
    [SerializeField] private float fallTime;
    [SerializeField] private float colorChangeDuration;
    public int growthLevel;


    private readonly Color[] _colors =
        {new Color(35f, 65f, 0f), new Color(169f, 142f, 48f), new Color(75f, 31f, 7f)};

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(ChangeColor), 0f, 1f);
    }

    public void LoadSaveData()
    {
        string jsonData = itemManager.GetSaveData();

        if (jsonData != String.Empty)
        {
            Coconut saveClassData = JsonUtility.FromJson<Coconut>(jsonData);
            growthLevel = saveClassData.growthLevel;
        }
    }

    // Update is called once per frame
    void ChangeColor()
    {
        Color unitColor = _colors[growthLevel] / 255f;
        var colorAnim =
            DOTween.To(() => mat.material.color, color => mat.material.color = color, unitColor, colorChangeDuration);
        colorAnim.Play();
        growthLevel++;

        if (growthLevel == 3)
        {
            CancelInvoke(nameof(ChangeColor));
            Invoke(nameof(OnFalling), fallTime);
        }
    }

    public void OnFalling()
    {
        rig.isKinematic = false;
    }

    public string GetObjectData()
    {
        return JsonUtility.ToJson(this, false);
    }
}