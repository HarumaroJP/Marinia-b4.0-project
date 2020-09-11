using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private Image[] ParamImgsList;
    [SerializeField] public float[] ParamValues;
    [SerializeField] private int staminaIncreaseNum;
    [SerializeField] private int drinkAndfoodDecreaseNum;
    [SerializeField] private Image hurtImage;
    [SerializeField] private Rigidbody playerRig;
    [SerializeField] private Control control;

    public enum Status
    {
        Health = 0,
        Stamina = 1,
        Drink = 2,
        Food = 3
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(IncreaseStamina), 1f, 10f);
        InvokeRepeating(nameof(IncreaseHealth), 3f, 10f);
    }

    private void IncreaseStamina()
    {
        // Debug.Log(nameof(IncreaseStamina));
        // if (ParamValues[1] < 100f && ParamValues[2] > 0f && ParamValues[3] > 0f)
        // {
        //     SetStatusParam(Status.Stamina, staminaIncreaseNum);
        //     SetStatusParam(Status.Drink, -drinkAndfoodDecreaseNum);
        //     SetStatusParam(Status.Food, -drinkAndfoodDecreaseNum);
        // }
        if (ParamValues[1] < 100f)
        {
            SetStatusParam(Status.Stamina, staminaIncreaseNum);
        }
    }

    private void IncreaseHealth()
    {
        if (ParamValues[0] < 100f && ParamValues[1] >= staminaIncreaseNum)
        {
            SetStatusParam(Status.Stamina, -staminaIncreaseNum);
            SetStatusParam(Status.Health, staminaIncreaseNum);
        }
    }

    private void OnPlay(int value)
    {
        Tweener paramsAnimElement = DOTween.To(() => ParamImgsList[value].fillAmount,
            i => ParamImgsList[value].fillAmount = i,
            ParamValues[value] / 100f, 0.5f);

        paramsAnimElement.Play();
    }

    private void GetHurt()
    {
        Color t = hurtImage.color;
        t.a = 0.0f;
        hurtImage.color = t;

        Tweener hurtAnim = DOTween.ToAlpha(() => hurtImage.color, i => hurtImage.color = i, 0.3f, 0.3f)
            .SetLoops(2, LoopType.Yoyo);

        hurtAnim.Play();
    }

    /// <summary>
    /// 有効なパラメーター名
    /// ・ Health
    /// ・　Stamina
    /// ・　Drink
    /// ・ Food
    /// </summary>
    public void SetStatusParam(Status name, float value)
    {
        Debug.Log(name);
        Debug.Log(value);
        int index = (int) name;

        //TODO
        //・100超えてたら最後のelseに飛ばされて体力がゼロになってしまうっぽいのでそこを直してね！

        if (index == 0 && ParamValues[index] + value <= 0f)
        {
            ParamValues[index] = 0f;
            OnPlay(index);
            OnDeath();
        }
        else if (ParamValues[index] + value >= 100f || ParamValues[index] + value <= 0f)
        {
            ParamValues[index] = ParamValues[index] + value >= 100f ? 100f : 0f;
            OnPlay(index);
        }
        else
        {
            ParamValues[index] += value;
            OnPlay(index);
        }

        if (name == Status.Health && value < 0f)
        {
            GetHurt();
        }
    }

    private void OnDeath()
    {
        playerRig.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        control.isPlaying = false;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1f);
        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(false, InGameMenu.FadeType.Normal));
        sequence.AppendInterval(7f);
        sequence.AppendCallback(() =>
        {
            control.transform.position = new Vector3(150.35f, -20.57f, 152.69f);
            control.transform.rotation = Quaternion.Euler(0f, 212.462f, 0f);
            playerRig.isKinematic = false;
            playerRig.constraints = RigidbodyConstraints.FreezeRotation;
            control.isPlaying = true;

            ParamValues[0] = 100f;
            ParamValues[1] = 100f;

            OnPlay(0);
            OnPlay(1);
        });
        sequence.AppendCallback(() => InGameMenu.Instance.OnFadeDown(true, InGameMenu.FadeType.Normal));
        sequence.AppendInterval(7f);
        sequence.Play();
    }
}