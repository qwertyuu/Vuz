using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 beatScale = new Vector3(2f, 2f, 2f);
    Vector3 normalScale = new Vector3(1f, 1f, 1f);
    float t = 0;
    float speed = 0.1f;


    void Start()
    {
        AudioProcessor.OnBeat += a;
    }

    void a()
    {
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.Min(speed + t, 1);

        transform.localScale = Vector3.Lerp(beatScale,normalScale , t);

    }
}
