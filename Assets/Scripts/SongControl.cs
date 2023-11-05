using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongControl : MonoBehaviour
{
    public AudioClip quarterNoteSound; // Assign this in the inspector with your sound
    public AudioSource musicSource; // Assign the music AudioSource in the inspector

    public float beatDuration = 1.0f; // Duration of a beat in seconds
    private float subdivision; // Duration of a subdivision in seconds
    public AudioSource audioSource;

    private int beatCounter = 0; // Tracks the current beat within a cycle
    private int cycleCounter = 0; // Tracks the number of completed cycles
    private int subdivisionCounter = 0; // Tracks the subdivisions within a beat

    private HashSet<int> possibleSubdivisions = new HashSet<int>(); // Stores possible subdivisions for rhythm

    private List<bool> listenStateRhythm = new List<bool>(); // Stores the rhythm for the listen state
    private enum RhythmType { Quarter, Eighth, Sixteenth }
    private RhythmType rhythmType = RhythmType.Quarter;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null || audioSource == musicSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        subdivision = beatDuration / 4;
        StartCoroutine(RhythmAndMusicCycle());
    }

    IEnumerator RhythmAndMusicCycle()
    {
        yield return new WaitForSeconds(beatDuration * 2);

        // Generate the first rhythm
        HandleRhythmGeneration();
        IncrementCounters();

        // Now start the music
        //musicSource.Play();
        //Debug.Log("Music started at: " + Time.time);

        float nextSubdivisionTime = musicSource.time + subdivision;

        while (true)
        {
            float currentTime = musicSource.time;
            if (currentTime >= nextSubdivisionTime)
            {
                HandleRhythmGeneration();
                IncrementCounters();
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
                listenStateRhythm.Add(playSound); // Update the list here
                if (playSound)
                {
                    audioSource.PlayOneShot(quarterNoteSound);
                }
            }
            else
            {
                listenStateRhythm.Add(false); // Add false if no sound is played
            }
        }
    }

    private void UpdateRhythmType()
    {
        if (beatCounter == 0 && subdivisionCounter == 0)
        {
            Debug.Log("Starting cycle number: " + (cycleCounter + 1));
            possibleSubdivisions.Clear(); // Clear previous rhythm type possible subdivisions

            if (cycleCounter < 4)
            {
                rhythmType = RhythmType.Quarter;
                possibleSubdivisions.UnionWith(new int[] { 1, 5, 9, 13 });
            }
            else if (cycleCounter < 8)
            {
                rhythmType = RhythmType.Eighth;
                possibleSubdivisions.UnionWith(new int[] { 1, 3, 5, 7, 9, 11, 13, 15 });
            }
            else
            {
                rhythmType = RhythmType.Sixteenth;
                for (int i = 1; i <= 16; i++)
                {
                    possibleSubdivisions.Add(i);
                }
            }
        }
    }

    // Public method to get the rhythm of the listen state
    public List<bool> GetListenStateRhythm()
    {
        return listenStateRhythm;
    }

    // Public method to determine if it's the listen phase
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
            Debug.Log("Beat Counter: " + beatCounter);
            if (beatCounter == 8)
            {
                cycleCounter++;
                beatCounter = 0;
                Debug.Log("Starting cycle number: " + (cycleCounter + 1));
            }
        }
    }
}
