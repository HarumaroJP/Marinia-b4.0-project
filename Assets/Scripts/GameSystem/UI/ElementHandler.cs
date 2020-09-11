using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image sourceImage;

    public void AddElement(Image data)
    {
        sourceImage = data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log(nameof(OnPointerEnter) + "_handler");
        if (sourceImage == null) return;
        if (eventData.dragging)
            sourceImage.raycastTarget = false;
        // Debug.Log(sourceImage.raycastTarget);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sourceImage == null) return;
        sourceImage.raycastTarget = true;
    }
}