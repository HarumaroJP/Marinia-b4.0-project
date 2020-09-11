using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrayCheck : MonoBehaviour
{
    int[] a = {1, 2, 3, 4, 5};
    private int[] a_return;
    [SerializeField] private StrInt[] array;

    [Serializable]
    public struct StrInt
    {
        public string key;
        public int value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // foreach (StrInt t in array)
            // {
            //     Debug.Log(t.key + " , " + t.value);
            // }

            StrInt tmp = array.FirstOrDefault(x => x.key == "test");
            tmp.value = 1000;

            // foreach (StrInt t in array)
            // {
            //     Debug.Log(t.key + " , " + t.value);
            // }

            // Array.Reverse(a);
            // Logger();
            // ArrayReverse_tmp(a);
            // Logger();
            // a_return = ArrayReverse_return();
            // Array.Reverse(a_return);
            // Logger();
        }
    }

    void Logger()
    {
        string str = "";
        foreach (int t in a)
        {
            str += t + " ";
        }

        // Debug.Log(str);
        a = new int[] {1, 2, 3, 4, 5};
    }

    void ArrayReverse_tmp(int[] tmp)
    {
        Array.Reverse(tmp);
    }

    int[] ArrayReverse_return()
    {
        return a;
    }

    public void OnLog(string str)
    {
        // Debug.Log("Enter to " + str);
    }
}