using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PalmRoot : MonoBehaviour, IWooden
{
    [SerializeField] private GameObject palmCore;
    [SerializeField] public List<Transform> coconuts;
    [SerializeField] private int haveWoods = 1;
    private bool isAlive = true;
    private bool hasLeaves = false;

    public bool CheckCanBreak(int count)
    {
        bool flag = haveWoods > 0;
        if (flag)
        {
            haveWoods += count;
            if (haveWoods <= 0)
            {
                Kill();
            }
        }

        return flag;
    }

    private void Kill()
    {
        Transform parent = palmCore.transform.parent.parent.Find("Coconuts");
        foreach (Transform coconut in coconuts)
        {
            coconut.SetParent(parent);
        }

        Sequence killAnim = DOTween.Sequence();
        killAnim.Append(transform.DOScale(Vector3.zero, 2f));
        killAnim.AppendCallback(() => Destroy(palmCore)).Play();
    }

    public bool IsAlive() => isAlive;
    public bool HasLeaves() => hasLeaves;
}