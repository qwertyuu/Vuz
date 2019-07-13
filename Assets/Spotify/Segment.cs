using System;

[Serializable]
public class Segment
{
    public float start;
    public float duration;
    public float confidence;
    public float loudness_start;
    public float loudness_max_time;
    public float loudness_max;
    public float[] pitches;
}
