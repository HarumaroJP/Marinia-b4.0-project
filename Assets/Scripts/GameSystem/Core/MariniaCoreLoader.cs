using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MariniaCoreLoader : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Image progressElement;
    [SerializeField] private Image[] loadingString;
    [SerializeField] private string[] objectOriginDataList;
    [SerializeField] private string[] environmentDataList;
    [SerializeField] private string[] uiElementList;
    [SerializeField] private string[] monsterDataList;
    [SerializeField] private float loadingInterval;
    [SerializeField] private Tweener progressBar;
    [SerializeField] private Text progressCountText;

    private ResourceRequest request;
    private int progressCount = 0;
    private float tmpProgress;
    private AsyncOperation async;

    private string[] splitString = {"@add"};
    private char[] nameChecker = {'%', '%'};

    private float progressValue
    {
        get
        {
            Debug.Log("progressValue has called" + progressCount);
            return Mathf.InverseLerp(0f, 6f, progressCount);
        }
    }

// Start is called before the first frame update
    void Start()
    {
        // Screen.SetResolution(Screen.width, Screen.height, true);

        Debug.Log("Loading...");

        objectOriginDataList = new string[itemDatabase.GetItemCount()];
        int count = 0;
        foreach (ItemData element in itemDatabase.GetItemList())
        {
            objectOriginDataList[count] = element.GetFileName();
            count++;
        }

        monsterDataList = new string[monsterDatabase.GetMonsterCount()];
        count = 0;
        foreach (MonsterData monster in monsterDatabase.GetMonsterList())
        {
            monsterDataList[count] = monster.GetFileName();
            count++;
        }

        Debug.Log(progressElement.material);
        progressElement.material.SetFloat("_FillAmount", 0f);
        progressBar = progressElement.material.DOFloat(progressValue, "_FillAmount", 2f);

        // progressBar = DOTween.To(() => progressElement.material.GetFloat("_FillAmount"),
        //     value => progressElement.material.SetFloat("_FillAmount", value),
        //     Mathf.InverseLerp(0, 4, progressCount), 2f);

        InvokeRepeating(nameof(UpdateLoadingString), 1f, 0.5f);
        StartCoroutine(nameof(LoadGame));
    }

    private IEnumerator LoadGame()
    {
        Debug.Log("オブジェクトのリソースを読み込み中...");
        foreach (string element in objectOriginDataList)
        {
            request = Resources.LoadAsync<GameObject>("ObjectData/MainData/" + element);

            if (request.asset != null)
                Marinia.ObjectList_Build.Add(request.asset as GameObject);
        }

        UpdateLoadingBackGround();
        yield return new WaitForSeconds(loadingInterval);

        foreach (string element in objectOriginDataList)
        {
            request = Resources.LoadAsync<GameObject>("ObjectData/HoldData/" + element);

            if (request.asset != null)
                Marinia.ObjectList_Hold.Add(request.asset as GameObject);
        }

        UpdateLoadingBackGround();
        yield return new WaitForSeconds(loadingInterval);

        foreach (string element in objectOriginDataList)
        {
            request = Resources.LoadAsync<GameObject>("ObjectData/FunctionalData/" + element);

            if (request.asset != null)
                Marinia.ObjectList_Natural.Add(request.asset as GameObject);
        }

        UpdateLoadingBackGround();
        yield return new WaitForSeconds(loadingInterval);

        Debug.Log("環境データを読み込み中...");
        foreach (string element in environmentDataList)
        {
            request = Resources.LoadAsync<GameObject>("ObjectData/EnvironmentData/" + element);

            if (request.asset != null)
                Marinia.ObjectList_Environment.Add(request.asset as GameObject);
        }

        UpdateLoadingBackGround();
        yield return new WaitForSeconds(loadingInterval);

        Debug.Log("アイテムデータを読み込み中...");
        foreach (string element in uiElementList)
        {
            request = Resources.LoadAsync<GameObject>("NavUIData/" + element);

            if (request.asset != null)
                Marinia.UIObjectList.Add(request.asset as GameObject);
        }

        UpdateLoadingBackGround();
        yield return new WaitForSeconds(loadingInterval);
        AppendPlayerNameOnEventText();

        UpdateLoadingBackGround();
        yield return new WaitForSeconds(loadingInterval);
        foreach (string monster in monsterDataList)
        {
            request = Resources.LoadAsync<GameObject>("MonsterData/" + monster);

            if (request.asset != null)
                Marinia.MonsterObjectList.Add(request.asset as GameObject);
        }

        // PreLoadScene("TreeIsland");
        SceneManager.LoadScene("TreeIsland");
        Debug.Log("Loading completed");
        yield break;
    }

    // private void PreLoadScene(string sceneName)
    // {
    //     async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    //     async.allowSceneActivation = false;
    // }

    private void AppendPlayerNameOnEventText()
    {
        TextAsset[] eventData_impact = Resources.LoadAll("TextData", typeof(TextAsset)).Cast<TextAsset>().ToArray();

        for (int index = 0; index < eventData_impact.Length; index++)
        {
            TextAsset textAsset = eventData_impact[index];
            string tmpText = textAsset.text;
            string[] splitText = tmpText.Split(splitString, StringSplitOptions.None);
            for (int i = 0; i < splitText.Length; i++)
            {
                splitText[i] = splitText[i].Replace("\n", "");
                string[] strings = splitText[i].Split(nameChecker, StringSplitOptions.RemoveEmptyEntries);
                if (strings.Length > 1)
                {
                    for (int j = 0; j < strings.Length; j++)
                    {
                        if (strings[j] == "PlayerName") strings[j] = Marinia.playerName;
                    }
                }

                splitText[i] = String.Concat(strings);
            }

            switch (textAsset.name)
            {
                case "ImpactText":
                    Marinia.eventMessageList_impact = splitText.ToList();
                    break;

                case "PlayerText":
                    Marinia.eventMessageList_player = splitText.ToList();
                    break;
            }
        }
    }

    private void UpdateLoadingBackGround()
    {
        progressCount++;
        progressBar = progressElement.material.DOFloat(progressValue, "_FillAmount", 1f).SetEase(Ease.OutBack);
        progressBar.Play();
    }

    private int countString;

    private void UpdateLoadingString()
    {
        if (countString == 3)
        {
            foreach (Image image in loadingString)
            {
                image.enabled = false;
            }

            countString = 0;
        }
        else
        {
            loadingString[countString].enabled = true;
            countString++;
        }
    }
}