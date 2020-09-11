using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DictionaryCheck : MonoBehaviour
{
    private Dictionary<string, int> dic1 = new Dictionary<string, int>();
    private Dictionary<string, int> dic2 = new Dictionary<string, int>();

    void Start()
    {
        dic1.Add("test", 2);
        dic2.Add("test2", 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // dic1.Add("test2", 2);
            // dic1.Add("test", 2);

            bool flag1 = false;

            foreach (string t1 in dic1.Keys)
            {
                foreach (string t2 in dic2.Keys)
                {
                    if (t1 == t2)
                        flag1 = true;
                }
            }

            Debug.Log("result1 = " + flag1);

            IEnumerable<bool> flag2 =
                from data in dic1.Keys
                from data_end in dic1.Keys
                select data == data_end;

            Debug.Log("result2 = " + flag2.Single());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // foreach (var key in dic1.Keys)
            // {
            //     Debug.Log(key);
            // }
            //
            // foreach (var value in dic1.Values)
            // {
            //     Debug.Log(value);
            // }
        }
    }
}