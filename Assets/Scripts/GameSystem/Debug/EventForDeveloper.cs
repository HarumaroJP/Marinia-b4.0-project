using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventForDeveloper : MonoBehaviour
{
    [SerializeField] private GameObject CommandLine;
    [SerializeField] private InputField CommandField;
    [SerializeField] private GameObject EntityField;
    [SerializeField] private Text commandText;
    [SerializeField] private Light DirectionalLight;
    GameObject RespawnPoint;
    GameObject Player;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        RespawnPoint = GameObject.FindWithTag("Respawn");
        // Player.transform.position = RespawnPoint.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Player.transform.position = RespawnPoint.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.None; //カーソルをアンロック 
            Cursor.visible = true; //カーソルを表示 


            SceneManager.LoadScene("Title");
        }
    }

    public void OpenCommandLine()
    {
        CommandLine.SetActive(true);
        CommandField.text = String.Empty;
        CommandField.ActivateInputField();
    }

    public void CloseCommandLine()
    {
        CommandLine.SetActive(false);
    }

    public void RunCommand(Text commandText)
    {
        string[] commandArr = commandText.text.Split(' ');

        switch (commandArr[0])
        {
            case "get":
                switch (commandArr[1])
                {
                    case "item":
                        GetElementByDeveloper(int.Parse(commandArr[2]), int.Parse(commandArr[3]));
                        break;

                    case "itemlist":
                        ItemLibrary.Instance.GetItemList();
                        break;


                    default:
                        // Debug.LogError("存在しないエレメントです！");
                        break;
                }

                break;

            case "timeset":
                if (commandArr.Length == 2)
                {
                    switch (commandArr[1])
                    {
                        case "day":
                            DirectionalLight.enabled = true;
                            break;

                        case "night":
                            DirectionalLight.enabled = false;
                            break;

                        default:
                            Debug.LogError("存在しない時間です！");
                            break;
                    }
                }
                else
                    Debug.LogError("不正な引数の数です！");

                break;

            case "entity":
                if (commandArr.Length == 2)
                {
                    switch (commandArr[1])
                    {
                        case "enable":
                            EntityField.SetActive(true);
                            break;

                        case "disable":
                            EntityField.SetActive(false);
                            break;

                        default:
                            Debug.LogError("存在しない時間です！");
                            break;
                    }
                }
                else
                    Debug.LogError("不正な引数の数です！");

                break;

            default:
                Debug.LogError("不正なコマンドです！");
                break;
        }

        CloseCommandLine();
        InGameMenu.Instance.OffUI(false);
    }

    void GetElementByDeveloper(int itemID, int count)
    {
        ItemData item = ItemLibrary.Instance.FindItems(itemID);
        ItemLibrary.Instance.AddItemsForMenu(item, count);
    }
}