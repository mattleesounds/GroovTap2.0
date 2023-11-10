using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    public SongControl songControl;
    public AudioClip tapSound;

    private int score = 0;
    private List<bool> listenStateRhythm;
    private List<bool> playerTaps = new List<bool>();
    private bool isTapPhase = false;
    public AudioSource[] audioSourcePool; // Populate this with references to pre-existing AudioSource components
    public TMP_Text scoreText; // Reference to the UI Text component

    private int totalScore = 0; // This will hold the total score


    void Start()
    {
        InitializeAudioSourcePool(16); // Initialize pool with 16 AudioSource components
        UpdateScoreText();
    }
    void Update()
    {
        if (songControl == null || !songControl.IsInitialized())
        {
            return; // Exit early if songControl is not ready
        }

        isTapPhase = !songControl.IsListenPhase();

        if (isTapPhase && Input.GetMouseButtonDown(0))
        {
            double tapTime = AudioSettings.dspTime;
            Debug.Log("Tap detected at time: " + tapTime);

            List<double> expectedTimings = songControl.GetExpectedTapTimings();
            bool correctTap = expectedTimings.Any(timing => System.Math.Abs(timing - tapTime) <= 0.1);

            if (correctTap)
            {
                score++;
                Debug.Log($"Correct tap! Score: {score}");
                expectedTimings.Remove(expectedTimings.First(timing => System.Math.Abs(timing - tapTime) <= 0.1));
                PlayTapSound(tapSound); // Play the tap sound using the pooled AudioSource system
                totalScore++; // Increment the total score
            }
            else
            {
                Debug.Log("Incorrect tap.");
                PlayTapSound(tapSound);
                totalScore--; // Decrement the total score
            }
            UpdateScoreText();
            playerTaps.Add(correctTap);
        }


        if (!isTapPhase && playerTaps.Count > 0)
        {
            CheckScore();
            playerTaps.Clear();
        }
    }

    private void CheckScore()
    {
        Debug.Log("Final Score for this cycle: " + score);
        score = 0; // Reset the score for the next cycle
    }

    private void PlayTapSound(AudioClip clip)
    {
        AudioSource sourceToUse = GetPooledAudioSource();
        if (sourceToUse != null)
        {
            sourceToUse.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("No audio source available to play tap sound");
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        Debug.LogWarning("No audio source available to play tap sound");
        return null; // Or handle this case as needed
    }

    private void InitializeAudioSourcePool(int poolSize)
    {
        audioSourcePool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            audioSourcePool[i] = gameObject.AddComponent<AudioSource>();
        }
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{totalScore}";
        }
    }
}
