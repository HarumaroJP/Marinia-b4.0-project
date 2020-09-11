using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class MagicRock : MonoBehaviour, IUsable
{
    [SerializeField] private ItemData allowData;
    [SerializeField] private ToolSetter toolSetter;
    [SerializeField] private int allowCounter;
    [SerializeField] private List<Renderer> renderers;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }


    public void Initialize()
    {
        Debug.Log("Initialize");
        if (ItemLibrary.Instance.itemSlotIDList[toolSetter.activeIndex].key != allowData) return;
        renderers[allowCounter].enabled = true;
        allowCounter++;
        ItemLibrary.Instance.RemoveItemsForMenu(allowData, 1);
        if (allowCounter == renderers.Count)
        {
            DOVirtual.DelayedCall(1f, () => PhaseSystem.Instance.StorageMagicRock()).Play();
        }
    }
}