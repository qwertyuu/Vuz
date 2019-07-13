using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSpotifyPitch : MonoBehaviour
{
    void Start()
    {
        Conductor.OnSegment += a;
    }

    void a(Segment s)
    {
        for (int i = 0; i < s.pitches.Length; i++)
        {
            float currentPitch = s.pitches[i];
            transform.GetChild(i).transform.localScale = new Vector3(1, currentPitch * 10, 1);
        }
    }
}
