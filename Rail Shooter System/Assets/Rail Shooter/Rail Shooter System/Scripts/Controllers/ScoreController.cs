using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    #region Attributes

    private int score;

    [Header("Text to show score :")]
    public Text scoreText;

    #endregion

    #region Unity Methods

    void Start()
    {
        UpdateScoreText();
    }

    #endregion

    #region Score Methods

    private void UpdateScoreText()
    {
        scoreText.text = "SCORE: " + score;
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    #endregion
}