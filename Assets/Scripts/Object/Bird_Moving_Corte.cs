using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_Moving_Corte : MonoBehaviour {
    [SerializeField] private float Bird_Speed = 2f;
    float MovingTime = 0f;
    int[] BirdMovingPattern = { 81, -240, -519 };

    bool Bird_Movable = false;

    // Update is called once per frame
    void Update () {
        if (MovingTime >= 25) {
            MovingTime = 0f;
            Bird_Movable = false;
        } else if (Bird_Movable) {
            transform.Translate (0, 0, -Bird_Speed * Time.deltaTime);
            MovingTime += Time.deltaTime;
        } else if (!Bird_Movable) {
            int i = Random.Range (0, BirdMovingPattern.Length);
            transform.position = new Vector3 (BirdMovingPattern[i], 151, 2540);
            Bird_Movable = true;
        }
    }
}