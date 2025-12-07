using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Question
{
    public string questionText;
    public List<string> answers;
    public int correctAnswerIndex;
    public string category;
    public int timeLimit; // Tiempo en segundos

    public Question(string question, List<string> answerList, int correctIndex, string cat = "General", int time = 20)
    {
        questionText = question;
        answers = answerList;
        correctAnswerIndex = correctIndex;
        category = cat;
        timeLimit = time;
    }

    public bool IsCorrectAnswer(int answerIndex)
    {
        return answerIndex == correctAnswerIndex;
    }

    public string GetCorrectAnswer()
    {
        if (correctAnswerIndex >= 0 && correctAnswerIndex < answers.Count)
        {
            return answers[correctAnswerIndex];
        }
        return "";
    }
}

[Serializable]
public class QuestionSet
{
    public string setName;
    public string theme; // "Inazuma Eleven"
    public List<Question> questions;

    public QuestionSet()
    {
        questions = new List<Question>();
    }
}

