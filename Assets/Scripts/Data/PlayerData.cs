using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public int correctAnswers;
    public int totalQuestions;
    public float averageTime;

    public PlayerData(string name)
    {
        playerName = name;
        score = 0;
        correctAnswers = 0;
        totalQuestions = 0;
        averageTime = 0f;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void AddCorrectAnswer()
    {
        correctAnswers++;
    }

    public void IncrementTotalQuestions()
    {
        totalQuestions++;
    }

    public float GetAccuracy()
    {
        if (totalQuestions == 0) return 0f;
        return ((float)correctAnswers / totalQuestions) * 100f;
    }
}

