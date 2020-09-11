using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyScriot : MonoBehaviour
{
    Vector3 TestVec
    {
        get
        {
            return new Vector3(1, 1, 1);
        }
    }

    [SerializeField] private float AxisX = 5f;

    Vector3 TestVec2
    {
        get
        {
            return new Vector3(AxisX, 1, 1);
        }
    }

    Vector3 Testfloat
    {
        get
        {
            return new Vector3(-AxisX, 1, 1);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(TestVec);
        // Debug.Log(TestVec2);
        // Debug.Log(Testfloat);
    }
}
