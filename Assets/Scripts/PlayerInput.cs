using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerInput : MonoBehaviour
{
    public SongControl songControl;
    public AudioClip tapSound;

    private int score = 0;
    private List<bool> listenStateRhythm;
    private List<bool> playerTaps = new List<bool>();
    private bool isTapPhase = false;
    public AudioSource tapSoundSource;
    public TMP_Text scoreText; // Reference to the UI Text component

    private int totalScore = 0; // This will hold the total score
    private double correctTapBuffer = 0.125;
    private Queue<AudioSource> audioSourcePool;

    void Start()
    {
        InitializeAudioSourcePool(16); // Initialize pool with 16 AudioSource components
        UpdateScoreText();
        SetDifficultyBuffer(GameSettings.Difficulty);
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
            PlayTapSound(tapSound);
            double tapTime = AudioSettings.dspTime;
            //Debug.Log("Tap detected at time: " + tapTime);

            List<double> expectedTimings = songControl.GetExpectedTapTimings();
            bool correctTap = expectedTimings.Any(timing => System.Math.Abs(timing - tapTime) <= correctTapBuffer);

            if (correctTap)
            {
                score++;
                //Debug.Log($"Correct tap! Score: {score}");
                expectedTimings.Remove(expectedTimings.First(timing => System.Math.Abs(timing - tapTime) <= correctTapBuffer));
                //PlayTapSound(tapSound); // Play the tap sound using the pooled AudioSource system
                totalScore++; // Increment the total score
            }
            else
            {
                //Debug.Log("Incorrect tap.");

                totalScore--; // Decrement the total score
            }
            UpdateScoreText();
            playerTaps.Add(correctTap);

            ScoreKeeper.CurrentScore = totalScore; // Update the static score variable
            songControl.ChangeBackgroundColorOnPlayerTap();
        }


        if (!isTapPhase && playerTaps.Count > 0)
        {
            CheckScore();
            playerTaps.Clear();
        }

    }

    private void CheckScore()
    {
        //Debug.Log("Final Score for this cycle: " + score);
        score = 0; // Reset the score for the next cycle
    }

    private void PlayTapSound(AudioClip clip)
    {
        AudioSource sourceToUse = GetPooledAudioSource();

        sourceToUse.PlayOneShot(clip);

    }

    private AudioSource GetPooledAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            StartCoroutine(RequeueAudioSource(source));
            return source;
        }
        else
        {
            //Debug.LogWarning("No audio source available to play tap sound");
            return null;
        }
    }

    private void InitializeAudioSourcePool(int poolSize)
    {
        audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSourcePool.Enqueue(newSource);
            //Debug.Log("Added AudioSource to pool: " + newSource.GetInstanceID());
        }
        //Debug.Log("Total AudioSources in pool: " + audioSourcePool.Count);
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{totalScore}";
        }
    }

    public void SetDifficultyBuffer(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                correctTapBuffer = 0.2;
                //Debug.Log("Difficulty set to Easy");
                break;
            case "Normal":
                correctTapBuffer = 0.15;
                //Debug.Log("Difficulty set to Normal");
                break;
            case "Expert":
                correctTapBuffer = 0.1;
                //Debug.Log("Difficulty set to Expert");
                break;
        }
        songControl.SetDifficulty(difficulty);
    }
    private IEnumerator RequeueAudioSource(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        audioSourcePool.Enqueue(source);
    }
}
