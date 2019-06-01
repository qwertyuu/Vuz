using System;

[Serializable]
public class Track
{
    public Segment[] segments;
    public Beat[] beats;
    public Bar[] bars;
    public Tatum[] tatums;
}
