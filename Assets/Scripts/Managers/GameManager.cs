using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración del Juego")]
    [SerializeField] private int pointsPerCorrectAnswer = 100;
    [SerializeField] private int timeBonus = 10; // Puntos extra por segundo restante
    [SerializeField] private bool enableTimeBonus = true;

    private PlayerData currentPlayer;
    private int currentScore = 0;
    private float questionStartTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Asegurar que sea objeto raíz
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            
        }
    }

    public void StartNewGame(string playerName)
    {
        currentPlayer = new PlayerData(playerName);
        currentScore = 0;
        
        // IMPORTANTE: Recargar preguntas según el Kahoot seleccionado
        if (QuestionManager.Instance != null)
        {
            QuestionManager.Instance.ReloadQuestionsForGame();
            QuestionManager.Instance.ResetQuestions();
        }

        // Iniciar tracking de reportes
        if (ReportsManager.Instance != null)
        {
            ReportsManager.Instance.StartNewGameTracking();
        }
        
        
    }

    public void StartQuestion()
    {
        questionStartTime = Time.time;
    }

    public void SubmitAnswer(int answerIndex)
    {
        Question currentQuestion = QuestionManager.Instance.GetCurrentQuestion();
        
        if (currentQuestion == null) return;

        float timeSpent = Time.time - questionStartTime;
        currentPlayer.IncrementTotalQuestions();

        bool isCorrect = currentQuestion.IsCorrectAnswer(answerIndex);
        int bonusPoints = 0;

        if (isCorrect)
        {
            currentPlayer.AddCorrectAnswer();
            
            int points = pointsPerCorrectAnswer;
            
            // Calcular bonus de tiempo
            if (enableTimeBonus)
            {
                float timeRemaining = currentQuestion.timeLimit - timeSpent;
                if (timeRemaining > 0)
                {
                    bonusPoints = Mathf.RoundToInt(timeRemaining * timeBonus);
                    points += bonusPoints;
                }
            }
            
            currentPlayer.AddScore(points);
            currentScore = currentPlayer.score;
            
            // Track para reportes
            if (ReportsManager.Instance != null)
            {
                ReportsManager.Instance.TrackAnswer(true, bonusPoints);
            }
            
            
        }
        else
        {
            // Track para reportes
            if (ReportsManager.Instance != null)
            {
                ReportsManager.Instance.TrackAnswer(false, 0);
            }
            
            
        }

        // Track tiempo y categoría para reportes
        if (ReportsManager.Instance != null)
        {
            ReportsManager.Instance.TrackQuestionTime(timeSpent);
            ReportsManager.Instance.TrackCategory(currentQuestion.category);
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public PlayerData GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public float GetAccuracy()
    {
        return currentPlayer != null ? currentPlayer.GetAccuracy() : 0f;
    }

    public void EndGame()
    {
        if (currentPlayer != null)
        {
            
            
            
            
        }
    }
}

