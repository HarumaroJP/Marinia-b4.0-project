using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    List<CharaList> CharaData = new List<CharaList>();

    bool isMoving = false;
    float moveBlock = 3f;
    float Timer;

    [System.Serializable]
    public struct CharaList
    {
        public string name;
        public GameObject EntityObjects;
        public GameObject BattleAngle;
    }

    void Start()
    {
        CharaData.Add(new CharaList()
        {
            name = "Player",

            EntityObjects = GameObject.FindWithTag("Player"),
            BattleAngle = GameObject.FindWithTag("Player").transform.GetChild(1).gameObject
        });
        CharaData.Add(new CharaList()
        {
            name = "Boss",

            EntityObjects = GameObject.FindWithTag("Boss"),
            BattleAngle = GameObject.FindWithTag("Boss").transform.GetChild(1).gameObject
        });
    }

    void Update()
    {
        if (isMoving) //タイマー
            Timer += Time.deltaTime;
        else
            Timer = 0;

        if (Timer < 1 && isMoving) //BattleAngleというオブジェクトの前方向に進む
            CharaData[0].EntityObjects.transform.position += CharaData[0].BattleAngle.transform.forward * moveBlock * Time.deltaTime;
        //これだけだとPlayerしか移動しない

        else if (Timer > 1 && isMoving)
            isMoving = false;


        if (Input.GetKeyDown(KeyCode.Space)) //スペースキーを押したら移動開始
            isMoving = true;
    }
}
