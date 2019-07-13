using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    float bobbingAngle = 10f;
    float sideBobbingAngle = 5f;
    int attackAmount = 0;
    float t = 0;
    float speed = 0.03f;

    void Start()
    {
        Conductor.OnTick += a;
    }

    void a(bool bar, bool beat)
    {
        if (beat) {
            t = 1;
            attackAmount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.Max(t - speed, 0);

        transform.localRotation = Quaternion.Euler(bobbingAngle * t, Mathf.PingPong(attackAmount, 50), -(attackAmount % 2) * 2 * sideBobbingAngle + sideBobbingAngle);

    }
}
