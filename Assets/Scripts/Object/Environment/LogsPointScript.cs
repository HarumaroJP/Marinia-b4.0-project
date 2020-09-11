using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogsPointScript : MonoBehaviour
{
    [SerializeField] GameObject Prefabwood;
    GameObject obj;
    [SerializeField] private float interval = 10f;
    [SerializeField] private float tmpTime = 0;
    [SerializeField] private int maxNumOfWoods;
    private int numberOfWoods = 0;
    private Vector3 thisTransform;
    public GameObject[] parent;

    // Update is called once per frame
    void FixedUpdate()
    {
        tmpTime += Time.deltaTime;
        if (tmpTime >= interval)
        {
            if (numberOfWoods < maxNumOfWoods)
            {
                thisTransform = transform.position;
                GameObject obj = Instantiate(Prefabwood,
                    new Vector3(thisTransform.x, thisTransform.y, thisTransform.z), Quaternion.identity);
                int number = Random.Range(0, 4);
                obj.transform.parent = parent[number].transform;
                obj.transform.localPosition =
                    new Vector3(Random.Range(-245, 245), -1, thisTransform.z);

                numberOfWoods++;
                tmpTime = 0;
                // Debug.Log(numberOfWoods);
            }
        }
    }
}