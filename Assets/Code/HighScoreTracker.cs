using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreEntry
{
    public string name;
    public float score;
}

public class HighScoreTracker : MonoBehaviour
{
    float last_high_score = 0f;

    void Start()
    {
        last_high_score = PlayerPrefs.GetFloat("high_score", 0f);
    }

    public void UpdateHighScore(float score)
    {
        if (score > last_high_score)
        {
            last_high_score = score;
            PlayerPrefs.SetFloat("high_score", score);
            PlayerPrefs.Save();
        }
    }

    public float GetHighScore()
    {
        return last_high_score;
    }
}
