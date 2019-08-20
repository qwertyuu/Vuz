using System;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    float bobbingAngle = 10f;
    float sideBobbingAngle = 5f;
    int attackAmount = 0;
    float t = 0;
    float speed = 0.03f;

    float firstBeatAt;
    float B;
    private Vector3 originalPosition;

    void Awake()
    {
        Conductor.OnTick += a;
        Conductor.OnLoadSong += b;
        Conductor.OnUpdateSongTime += UpdateSongTime;
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    private void UpdateSongTime(float currentSoundTime)
    {
        t = Mathf.Max(t - speed, 0);

        transform.localRotation = Quaternion.Euler(bobbingAngle * t, Mathf.PingPong(attackAmount, 50), -(attackAmount % 2) * 2 * sideBobbingAngle + sideBobbingAngle);

        if (currentSoundTime >= firstBeatAt) {
            var adjustedTime = currentSoundTime - firstBeatAt;
            transform.position = new Vector3(originalPosition.x + Mathf.Sin(B * adjustedTime) / 4, originalPosition.y, originalPosition.z);
        }
    }

    private void b(Track currentTrack)
    {
        // Basé sur https://www.mathsisfun.com/algebra/amplitude-period-frequency-phase-shift.html
        var firstBeat = currentTrack.beats[0];
        firstBeatAt = firstBeat.start;
        var period = firstBeat.duration * 4;
        B = (2 * Mathf.PI) / period;
    }

    void a(bool bar, bool beat)
    {
        if (beat) {
            t = 1;
            attackAmount++;
        }
    }
}
