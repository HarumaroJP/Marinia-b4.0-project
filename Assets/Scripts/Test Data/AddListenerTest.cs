using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddListenerTest : MonoBehaviour
{
    [SerializeField] private UnityEvent testEvent;

    void Start()
    {
        testEvent.AddListener(Test);
    }

    public void Test()
    {
        // Debug.Log("Test() 実行しました");
    }

    public void TestWithInspector()
    {
        // Debug.Log("TestWithInspector() 実行しました");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            testEvent.Invoke();
    }
}