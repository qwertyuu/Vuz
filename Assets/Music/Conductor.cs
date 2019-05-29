using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Conductor : SceneSingleton<Conductor>
{
    event System.Action _onBeat = delegate { };
    public static event System.Action OnBeat
    {
        add { Instance._onBeat += value; }
        remove { Instance._onBeat -= value; }
    }

	public static AudioSource audioSource;

    private Track currentTrack;

    private int currentSegment = 0;
    private int currentBeat = 0;
    private int lastBeat = -1;
    private int lastSegment = -1;
    private bool hasSegmentLoudness = false;

    // Start is called before the first frame update
    void Start()
    {
		audioSource = GetComponent<AudioSource> ();
        
        // TODO: Make this interchangable and resettable, you know when you change songs n shit
        string path = "Assets/Songs/shake.json";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 

        string fileContents = reader.ReadToEnd();
        reader.Close();

        currentTrack = JsonUtility.FromJson<Track>(fileContents);

    }

    // Update is called once per frame
    void SegmentUpdate()
    {
        // TODO: Compare loudness_max with loudness_start in order to find an appropriate attack ratio for interesting sound event
        // Make another event (not onbeat) for this kind of info since it's not a beat per se
        if (currentTrack.segments[currentSegment + 1].start <= audioSource.time) {
            currentSegment++;
            hasSegmentLoudness = false;
        }
        var s = currentTrack.segments[currentSegment];
        if (lastSegment != currentSegment) {
            Debug.Log(string.Format("confidence: {0}, loudness_start: {1}, loudness_max_time: {2}, loudness_max: {3}", s.confidence, s.loudness_start, s.loudness_max_time, s.loudness_max));
        }
        if (!hasSegmentLoudness && s.loudness_max > -6 && (s.start + s.loudness_max_time) <= audioSource.time) {
            _onBeat();
            hasSegmentLoudness = true;
        }

        lastSegment = currentSegment;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: transform raw beats/tatums/bars into a nice function "onTick" with nice pre-made info like a counter and what has changed (tatum, bar or beat or all)
        if (currentTrack.beats[currentBeat + 1].start <= audioSource.time) {
            currentBeat++;
        }
        var b = currentTrack.beats[currentBeat];
        if (lastBeat != currentBeat) {
            _onBeat();
            Debug.Log(string.Format("confidence: {0}", b.confidence));
        }

        lastBeat = currentBeat;
    }
}
