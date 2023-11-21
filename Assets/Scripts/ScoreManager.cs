using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreDisplayText;

    void Start()
    {
        int score = ScoreKeeper.CurrentScore;
        scoreDisplayText.text = $"{score}";
    }
}

public static class ScoreKeeper
{
    public static int CurrentScore { get; set; }
}