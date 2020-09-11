using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delegateReciever : MonoBehaviour
{
    private Transform playerObjects;

    struct TestMethodStructType
    {
        public TestMethodStructType(Delegater.TestMethodType data)
        {
            // Debug.Log(data);
            data.Invoke();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerObjects = GameObject.Find("delegateCheck").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameObject obj = GameObject.Find("delegateCheck");

            obj.GetComponent<Delegater>().testMethod.Invoke();

            foreach (Transform trans in playerObjects)
            {
                Delegater.TestMethodType data = trans.GetComponent<Delegater>().testMethod;
                Debug.Log(data);
                TestMethodStructType testInstance = new TestMethodStructType(data);
            }
        }
    }
}