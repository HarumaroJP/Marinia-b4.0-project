using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcoSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] Icos;
    [SerializeField] private Transform parent;
    private GameObject obj;
    [SerializeField] private float interval;
    [SerializeField] private Vector3[] vec;
    private int min = 0;
    private int max = 0;
    List<int> numbers = new List<int>();

    void Start()
    {
        for (int i = min; i < vec.Length; i++)
        {
            numbers.Add(i);
        }
        // foreach (Vector3 tmpVec in vec)
        // {
        //     GameObject obj = Instantiate(Icos[1],
        //         new Vector3(tmpVec.x, tmpVec.y, tmpVec.z), Quaternion.Euler(-90, 0, 0));
        // }

        //デバッグ用
        InvokeRepeating(nameof(CreateOre), 1f, interval);
    }

    private void CreateOre()
    {
        if (numbers.Count > 0)
        {
            int index = Random.Range(0, numbers.Count);
            int posIndex = numbers[index];

            Vector3 position = new Vector3(vec[posIndex].x, vec[posIndex].y, vec[posIndex].z);
            GameObject obj = Instantiate(Icos[Random.Range(0, 3)], parent, true);
            obj.GetComponent<OreManager>().myNumber = posIndex;
            obj.transform.localPosition = position;
            
            numbers.RemoveAt(index);
        }
        else
        {
            CancelInvoke(nameof(CreateOre));
        }
    }

    public void DestroyOre(int myNum)
    {
        numbers.Add(myNum);
        if (numbers.Count == 1)
        {
            InvokeRepeating(nameof(CreateOre), 1f, interval);
        }
    }
}