using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pracrice : MonoBehaviour
{

    int[] RandomArray = { 1, 1, 1, 1, 1, 1 };
    int MovingModeSet = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (MovingModeSet)
        {
            case 1:
                Debug.Log("A");
                break;
            case 2:
                Debug.Log("B");
                break;
            case 3:
                Debug.Log("C");
                break;
            default:
                break;
        }
    }
}
