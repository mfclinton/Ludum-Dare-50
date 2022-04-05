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
    
    List<HighScoreEntry> scores = new List<HighScoreEntry>();

    void AddNewScore(string entryName, float entryScore)
    {
        scores.Add(new HighScoreEntry { name = entryName, score = entryScore });
    }

    
}
