using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultsUIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI correctAnswersText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Botones")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Configuración")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        StartCoroutine(InitializeResults());
    }

    private IEnumerator InitializeResults()
    {
        // Esperar un frame para asegurar que todos los Managers estén inicializados
        yield return null;
        
        // Verificar que GameManager existe
        if (GameManager.Instance == null)
        {
            
            
            // Buscar GameManager en la escena
            GameManager gameManager = FindObjectOfType<GameManager>();
            
            if (gameManager == null)
            {
                
                // Mostrar mensaje de error al usuario
                if (messageText != null)
                {
                    messageText.text = "Error: No se pueden cargar los resultados. Vuelve al menú principal.";
                }
                yield break;
            }
        }
        
        // Crear LeaderboardManager si no existe
        if (LeaderboardManager.Instance == null)
        {
            GameObject leaderboardManagerObj = new GameObject("LeaderboardManager");
            leaderboardManagerObj.AddComponent<LeaderboardManager>();
            
            yield return null; // Esperar otro frame para que Awake() se ejecute
        }

        // Crear XMLLeaderboardManager si no existe
        if (XMLLeaderboardManager.Instance == null)
        {
            GameObject xmlLeaderboardManagerObj = new GameObject("XMLLeaderboardManager");
            xmlLeaderboardManagerObj.AddComponent<XMLLeaderboardManager>();
            
            yield return null;
        }
        
        // Crear ReportsManager si no existe
        if (ReportsManager.Instance == null)
        {
            GameObject reportsManagerObj = new GameObject("ReportsManager");
            reportsManagerObj.AddComponent<ReportsManager>();
            
            yield return null; // Esperar otro frame para que Awake() se ejecute
        }
        
        DisplayResults();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveAllListeners();
            playAgainButton.onClick.AddListener(PlayAgain);
            
        }
        else
        {
            
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
        }
        else
        {
            
        }
    }

    private void DisplayResults()
    {
        
        
        if (GameManager.Instance == null)
        {
            
            return;
        }

        PlayerData player = GameManager.Instance.GetCurrentPlayer();
        
        if (player == null)
        {
            
            return;
        }

        
        
        
        
        

        // Mostrar nombre del jugador
        if (playerNameText != null)
        {
            playerNameText.text = $"Jugador: {player.playerName}";
            
        }
        else
        {
            
        }

        // Mostrar puntuación final
        if (finalScoreText != null)
        {
            finalScoreText.text = player.score.ToString();
            
        }
        else
        {
            
        }

        // Mostrar precisión
        if (accuracyText != null)
        {
            accuracyText.text = $"Precisión: {player.GetAccuracy():F1}%";
            
        }
        else
        {
            
        }

        // Mostrar respuestas correctas
        if (correctAnswersText != null)
        {
            correctAnswersText.text = $"Respuestas correctas: {player.correctAnswers}/{player.totalQuestions}";
            
        }
        else
        {
            
        }

        // Mostrar mensaje personalizado
        if (messageText != null)
        {
            string message = GetPersonalizedMessage(player.GetAccuracy());
            messageText.text = message;
            
        }
        else
        {
            
        }

        // Guardar en leaderboard JSON (global)
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddScore(player);
            
        }
        else
        {
            
        }

        // Guardar en leaderboard XML (por Kahoot)
        if (XMLLeaderboardManager.Instance != null && QuestionManager.Instance != null)
        {
            string kahootName = QuestionManager.Instance.GetCurrentKahootName();
            
            LeaderboardEntry entry = new LeaderboardEntry(
                player.playerName,
                player.score,
                player.GetAccuracy(),
                player.totalQuestions,
                player.correctAnswers
            );
            
            XMLLeaderboardManager.Instance.SaveEntry(kahootName, entry);
            
        }
        else
        {
            
        }

        // Guardar reporte
        if (ReportsManager.Instance != null)
        {
            ReportsManager.Instance.SaveGameReport(player);
            
        }
        else
        {
            
        }
        
        
    }

    private string GetPersonalizedMessage(float accuracy)
    {
        if (accuracy >= 90)
            return "¡Increíble! ¡Eres un verdadero experto en Inazuma Eleven!";
        else if (accuracy >= 75)
            return "¡Excelente trabajo! ¡Conoces muy bien Inazuma Eleven!";
        else if (accuracy >= 60)
            return "¡Buen trabajo! Pero puedes mejorar aún más.";
        else if (accuracy >= 40)
            return "No está mal, pero necesitas estudiar más sobre Inazuma Eleven.";
        else
            return "Parece que necesitas ver más Inazuma Eleven. ¡Inténtalo de nuevo!";
    }

    public void ReturnToMainMenu()
    {
        
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PlayAgain()
    {
        
        
        if (GameManager.Instance == null)
        {
            
            return;
        }
        
        PlayerData currentPlayer = GameManager.Instance.GetCurrentPlayer();
        
        if (currentPlayer == null)
        {
            
            return;
        }
        
        string playerName = currentPlayer.playerName;
        
        
        GameManager.Instance.StartNewGame(playerName);
        
        
        SceneManager.LoadScene("Game");
    }
}

