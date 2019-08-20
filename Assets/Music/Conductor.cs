using System;
using System.IO;
using UnityEngine;
using System.Linq;

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

    event System.Action<float, float, float> _onUpdateRelativeTick = delegate { };
    public static event System.Action<float, float, float> OnUpdateRelativeTick
    {
        add { Instance._onUpdateRelativeTick += value; }
        remove { Instance._onUpdateRelativeTick -= value; }
    }

    event System.Action<Segment> _onSegment = delegate { };
    public static event System.Action<Segment> OnSegment
    {
        add { Instance._onSegment += value; }
        remove { Instance._onSegment -= value; }
    }

    event System.Action<float, float> _onUpdateSegmentAttack = delegate { };
    public static event System.Action<float, float> OnUpdateSegmentAttack
    {
        add { Instance._onUpdateSegmentAttack += value; }
        remove { Instance._onUpdateSegmentAttack -= value; }
    }

    event System.Action<Track> _onLoadSong = delegate { };
    public static event System.Action<Track> OnLoadSong
    {
        add { Instance._onLoadSong += value; }
        remove { Instance._onLoadSong -= value; }
    }

    event System.Action<float> _onUpdateSongTime = delegate { };
    public static event System.Action<float> OnUpdateSongTime
    {
        add { Instance._onUpdateSongTime += value; }
        remove { Instance._onUpdateSongTime -= value; }
    }

	public static AudioSource audioSource;

    private Track currentTrack;

    private int currentSegment = 0;
    private int currentSegmentPerLoudness = 0;
    private int currentBeat = 0;
    private int currentBar = 0;
    private int currentTatum = 0;
    private bool hasSegmentLoudness = false;

    private float loudnessTreshold = -6;
    private Segment[] loudnessFilteredSegments;

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

        PreProcessTrack(currentTrack);

        ComputeSegmentLoudnessArray();

        _onLoadSong(currentTrack);
    }

    private void ComputeSegmentLoudnessArray()
    {
        this.loudnessFilteredSegments = currentTrack.segments.Where(segment => segment.loudness_max >= loudnessTreshold).ToArray();
    }

    private void PreProcessTrack(Track currentTrack)
    {
        // Make loudness max time absolute
        for (int i = 0; i < currentTrack.segments.Length; i++)
        {
            currentTrack.segments[i].loudness_max_time = currentTrack.segments[i].start + currentTrack.segments[i].loudness_max_time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var currentAudioTime = audioSource.time;

        _onUpdateSongTime(currentAudioTime);

        UpdateSegmentAttack(currentAudioTime);

        UpdateRelativeSegmentAttack(currentAudioTime);

        UpdateTick(currentAudioTime);
    }

    private void UpdateSegmentAttack(float currentAudioTime)
    {
        var currentSegment = currentTrack.segments[this.currentSegment];
        var nextSegment = currentTrack.segments[this.currentSegment + 1];
        if (nextSegment.start <= currentAudioTime) {
            this.currentSegment++;
            currentSegment = currentTrack.segments[this.currentSegment];
            _onSegment(currentSegment);
            hasSegmentLoudness = false;
        }
        
        if (!hasSegmentLoudness && currentSegment.loudness_max_time <= currentAudioTime) {
            _onAttack(currentSegment.loudness_start, currentSegment.loudness_max);
            hasSegmentLoudness = true;
        }
    }

    private void UpdateRelativeSegmentAttack(float currentAudioTime)
    {
        var currentSegment = this.loudnessFilteredSegments[this.currentSegmentPerLoudness];

        float relativeTime = 0;
        float loudnessValue = 0;

        if (this.currentSegmentPerLoudness == 0 && currentAudioTime < currentSegment.loudness_max_time) {
            relativeTime = (currentAudioTime - currentSegment.start) / (currentSegment.loudness_max_time - currentSegment.start);
            loudnessValue = currentSegment.loudness_max;
        } else {
            var nextSegment = this.loudnessFilteredSegments[this.currentSegmentPerLoudness + 1];
            if (nextSegment.loudness_max_time <= currentAudioTime) {
                this.currentSegmentPerLoudness++;
                
                currentSegment = this.loudnessFilteredSegments[this.currentSegmentPerLoudness];
                nextSegment = this.loudnessFilteredSegments[this.currentSegmentPerLoudness + 1];
            }
            relativeTime = (currentAudioTime - currentSegment.loudness_max_time) / (nextSegment.loudness_max_time - currentSegment.loudness_max_time);
            loudnessValue = nextSegment.loudness_max;
        }

        _onUpdateSegmentAttack(relativeTime, loudnessValue);
    }

    private void UpdateTick(float currentAudioTime)
    {
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
            _onTick(barChanged, beatChanged);
        }

        var nextBar = currentTrack.bars[currentBar + 1];
        float relativeBarTime = (currentAudioTime - bar.start) / (nextBar.start - bar.start);

        var nextBeat = currentTrack.beats[currentBeat + 1];
        float relativeBeatTime = (currentAudioTime - beat.start) / (nextBeat.start - beat.start);

        var nextTick = currentTrack.tatums[currentTatum + 1];
        float relativeTickTime = (currentAudioTime - tatum.start) / (nextTick.start - tatum.start);

        _onUpdateRelativeTick(relativeBarTime, relativeBeatTime, relativeTickTime);
    }

    /*
        Makes it easier to make beat match in case we get a big gap between frames
    */
    bool isLessOrNearOf(float first, float second, float tolerence = 0.015f)
    {
        if (first <= second) {
            return true;
        }
        return first - second <= tolerence;
    }
}
