using System.Collections;
using System.Collections.Generic;
using System.Linq; // Add this to use Linq functionalities such as Contains
using UnityEngine;

public class SongControl : MonoBehaviour
{
    public AudioClip quarterNoteSound; // Assign this in the inspector with your sound
    public AudioSource musicSource; // Assign the music AudioSource in the inspector

    public float beatDuration = 1.0f; // Duration of a beat in seconds
    private AudioSource audioSource;

    private bool isListenPhase = false; // Indicates if it's the phase where the game generates rhythm
    private int beatCounter = 0; // Starts at -2 to account for the initial 2 beat delay
    private bool[] rhythmPattern = new bool[4]; // Stores the generated rhythm for the current cycle

    void Start()
    {
        // Get the AudioSource component attached to this GameObject for quarter note sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null || audioSource == musicSource)
        {
            // If there isn't one already, or if it's the same as musicSource, add an AudioSource component dynamically
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start the rhythm cycle
        StartCoroutine(RhythmAndMusicCycle());
    }

    IEnumerator RhythmAndMusicCycle()
    {
        yield return new WaitForSeconds(beatDuration * 2); // Initial 2-beat delay
        musicSource.Play(); // Start playing the music
        Debug.Log("Music started at: " + Time.time);

        float nextBeatTime = musicSource.time + beatDuration; // Time for the next beat

        while (true)
        {
            float currentTime = musicSource.time; // Current playback time
            if (currentTime >= nextBeatTime)
            {
                beatCounter = (beatCounter + 1) % 8;
                Debug.Log("Current beatCounter: " + beatCounter + " at time: " + Time.time);

                // Listen phase is only during the first half of the cycle (beats 0-3)
                isListenPhase = beatCounter < 4;

                if (isListenPhase && beatCounter >= 0)
                {
                    // Generate rhythm only during the listen phase
                    bool playSound = Random.value > 0.5f;
                    rhythmPattern[beatCounter] = playSound;

                    // Ensure at least one sound plays during the listen phase
                    if (beatCounter == 3 && !rhythmPattern.Contains(true))
                    {
                        playSound = true;
                        rhythmPattern[3] = true;
                    }

                    if (playSound)
                    {
                        audioSource.PlayOneShot(quarterNoteSound);
                    }
                }

                nextBeatTime += beatDuration;
            }

            yield return null; // Wait until the next frame
        }
    }






}
