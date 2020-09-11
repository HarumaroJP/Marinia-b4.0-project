using DG.Tweening;
using UnityEngine;

public class DoSizeDelta_test : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    private Vector2 size_origin = new Vector2(100f, 100f);
    private Vector2 size_select = new Vector2(120f, 120f);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var anim = rect.DOSizeDelta(size_select, 1f);
            anim.Play();
            Debug.Log("keydown");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var anim = rect.DOSizeDelta(size_origin, 1f);
            anim.Play();
            Debug.Log("keyup");
        }
    }
}