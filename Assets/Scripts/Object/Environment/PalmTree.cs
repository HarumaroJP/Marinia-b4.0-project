using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PalmTree : MonoBehaviour, IWooden
{
    [SerializeField] private int maxHaveWoods;
    [SerializeField] private int haveWoods;
    [SerializeField] private float woodIncreaseInterval;
    [SerializeField] private Rigidbody rig;
    [SerializeField] private PalmRoot root;
    [SerializeField] private Transform treeRoot;
    [SerializeField] private MeshCollider colMain;
    [SerializeField] private Collider colRoot;
    [SerializeField] private Transform particleTransform;

    private Sequence killAnim;
    private bool isAlive = true;
    private bool hasLeaves = true;

    private void Awake()
    {
        InvokeRepeating(nameof(ChangeHaveWoods), 1f, woodIncreaseInterval);
        killAnim = DOTween.Sequence();

        killAnim.AppendInterval(6f);
        killAnim.Append(transform.DOScale(Vector3.zero, 2f));
        killAnim.AppendCallback(() => Destroy(gameObject));
    }

    private void ChangeHaveWoods()
    {
        haveWoods = (int) Mathf.Clamp(haveWoods + 1, 0f, maxHaveWoods);
    }

    public bool CheckCanBreak(int count)
    {
        bool flag = haveWoods > 0;
        if (flag)
        {
            // Debug.Log("counted");
            haveWoods += count;
            if (haveWoods <= 0)
            {
                Kill();
            }
        }

        return flag;
    }

    public bool IsAlive() => isAlive;
    public bool HasLeaves() => hasLeaves;

    private void Kill()
    {
        colMain.convex = true;
        colRoot.enabled = true;
        rig.isKinematic = false;
        gameObject.tag = "Untagged";
        treeRoot.tag = "CanTake";
        isAlive = false;

        particleTransform.GetComponent<ParticleSystem>().Stop();
        particleTransform.SetParent(treeRoot);

        foreach (Transform coconut in root.coconuts)
        {
            coconut.GetComponent<Rigidbody>().isKinematic = false;
        }

        killAnim.Play();
    }
}