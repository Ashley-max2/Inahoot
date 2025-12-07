using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizUIManager : MonoBehaviour
{
    [Header("Referencias UI - Pregunta")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI questionNumberText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Referencias UI - Respuestas")]
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI[] answerTexts;

    [Header("Referencias UI - Puntuación")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;

    [Header("Colores")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;

    [Header("Configuración")]
    [SerializeField] private float feedbackDuration = 1.5f;
    [SerializeField] private string resultsSceneName = "Results";

    private Question currentQuestion;
    private float timeRemaining;
    private bool answerSelected = false;

    void Start()
    {
        SetupAnswerButtons();
        LoadNextQuestion();
    }

    private void SetupAnswerButtons()
    {
        // Conectar cada botón con su índice correspondiente
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                int index = i; // Capturar el índice en una variable local para el closure
                answerButtons[i].onClick.RemoveAllListeners(); // Limpiar listeners anteriores
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
        }
    }

    void Update()
    {
        if (!answerSelected && currentQuestion != null)
        {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                TimeUp();
            }
            
            UpdateTimerDisplay();
        }
    }

    public void LoadNextQuestion()
    {
        if (!QuestionManager.Instance.HasMoreQuestions())
        {
            EndQuiz();
            return;
        }

        currentQuestion = QuestionManager.Instance.GetCurrentQuestion();
        
        if (currentQuestion == null)
        {
            EndQuiz();
            return;
        }

        DisplayQuestion();
        ResetAnswerButtons();
        
        timeRemaining = currentQuestion.timeLimit;
        answerSelected = false;
        
        GameManager.Instance.StartQuestion();
    }

    private void DisplayQuestion()
    {
        // Mostrar texto de la pregunta
        if (questionText != null)
            questionText.text = currentQuestion.questionText;

        // Mostrar número de pregunta
        if (questionNumberText != null)
            questionNumberText.text = $"Pregunta {QuestionManager.Instance.GetCurrentQuestionNumber()}/{QuestionManager.Instance.GetTotalQuestions()}";

        // Mostrar categoría
        if (categoryText != null)
            categoryText.text = currentQuestion.category;

        // Mostrar respuestas
        int maxAnswers = Mathf.Min(answerButtons.Length, answerTexts.Length, currentQuestion.answers.Count);
        
        for (int i = 0; i < maxAnswers; i++)
        {
            if (answerTexts[i] != null)
                answerTexts[i].text = currentQuestion.answers[i];
            
            if (answerButtons[i] != null)
                answerButtons[i].gameObject.SetActive(true);
        }

        // Ocultar botones extra si hay menos de 4 respuestas
        for (int i = maxAnswers; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].gameObject.SetActive(false);
        }
    }

    private void ResetAnswerButtons()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = true;
            var buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.color = normalColor;
        }
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (answerSelected) return;

        answerSelected = true;
        
        // Deshabilitar todos los botones
        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }

        // Enviar respuesta al GameManager
        GameManager.Instance.SubmitAnswer(answerIndex);

        // Mostrar feedback visual
        StartCoroutine(ShowAnswerFeedback(answerIndex));
    }

    private IEnumerator ShowAnswerFeedback(int selectedIndex)
    {
        // Colorear la respuesta seleccionada
        var selectedButton = answerButtons[selectedIndex].GetComponent<Image>();
        bool isCorrect = currentQuestion.IsCorrectAnswer(selectedIndex);
        
        if (selectedButton != null)
        {
            selectedButton.color = isCorrect ? correctColor : incorrectColor;
        }

        // Si es incorrecta, mostrar también la correcta
        if (!isCorrect)
        {
            var correctButton = answerButtons[currentQuestion.correctAnswerIndex].GetComponent<Image>();
            if (correctButton != null)
            {
                correctButton.color = correctColor;
            }
        }

        // Actualizar puntuación
        UpdateScoreDisplay();

        // Esperar antes de continuar
        yield return new WaitForSeconds(feedbackDuration);

        // Cargar siguiente pregunta
        QuestionManager.Instance.GetNextQuestion();
        LoadNextQuestion();
    }

    private void TimeUp()
    {
        if (answerSelected) return;
        
        answerSelected = true;
        
        // Marcar como incorrecta por tiempo
        GameManager.Instance.SubmitAnswer(-1);
        
        // Mostrar la respuesta correcta
        var correctButton = answerButtons[currentQuestion.correctAnswerIndex].GetComponent<Image>();
        if (correctButton != null)
        {
            correctButton.color = correctColor;
        }

        // Deshabilitar botones
        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }

        StartCoroutine(ContinueAfterTimeout());
    }

    private IEnumerator ContinueAfterTimeout()
    {
        yield return new WaitForSeconds(feedbackDuration);
        
        QuestionManager.Instance.GetNextQuestion();
        LoadNextQuestion();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = $"Tiempo: {Mathf.CeilToInt(timeRemaining)}s";
            
            // Cambiar color si queda poco tiempo
            if (timeRemaining <= 5)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: {GameManager.Instance.GetCurrentScore()}";
        }

        if (accuracyText != null)
        {
            accuracyText.text = $"Precisión: {GameManager.Instance.GetAccuracy():F0}%";
        }
    }

    private void EndQuiz()
    {
        
        
        if (GameManager.Instance == null)
        {
            
            return;
        }
        
        GameManager.Instance.EndGame();
        
        // Verificar que los datos estén guardados
        PlayerData player = GameManager.Instance.GetCurrentPlayer();
        if (player != null)
        {
            
            
            
        }
        else
        {
            
        }
        
        // Cargar escena de resultados
        if (!string.IsNullOrEmpty(resultsSceneName))
        {
            
            SceneManager.LoadScene(resultsSceneName);
        }
    }
}

