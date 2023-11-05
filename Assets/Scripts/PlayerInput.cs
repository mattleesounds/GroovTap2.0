using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public SongControl songControl; // Reference to the SongControl script
    public AudioClip tapSound; // Assign this in the inspector

    private int score = 0;
    private List<bool> listenStateRhythm; // Stores the rhythm from the listen state
    private List<bool> playerTaps = new List<bool>(); // Instantiate the player taps list
    private bool isTapPhase = false;

    void Update()
    {
        if (songControl == null || !songControl.IsInitialized())
        {
            Debug.Log("SongControl is not initialized yet.");
            return; // Exit early if songControl is not ready
        }
        Debug.Log("songControl: " + (songControl != null)); // Check if songControl is assigned
        Debug.Log("playerTaps: " + (playerTaps != null)); // Check if playerTaps is initialized

        bool isTapPhase = !songControl.IsListenPhase();
        Debug.Log("Is Tap Phase: " + isTapPhase); // Log to check if the tap phase is being detected

        if (isTapPhase && Input.GetMouseButtonDown(0)) // 0 is the left mouse button or a touch on mobile
        {
            Debug.Log("Tap detected"); // Log to confirm that the tap/click is being detected
            AudioSource.PlayClipAtPoint(tapSound, Camera.main.transform.position);
            playerTaps.Add(true); // Add a tap to the player's rhythm

            // Ensure the listen state rhythm is updated at the start of the tap phase
            if (playerTaps.Count == 1)
            {
                listenStateRhythm = songControl.GetListenStateRhythm();
            }

            if (listenStateRhythm != null && playerTaps != null && playerTaps.Count <= listenStateRhythm.Count && playerTaps[playerTaps.Count - 1] == listenStateRhythm[playerTaps.Count - 1])
            {
                score++;
                Debug.Log("Correct tap! Score: " + score);
            }
            else
            {
                Debug.Log("Incorrect tap.");
            }
        }

        // Reset for the next cycle
        if (songControl.IsListenPhase() && playerTaps.Count > 0)
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
}
