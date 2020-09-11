using UnityEngine;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour
{
    public string[] scenarios;
    Text uiText;

    [SerializeField]
    [Range(0.001f, 0.3f)]
    float intervalForCharacterDisplay = 0.05f;

    private string currentText = string.Empty;
    private float timeUntilDisplay = 0;
    private float timeElapsed = 1;
    private int currentLine = 0;
    private int lastUpdateCharacter = -1;


    public bool IsCompleteDisplayText
    {
        get { return Time.time > timeElapsed + timeUntilDisplay; }
    }

    void Start()
    {
        GameObject Door = transform.GetChild(0).gameObject;
        GameObject TalkCanvas001 = GameObject.FindWithTag("PanelCanvas");
        GameObject TalkPanel = (GameObject)Resources.Load("TalkUIData/TalkUI");
        GameObject InstantiateTalkPanel = Instantiate(TalkPanel) as GameObject;
        RectTransform RectTfm = InstantiateTalkPanel.GetComponent<RectTransform>();
        uiText = InstantiateTalkPanel.GetComponentInChildren<Text>();
        InstantiateTalkPanel.transform.parent = TalkCanvas001.transform;
        Vector3 UIOffSet = new Vector3(-2f, 1.5f, 0);
        RectTfm.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Door.transform.position + UIOffSet);
        InstantiateTalkPanel.SetActive(true);
        SetNextLine();
    }

    void Update()
    {

        if (IsCompleteDisplayText)
        {
            if (currentLine < scenarios.Length && Input.GetMouseButtonDown(0))
            {
                SetNextLine();
            }
        }
        else
        {

            if (Input.GetMouseButtonDown(0))
            {
                timeUntilDisplay = 0;
            }
        }

        int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
        if (displayCharacterCount != lastUpdateCharacter)
        {
            uiText.text = currentText.Substring(0, displayCharacterCount);
            lastUpdateCharacter = displayCharacterCount;
        }
    }


    void SetNextLine()
    {
        currentText = scenarios[currentLine];
        timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
        timeElapsed = Time.time;
        currentLine++;
        lastUpdateCharacter = -1;
    }
}