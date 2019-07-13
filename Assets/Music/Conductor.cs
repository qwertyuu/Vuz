using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Conductor : SceneSingleton<Conductor>
{
    event System.Action<float, float> _onAttack = delegate { };
    public static event System.Action<float, float> OnAttack
    {
        add { Instance._onAttack += value; }
        remove { Instance._onAttack -= value; }
    }

    event System.Action<bool, bool> _onTick = delegate { };
    public static event System.Action<bool, bool> OnTick
    {
        add { Instance._onTick += value; }
        remove { Instance._onTick -= value; }
    }

    event System.Action<Segment> _onSegment = delegate { };
    public static event System.Action<Segment> OnSegment
    {
        add { Instance._onSegment += value; }
        remove { Instance._onSegment -= value; }
    }

	public static AudioSource audioSource;

    private Track currentTrack;

    private int currentSegment = 0;
    private int currentBeat = 0;
    private int currentBar = 0;
    private int currentTatum = 0;
    private bool hasSegmentLoudness = false;

    // Start is called before the first frame update
    void Start()
    {
		audioSource = GetComponent<AudioSource> ();
        
        // TODO: Make this interchangable and resettable, you know when you change songs n shit
        string path = "Assets/Songs/Analysis/ClimbingTheCorporateLadder.json";

        StreamReader reader = new StreamReader(path); 

        string fileContents = reader.ReadToEnd();
        reader.Close();

        currentTrack = JsonUtility.FromJson<Track>(fileContents);

    }

    // Update is called once per frame
    void Update()
    {
        var currentAudioTime = audioSource.time;

        // Make another event (not onbeat) for this kind of info since it's not a beat per se
        var s = currentTrack.segments[currentSegment];
        if (currentTrack.segments[currentSegment + 1].start <= currentAudioTime) {
            currentSegment++;
            s = currentTrack.segments[currentSegment];
            _onSegment(s);
            hasSegmentLoudness = false;
        }
        
        if (!hasSegmentLoudness && (s.start + s.loudness_max_time) <= currentAudioTime) {
            _onAttack(s.loudness_start, s.loudness_max);
            hasSegmentLoudness = true;
        }
        
        var barChanged = false;
        var bar = currentTrack.bars[currentBar];
        if (isLessOrNearOf(currentTrack.bars[currentBar + 1].start, currentAudioTime)) {
            currentBar++;
            barChanged = true;
            bar = currentTrack.bars[currentBar];
        }

        var beatChanged = false;
        var beat = currentTrack.beats[currentBeat];
        if (isLessOrNearOf(currentTrack.beats[currentBeat + 1].start, currentAudioTime)) {
            currentBeat++;
            beatChanged = true;
            beat = currentTrack.beats[currentBeat];
        }

        var tatum = currentTrack.tatums[currentTatum];
        if (isLessOrNearOf(currentTrack.tatums[currentTatum + 1].start, currentAudioTime)) {
            currentTatum++;
            tatum = currentTrack.tatums[currentTatum];
            //Debug.Log(string.Format("bar changed: {0}, beat changed: {1}, audiosource time: {2}", barChanged, beatChanged, audioSource.time));
            //Debug.Log(string.Format("bar start: {0}, beat start: {1}, tatum start: {2}, audiosource time: {3}", bar.start, beat.start, tatum.start, audioSource.time));
            _onTick(barChanged, beatChanged);
        }

        //Debug.Log(string.Format("next tatum start: {0}, audiosource time: {1}", currentTrack.tatums[currentTatum + 1].start, audioSource.time));
    }

    /*
        Makes it easier to make beat match in case we get a big gap between frames
    */
    bool isLessOrNearOf(float first, float second, float tolerence = 0.015f)
    {
        if (first <= second) {
            return true;
        }
        return false;
        return first - second <= tolerence;
    }
}
