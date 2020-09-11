using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FitForScreen : MonoBehaviour
{
    [SerializeField] private RectTransform CanvasRect;
    [SerializeField] private Sprite sprite;
    [SerializeField] private RectTransform ImageRect;
    private Vector2 LastCanvasSize;
    private Vector2 ImageSize;

    private void Start()
    {
        ImageSize = sprite.bounds.size * sprite.pixelsPerUnit;

        LastCanvasSize = CanvasRect.sizeDelta;
        bool narrow = LastCanvasSize.y / LastCanvasSize.x <= ImageSize.y / ImageSize.x;

        ImageRect.sizeDelta = narrow
            ? new Vector2(LastCanvasSize.x, ImageSize.y * LastCanvasSize.x / ImageSize.x)
            : new Vector2(ImageSize.x * LastCanvasSize.y / ImageSize.y, LastCanvasSize.y);
    }
}
