using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Practice : MonoBehaviour
{
    int i = 3; //入力された値
    private int[] onevalue = { 1, 2, 3, 4, 5 };
    public int OneValue
    {
        get
        {
            int ret = onevalue[i] + 5; //iはどうやって取得？
            return ret;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(OneValue); // ここの3が、上記iに代入されて、9が出力されてほしい。
    }

    // Update is called once per frame
    void Update()
    {

    }
}
