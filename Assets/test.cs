using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 beatScale = new Vector3(3f, 0, 0);
    Vector3 midScale = new Vector3(0, 3f, 0);
    Vector3 normalScale = new Vector3(1f, 1f, 1f);
    float t = 0;
    float m = 0;
    float speed = 0.1f;
    Renderer rend;
    float largest;
    bool hasMid = false;


    void Start()
    {
        AudioProcessor.OnBeat += a;
        AudioProcessor.OnSpectrum += b;
        rend = GetComponent<Renderer>();
    }

    void a()
    {
        t = 1;
    }

    void b(float[] j)
    {
        float k = j[9];
        if (k > 0.006f) { // TODO: Make that cleverer by checking how AudioProcessor works
            // on mid
            if (!hasMid) {
                mid();
            }
            hasMid = true;
        } else {
            hasMid = false;
        }
        if (k > largest) {
            largest = k;
            Debug.Log(largest);
        }
        rend.material.color = new Color(k * 50, 0f, 0f);
    }

    void mid()
    {
        m = 1;
    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.Max(t - speed, 0);
        m = Mathf.Max(m - speed, 0);

        transform.localScale = normalScale + (m * midScale + t * beatScale);

    }
}
