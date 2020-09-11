using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class MagicField : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private Transform circle;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Renderer[] renderer;
    private ISummonable summonable;

    void Start()
    {
        summonable = parent.GetComponent<ISummonable>();
        Play();
    }

    private void Play()
    {
        Sequence seq = DOTween.Sequence();
        float circleSize = circle.localScale.x;
        circle.localScale = Vector3.zero;
        foreach (Renderer rend in renderer) rend.enabled = false;

        seq.AppendCallback(() => summonable.Stop());
        seq.Append(circle.DOScale(circleSize, 1f));
        seq.Join(circle.DOLocalRotate(
            new Vector3(circle.localRotation.eulerAngles.x, 90f, circle.localRotation.eulerAngles.z), 2f));
        seq.AppendCallback(() => particle.Play());
        seq.AppendInterval(2f);
        seq.AppendCallback(() =>
        {
            foreach (Renderer rend in renderer) rend.enabled = true;
        });
        seq.AppendInterval(1f);
        seq.AppendCallback(() => particle.Stop());
        seq.Append(circle.DOScale(0f, 1f));
        seq.Join(circle.DOLocalRotate(
            new Vector3(circle.localRotation.eulerAngles.x, 0f, circle.localRotation.eulerAngles.z), 2f));
        seq.AppendCallback(() => summonable.Play());
        seq.Play().OnComplete(() => Destroy(gameObject));
    }
}