using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongControl : MonoBehaviour
{
    public float beatDuration = 1.0f; // Duration of a beat in seconds

    void Start()
    {
        // Start playing the music with a delay equal to twice the beat duration
        StartCoroutine(StartMusicWithDelay(beatDuration * 2));
    }

    IEnumerator StartMusicWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<AudioSource>().Play();
    }
}