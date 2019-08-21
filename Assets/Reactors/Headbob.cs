using System;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    float bobbingAngle = 10f;
    float sideBobbingAngle = 5f;
    int attackAmount = 0;
    bool offset = false;
    float t = 0;
    float speed = 0.03f;

    private Vector3 originalPosition;

    void Awake()
    {
        Conductor.OnTick += a;
        Conductor.OnUpdateRelativeTick += UpdateRelativeTick;
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        t = Mathf.Max(t - speed, 0);

        transform.localRotation = Quaternion.Euler(bobbingAngle * t, Mathf.PingPong(attackAmount, 50), -(attackAmount % 2) * 2 * sideBobbingAngle + sideBobbingAngle);
    }

    private void UpdateRelativeTick(float relativeBarTime, float relativeBeatTime, float relativeTickTime)
    {
        transform.position = new Vector3(originalPosition.x + Mathf.Cos(Mathf.PI * (relativeBeatTime + (offset ? 1 : 0))) / 4, originalPosition.y, originalPosition.z);
    }

    void a(bool bar, bool beat)
    {
        if (beat) {
            offset = !offset;
            t = 1;
            attackAmount++;
        }
    }
}
