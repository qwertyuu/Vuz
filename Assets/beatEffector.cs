using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatEffector : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 beatScale = new Vector3(3f, 0, 0);
    Vector3 normalScale = new Vector3(1f, 1f, 1f);
    float t = 0;
    float speed = 0.1f;

    void Start()
    {
        Conductor.OnTick += a;
    }

    void a(bool bar, bool beat)
    {
            t = 1;
    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.Max(t - speed, 0);

        transform.localScale = normalScale + (t * beatScale);

    }
}
