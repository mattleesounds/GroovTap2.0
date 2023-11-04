using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmGenerator : MonoBehaviour
{
    public AudioClip quarterNoteSound; // Assign this in the inspector with your sound

    public float beatDuration = 1.0f; // Duration of a beat in seconds
    private AudioSource audioSource;

    private bool isListenPhase = false; // Indicates if it's the phase where the game generates rhythm
    private int beatCounter = -2; // Starts at -2 to account for the initial 2 beat delay
    private bool[] rhythmPattern = new bool[4]; // Stores the generated rhythm for the current cycle

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If there isn't one already, add an AudioSource component dynamically
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start the rhythm cycle
        StartCoroutine(RhythmCycle());
    }

    IEnumerator RhythmCycle()
    {
        while (true)
        {
            // Wait for a beat
            yield return new WaitForSeconds(beatDuration);

            // Increment the beat counter
            beatCounter = (beatCounter + 1) % 8;

            // Skip the first two beats (initial delay)
            if (beatCounter < 0) continue;

            // The first half of the cycle is the listen phase (beats 0-3)
            // The second half of the cycle is the tap phase (beats 4-7)
            isListenPhase = (beatCounter < 4);

            if (isListenPhase)
            {
                // Randomly decide whether to play a sound on this beat
                // Ensure at least one sound plays during the listen phase
                bool playSound = (rhythmPattern[0] || rhythmPattern[1] || rhythmPattern[2]) ? Random.value > 0.5f : true;
                rhythmPattern[beatCounter] = playSound;

                if (playSound)
                {
                    audioSource.PlayOneShot(quarterNoteSound);
                }
            }
        }
    }
}
