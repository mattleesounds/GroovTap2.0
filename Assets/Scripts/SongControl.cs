using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Queue<AudioSource> audioSourcePool;
    public int poolSize = 16; // Adjust pool size as needed
    private bool tapGeneratedInCurrentPhase = false;
    private string Difficulty = "Easy";
    private Camera mainCamera;
    private Color defaultBackgroundColor;

    void Start()
    {
        InitializeAudioSourcePool();
        audioSource = GetComponent<AudioSource>();
        subdivision = beatDuration / 4;
        RingMover ringMover = FindObjectOfType<RingMover>();
        musicSource.PlayScheduled(ringMover.musicStartTime);
        double scheduledStartTime = ringMover.musicStartTime;

        StartCoroutine(RhythmAndMusicCycle(scheduledStartTime));
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            defaultBackgroundColor = mainCamera.backgroundColor;
        }

    }

    IEnumerator RhythmAndMusicCycle(double scheduledStartTime)
    {
        while (AudioSettings.dspTime < scheduledStartTime)
        {
            yield return null;
        }
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

                if (playSound)
                {
                    tapGeneratedInCurrentPhase = true; // Tap has been generated in this phase
                }

                // Determine the last possible subdivision for the current rhythm type
                int lastSubdivision = GetLastPossibleSubdivision();

                // Force a tap on the last possible subdivision if none have been generated
                if (totalSubdivision == lastSubdivision && !tapGeneratedInCurrentPhase)
                {
                    playSound = true;
                    tapGeneratedInCurrentPhase = true;
                }

                listenStateRhythm.Add(playSound);
                if (playSound)
                {
                    // Calculate the dspTime for when the sound should play
                    double nextSubdivisionDspTime = AudioSettings.dspTime + subdivision;

                    // Play the sound at the calculated dspTime
                    PlaySound(quarterNoteSound, nextSubdivisionDspTime);

                    // Add the tap timing for scoring purposes
                    expectedTapTimings.Add(nextSubdivisionDspTime + 4 * beatDuration);
                    //Debug.Log("Rhythm generated - scheduled sound at dspTime: " + nextSubdivisionDspTime);
                }
            }
            else
            {
                // Reset the flag when transitioning out of the listen phase
                tapGeneratedInCurrentPhase = false;
            }
        }
    }

    private void UpdateRhythmType()
    {
        if (beatCounter == 0 && subdivisionCounter == 0)
        {
            Debug.Log("Starting cycle number: " + (cycleCounter + 1));
            possibleSubdivisions.Clear();
            switch (Difficulty)
            {
                case "Easy":
                    {
                        switch (cycleCounter)
                        {
                            case < 2:
                                rhythmType = RhythmType.Quarter;
                                possibleSubdivisions.UnionWith(new int[] { 5, 9, 13 });
                                break;
                            case < 9:
                                rhythmType = RhythmType.Quarter;
                                possibleSubdivisions.UnionWith(new int[] { 1, 5, 9, 13 });
                                break;
                            default:
                                rhythmType = RhythmType.Eighth;
                                possibleSubdivisions.UnionWith(new int[] { 1, 3, 5, 7, 9, 11, 13, 15 });
                                break;
                        }
                        break;
                    }
                case "Normal":
                    {
                        switch (cycleCounter)
                        {
                            case < 2:
                                rhythmType = RhythmType.Quarter;
                                possibleSubdivisions.UnionWith(new int[] { 5, 9, 13 });
                                break;
                            case < 5:
                                rhythmType = RhythmType.Quarter;
                                possibleSubdivisions.UnionWith(new int[] { 1, 5, 9, 13 });
                                break;
                            case < 13:
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
                        break;
                    }

                case "Expert":
                    switch (cycleCounter)
                    {
                        case < 2:
                            rhythmType = RhythmType.Quarter;
                            possibleSubdivisions.UnionWith(new int[] { 5, 9, 13 });
                            break;
                        case < 3:
                            rhythmType = RhythmType.Quarter;
                            possibleSubdivisions.UnionWith(new int[] { 1, 5, 9, 13 });
                            break;
                        case < 9:
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
                //Debug.Log("Starting cycle number: " + (cycleCounter + 1));
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
        audioSourcePool = new Queue<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            StartCoroutine(RequeueAudioSource(source));
            return source;
        }

        //Debug.LogWarning("All audio sources are busy. Consider increasing pool size.");
        return null;
    }
    private void PlaySound(AudioClip clip, double dspTime)
    {
        AudioSource sourceToUse = GetPooledAudioSource();
        if (sourceToUse != null)
        {
            sourceToUse.clip = clip;
            sourceToUse.PlayScheduled(dspTime);
            StartCoroutine(TemporaryChangeBackgroundColor());
        }
    }

    private int GetLastPossibleSubdivision()
    {
        switch (rhythmType)
        {
            case RhythmType.Eighth:
                return 15;
            case RhythmType.Sixteenth:
                return 16;
            default: // Assuming Sixteenth is the default
                return 13;
        }
    }

    private IEnumerator RequeueAudioSource(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        audioSourcePool.Enqueue(source);
    }

    public void SetDifficulty(string difficulty)
    {
        Difficulty = difficulty;
    }

    private IEnumerator TemporaryChangeBackgroundColor()
    {
        if (mainCamera != null)
        {
            // Initial delay of 200 milliseconds before color change
            yield return new WaitForSeconds(0.3f);

            // Change to the specific color (RGB values divided by 255)
            mainCamera.backgroundColor = new Color(193f / 255f, 40f / 255f, 255f / 255f);

            // Wait for another 200 milliseconds
            yield return new WaitForSeconds(0.1f);

            // Revert to the default color
            mainCamera.backgroundColor = defaultBackgroundColor;
        }
    }

    public void ChangeBackgroundColorOnPlayerTap()
    {
        StartCoroutine(TemporaryChangeBackgroundColorOnTap());
    }

    private IEnumerator TemporaryChangeBackgroundColorOnTap()
    {
        if (mainCamera != null)
        {
            // Change to the specific color for player tap
            mainCamera.backgroundColor = new Color(47f / 255f, 246f / 255f, 186f / 255f);

            // Wait for 200 milliseconds
            yield return new WaitForSeconds(0.1f);

            // Revert to the default color
            mainCamera.backgroundColor = defaultBackgroundColor;
        }
    }

}
