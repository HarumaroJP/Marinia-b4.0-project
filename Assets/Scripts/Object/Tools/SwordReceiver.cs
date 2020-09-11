using DG.Tweening;
using UnityEngine;

public class SwordReceiver : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Collider col;
    [SerializeField] private int tmpItemDamage;

    private void Start()
    {
        tmpItemDamage = itemData.GetItemDamage();
    }

    public void OnHitCheck(bool trigger)
    {
        col.enabled = trigger;
    }

    private void OnTriggerEnter(Collider other1)
    {
        MonsterManager manager = other1.transform.GetComponent<MonsterManager>();

        if (manager == null) return;
        hitParticle.Play();
        manager.SetStatusParam(-tmpItemDamage);
    }
}