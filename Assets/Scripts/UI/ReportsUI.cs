using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReportsUI : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private GameObject playerStatsPanel;
    [SerializeField] private GameObject globalStatsPanel;
    [SerializeField] private GameObject recentGamesPanel;
    
    [Header("Tabs")]
    [SerializeField] private Button playerStatsTabButton;
    [SerializeField] private Button globalStatsTabButton;
    [SerializeField] private Button recentGamesTabButton;
    
    [Header("Player Stats")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button searchPlayerButton;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI totalGamesText;
    [SerializeField] private TextMeshProUGUI averageScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI perfectAnswersText;
    
    [Header("Global Stats")]
    [SerializeField] private TextMeshProUGUI totalPlayersText;
    [SerializeField] private TextMeshProUGUI totalGamesGlobalText;
    [SerializeField] private TextMeshProUGUI globalAverageScoreText;
    [SerializeField] private TextMeshProUGUI globalHighScoreText;
    [SerializeField] private TextMeshProUGUI globalAccuracyText;
    [SerializeField] private TextMeshProUGUI totalQuestionsText;
    [SerializeField] private Transform categoriesListContent;
    
    [Header("Recent Games")]
    [SerializeField] private Transform recentGamesContent;
    [SerializeField] private GameObject reportEntryPrefab;
    [SerializeField] private TMP_Dropdown recentGamesFilterDropdown;
    
    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button refreshButton;

    void Start()
    {
        SetupButtons();
        SetupTabs();
        ShowPlayerStatsPanel();
    }

    private void SetupButtons()
    {
        if (searchPlayerButton != null)
            searchPlayerButton.onClick.AddListener(OnSearchPlayer);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackToMainMenu);
        
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshCurrentPanel);
        
        if (recentGamesFilterDropdown != null)
            recentGamesFilterDropdown.onValueChanged.AddListener(OnRecentGamesFilterChanged);
    }

    private void SetupTabs()
    {
        if (playerStatsTabButton != null)
            playerStatsTabButton.onClick.AddListener(ShowPlayerStatsPanel);
        
        if (globalStatsTabButton != null)
            globalStatsTabButton.onClick.AddListener(ShowGlobalStatsPanel);
        
        if (recentGamesTabButton != null)
            recentGamesTabButton.onClick.AddListener(ShowRecentGamesPanel);
    }

    private void ShowPlayerStatsPanel()
    {
        SetPanelActive(playerStatsPanel);
    }

    private void ShowGlobalStatsPanel()
    {
        SetPanelActive(globalStatsPanel);
        UpdateGlobalStats();
    }

    private void ShowRecentGamesPanel()
    {
        SetPanelActive(recentGamesPanel);
        UpdateRecentGames();
    }

    private void SetPanelActive(GameObject activePanel)
    {
        if (playerStatsPanel != null) playerStatsPanel.SetActive(false);
        if (globalStatsPanel != null) globalStatsPanel.SetActive(false);
        if (recentGamesPanel != null) recentGamesPanel.SetActive(false);
        
        if (activePanel != null) activePanel.SetActive(true);
    }

    private void OnSearchPlayer()
    {
        string playerName = playerNameInput?.text ?? "";
        
        if (string.IsNullOrWhiteSpace(playerName))
        {
            
            return;
        }

        UpdatePlayerStats(playerName);
    }

    private void UpdatePlayerStats(string playerName)
    {
        PlayerStatistics stats = ReportsManager.Instance.GetPlayerStatistics(playerName);

        if (stats.totalGames == 0)
        {
            ShowNoDataMessage("No hay datos para este jugador");
            return;
        }

        if (playerNameText != null)
            playerNameText.text = $"Estadísticas de: {playerName}";
        
        if (totalGamesText != null)
            totalGamesText.text = $"Partidas Jugadas: {stats.totalGames}";
        
        if (averageScoreText != null)
            averageScoreText.text = $"Puntuación Media: {stats.averageScore:F0}";
        
        if (bestScoreText != null)
            bestScoreText.text = $"Mejor Puntuación: {stats.bestScore}";
        
        if (accuracyText != null)
            accuracyText.text = $"Precisión Media: {stats.averageAccuracy:F1}%";
        
        if (streakText != null)
            streakText.text = $"Mejor Racha: {stats.bestStreak}";
        
        if (perfectAnswersText != null)
            perfectAnswersText.text = $"Respuestas Perfectas: {stats.totalPerfectAnswers}";
    }

    private void UpdateGlobalStats()
    {
        GlobalStatistics stats = ReportsManager.Instance.GetGlobalStatistics();

        if (totalPlayersText != null)
            totalPlayersText.text = $"Total Jugadores: {stats.totalPlayers}";
        
        if (totalGamesGlobalText != null)
            totalGamesGlobalText.text = $"Partidas Totales: {stats.totalGamesPlayed}";
        
        if (globalAverageScoreText != null)
            globalAverageScoreText.text = $"Puntuación Media: {stats.averageScore:F0}";
        
        if (globalHighScoreText != null)
            globalHighScoreText.text = $"Mejor Puntuación: {stats.highestScore}";
        
        if (globalAccuracyText != null)
            globalAccuracyText.text = $"Precisión Global: {stats.averageAccuracy:F1}%";
        
        if (totalQuestionsText != null)
            totalQuestionsText.text = $"Preguntas Respondidas: {stats.totalQuestionsAnswered}";

        UpdateCategoryStats(stats.categoryStats);
    }

    private void UpdateCategoryStats(Dictionary<string, int> categoryStats)
    {
        if (categoriesListContent == null) return;

        // Limpiar lista
        foreach (Transform child in categoriesListContent)
        {
            Destroy(child.gameObject);
        }

        if (categoryStats == null || categoryStats.Count == 0)
            return;

        // Ordenar por frecuencia
        var sortedCategories = categoryStats.OrderByDescending(kvp => kvp.Value);

        foreach (var kvp in sortedCategories)
        {
            GameObject item = new GameObject("CategoryItem");
            item.transform.SetParent(categoriesListContent);
            
            TextMeshProUGUI text = item.AddComponent<TextMeshProUGUI>();
            text.text = $"{kvp.Key}: {kvp.Value} veces";
            text.fontSize = 18;
            text.color = Color.white;
        }
    }

    private void UpdateRecentGames(int count = 10)
    {
        if (recentGamesContent == null) return;

        // Limpiar lista
        foreach (Transform child in recentGamesContent)
        {
            Destroy(child.gameObject);
        }

        List<GameReport> recentReports = ReportsManager.Instance.GetRecentReports(count);

        if (recentReports.Count == 0)
        {
            CreateNoDataMessage(recentGamesContent);
            return;
        }

        foreach (GameReport report in recentReports)
        {
            CreateReportEntry(report);
        }
    }

    private void CreateReportEntry(GameReport report)
    {
        GameObject entry = Instantiate(reportEntryPrefab, recentGamesContent);
        
        TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (texts.Length >= 5)
        {
            texts[0].text = report.playerName;
            texts[1].text = report.totalScore.ToString();
            texts[2].text = $"{report.accuracy:F1}%";
            texts[3].text = $"{report.correctAnswers}/{report.questionsAnswered}";
            texts[4].text = report.date;
        }
    }

    private void CreateNoDataMessage(Transform parent)
    {
        GameObject obj = new GameObject("NoDataMessage");
        obj.transform.SetParent(parent);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = "No hay datos disponibles";
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.gray;
    }

    private void ShowNoDataMessage(string message)
    {
        if (playerNameText != null)
            playerNameText.text = message;
        
        if (totalGamesText != null) totalGamesText.text = "";
        if (averageScoreText != null) averageScoreText.text = "";
        if (bestScoreText != null) bestScoreText.text = "";
        if (accuracyText != null) accuracyText.text = "";
        if (streakText != null) streakText.text = "";
        if (perfectAnswersText != null) perfectAnswersText.text = "";
    }

    private void OnRecentGamesFilterChanged(int value)
    {
        switch (value)
        {
            case 0: UpdateRecentGames(10); break;
            case 1: UpdateRecentGames(25); break;
            case 2: UpdateRecentGames(50); break;
            default: UpdateRecentGames(10); break;
        }
    }

    private void RefreshCurrentPanel()
    {
        if (playerStatsPanel != null && playerStatsPanel.activeSelf)
        {
            OnSearchPlayer();
        }
        else if (globalStatsPanel != null && globalStatsPanel.activeSelf)
        {
            UpdateGlobalStats();
        }
        else if (recentGamesPanel != null && recentGamesPanel.activeSelf)
        {
            UpdateRecentGames();
        }
    }

    private void OnBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Método público para mostrar estadísticas de un jugador específico
    public void ShowPlayerStats(string playerName)
    {
        ShowPlayerStatsPanel();
        if (playerNameInput != null)
            playerNameInput.text = playerName;
        UpdatePlayerStats(playerName);
    }
}

