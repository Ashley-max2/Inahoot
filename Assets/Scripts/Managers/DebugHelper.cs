using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [Header("Debug Options")]
    [SerializeField] private bool enableDebugMode = false;
    [SerializeField] private bool showFPS = false;
    [SerializeField] private bool autoSkipQuestions = false;
    [SerializeField] private float autoSkipDelay = 2f;

    private float deltaTime = 0.0f;

    void Update()
    {
        if (!enableDebugMode) return;

        // Calcular FPS
        if (showFPS)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        // Auto-skip para testing
        if (autoSkipQuestions && Input.GetKeyDown(KeyCode.Space))
        {
            SkipToNextQuestion();
        }

        // Cheats de debug
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddDebugScore(100);
        }
    }

    void OnGUI()
    {
        if (!enableDebugMode || !showFPS) return;

        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }

    private void SkipToNextQuestion()
    {
        var quizUI = FindObjectOfType<QuizUIManager>();
        if (quizUI != null)
        {
            
            // Simular respuesta correcta
            quizUI.OnAnswerSelected(0);
        }
    }

    private void AddDebugScore(int points)
    {
        if (GameManager.Instance != null)
        {
            var player = GameManager.Instance.GetCurrentPlayer();
            if (player != null)
            {
                player.AddScore(points);
                
            }
        }
    }

    public void LogGameState()
    {
        
        
        if (QuestionManager.Instance != null)
        {
            
        }
        
        if (GameManager.Instance != null)
        {
            var player = GameManager.Instance.GetCurrentPlayer();
            if (player != null)
            {
                
                
                
            }
        }
        
        
    }
}

