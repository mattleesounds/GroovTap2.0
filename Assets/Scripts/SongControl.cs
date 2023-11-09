using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongControl : MonoBehaviour
{
    public AudioClip quarterNoteSound;
    public AudioSource musicSource;
    public AudioSource audioSource;
    public float beatDuration = 1.0f;
    private float subdivision;
    private int beatCounter = 0;
    private int cycleCounter = 0;
    private int subdivisionCounter = 0;
    private HashSet<int> possibleSubdivisions = new HashSet<int>();
    private List<bool> listenStateRhythm = new List<bool>();
    private enum RhythmType { Quarter, Eighth, Sixteenth }
    private RhythmType rhythmType = RhythmType.Quarter;
    public List<double> expectedTapTimings = new List<double>(); // Use double for dspTime
    // Pool of AudioSources to allow for rapid sound playback.
    private List<AudioSource> audioSourcePool;
    public int poolSize = 16; // Adjust pool size as needed

    void Start()
    {
        InitializeAudioSourcePool();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null || audioSource == musicSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        subdivision = beatDuration / 4;
        StartCoroutine(RhythmAndMusicCycle());
        StartCoroutine(StartMusicWithDelay(beatDuration * 2.0f));
    }

    IEnumerator RhythmAndMusicCycle()
    {
        // Wait for two beats before starting
        yield return new WaitForSeconds((beatDuration * 2.0f) + 0.05f);

        // Start the rhythm generation
        HandleRhythmGeneration();
        IncrementCounters();



        // Calculate the next subdivision time
        double nextSubdivisionTime = AudioSettings.dspTime + subdivision;

        while (true)
        {
            double currentTime = AudioSettings.dspTime;
            if (currentTime >= nextSubdivisionTime)
            {

                IncrementCounters();
                HandleRhythmGeneration();
                nextSubdivisionTime += subdivision;
            }
            yield return null;
        }
    }

    private void HandleRhythmGeneration()
    {
        UpdateRhythmType();
        bool isListenPhase = beatCounter < 4;

        if (isListenPhase)
        {
            int totalSubdivision = beatCounter * 4 + subdivisionCounter + 1;
            if (possibleSubdivisions.Contains(totalSubdivision))
            {
                bool playSound = Random.value > 0.5f;
                listenStateRhythm.Add(playSound);
                if (playSound)
                {
                    // Calculate the dspTime for when the sound should play
                    double nextSubdivisionDspTime = AudioSettings.dspTime + subdivision;

                    // Play the sound at the calculated dspTime
                    PlaySound(quarterNoteSound, nextSubdivisionDspTime);

                    // Add the tap timing for scoring purposes
                    expectedTapTimings.Add(nextSubdivisionDspTime + 4 * beatDuration);
                    Debug.Log("Rhythm generated - scheduled sound at dspTime: " + nextSubdivisionDspTime);
                }
            }
        }
    }

    private void UpdateRhythmType()
    {
        if (beatCounter == 0 && subdivisionCounter == 0)
        {
            Debug.Log("Starting cycle number: " + (cycleCounter + 1));
            possibleSubdivisions.Clear();
            switch (cycleCounter)
            {
                case < 2:
                    rhythmType = RhythmType.Quarter;
                    possibleSubdivisions.UnionWith(new int[] { 5, 9, 13 });
                    break;
                case < 4:
                    rhythmType = RhythmType.Quarter;
                    possibleSubdivisions.UnionWith(new int[] { 1, 5, 9, 13 });
                    break;
                case < 8:
                    rhythmType = RhythmType.Eighth;
                    possibleSubdivisions.UnionWith(new int[] { 1, 3, 5, 7, 9, 11, 13, 15 });
                    break;
                default:
                    rhythmType = RhythmType.Sixteenth;
                    for (int i = 1; i <= 16; i++)
                    {
                        possibleSubdivisions.Add(i);
                    }
                    break;
            }
        }
    }

    public List<bool> GetListenStateRhythm()
    {
        return listenStateRhythm;
    }

    public bool IsListenPhase()
    {
        return beatCounter < 4;
    }

    public bool IsInitialized()
    {
        return listenStateRhythm.Count > 0;
    }

    private void IncrementCounters()
    {
        subdivisionCounter++;
        if (subdivisionCounter == 4)
        {
            subdivisionCounter = 0;
            beatCounter++;
            if (beatCounter == 8)
            {
                cycleCounter++;
                beatCounter = 0;
                Debug.Log("Starting cycle number: " + (cycleCounter + 1));
                listenStateRhythm.Clear();
                expectedTapTimings.Clear();
            }
        }
    }

    public List<double> GetExpectedTapTimings()
    {
        return expectedTapTimings;
    }

    IEnumerator StartMusicWithDelay(float delay)
    {
        // Wait for the specified delay before starting the music
        yield return new WaitForSeconds(delay);
        musicSource.Play();
    }

    private void InitializeAudioSourcePool()
    {
        audioSourcePool = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSourcePool.Add(source);
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // Optional: if all sources are playing and you want to force playing the new sound, return the first one.
        // Be aware this will stop the sound currently playing on that source.
        // return audioSourcePool[0];

        // If all AudioSources are in use, log a warning or consider expanding your pool.
        Debug.LogWarning("All audio sources are busy. Consider increasing pool size.");
        return null;
    }

    private void PlaySound(AudioClip clip, double dspTime)
    {
        AudioSource sourceToUse = GetPooledAudioSource();
        if (sourceToUse != null)
        {
            sourceToUse.clip = clip;
            sourceToUse.PlayScheduled(dspTime);
        }
    }
}
