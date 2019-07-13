using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handbreathe : MonoBehaviour
{
    float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        float amplitude = 5f;
        float r = Mathf.Cos(Time.time / speed) * amplitude + (amplitude / 2);

        transform.localRotation = Quaternion.Euler(r, r, r);

    }
}
