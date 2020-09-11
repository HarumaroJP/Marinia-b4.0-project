using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeOutSystem : MonoBehaviour
{
    [SerializeField] private Image fadeOutPanel;
    Color color;
    [SerializeField] float timer = 0.035f;
    float r, g, b, alpha;

    public bool FadeCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        color = fadeOutPanel.color;
        r = color.r;
        g = color.g;
        b = color.b;
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeCheck)
        {
            OnFadeOut();
        }
    }

    void OnFadeOut()
    {
        alpha += timer;
        fadeOutPanel.color = new Color(r, g, b, alpha);
        if (alpha > 1)
        {
            SceneManager.LoadScene("LoadDisplay");
        }
    }
}