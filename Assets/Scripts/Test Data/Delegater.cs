using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delegater : MonoBehaviour
{
    public delegate void TestMethodType();

    public TestMethodType testMethod;

    private void Start()
    {
        testMethod = ShowResult;
    }

    public void ShowResult()
    {
        Debug.Log("実行されました");
    }
}