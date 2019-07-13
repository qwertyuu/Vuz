using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handtap : MonoBehaviour
{
    float tapPosition = 5f;
    float restPosition = -10f;
    int attackAmount = 0;
    float t = 0;
    float speed = 0.03f;

    void Start()
    {
        Conductor.OnAttack += a;
    }

    void a(float start, float max)
    {
        if (max >= -6) {
            t = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.Max(t - speed, 0);

        transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(restPosition, tapPosition, t));

    }
}
