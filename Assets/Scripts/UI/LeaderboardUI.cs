using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Leaderboard Display")]
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    
    [Header("Filter Options")]
    [SerializeField] private TMP_Dropdown filterDropdown;
    [SerializeField] private TMP_InputField searchInput;
    
    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI totalGamesText;
    [SerializeField] private TextMeshProUGUI averageScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    [Header("Buttons")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button backButton;

    [Header("Colors")]
    [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);

    private List<LeaderboardEntry> currentEntries;

    void Start()
    {
        SetupButtons();
        SetupFilterDropdown();
        RefreshLeaderboard();
    }

    private void SetupButtons()
    {
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshLeaderboard);
        
        if (clearButton != null)
            clearButton.onClick.AddListener(OnClearLeaderboard);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackToMainMenu);
        
        if (searchInput != null)
            searchInput.onEndEdit.AddListener((value) => FilterByPlayerName(value));
    }

    private void SetupFilterDropdown()
    {
        if (filterDropdown != null)
        {
            filterDropdown.ClearOptions();
            filterDropdown.AddOptions(new List<string>
            {
                "Top 10",
                "Top 25",
                "Top 50",
                "Todos"
            });
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        }
    }

    private void RefreshLeaderboard()
    {
        currentEntries = GetFilteredEntries();
        DisplayLeaderboard(currentEntries);
        UpdateStats();
    }

    private void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        if (leaderboardContent == null) return;

        // Limpiar entradas anteriores
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        if (entries == null || entries.Count == 0)
        {
            CreateEmptyMessage();
            return;
        }

        // Crear entradas
        for (int i = 0; i < entries.Count; i++)
        {
            CreateLeaderboardEntry(entries[i], i + 1);
        }
    }

    private void CreateLeaderboardEntry(LeaderboardEntry entry, int rank)
    {
        GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
        
        // Buscar componentes de texto (ajustar según tu prefab)
        TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (texts.Length >= 5)
        {
            texts[0].text = rank.ToString();
            texts[1].text = entry.playerName;
            texts[2].text = entry.score.ToString();
            texts[3].text = $"{entry.accuracy:F1}%";
            texts[4].text = entry.date;

            // Colorear top 3
            if (rank == 1)
            {
                texts[0].color = goldColor;
                texts[1].fontStyle = FontStyles.Bold;
            }
            else if (rank == 2)
            {
                texts[0].color = silverColor;
                texts[1].fontStyle = FontStyles.Bold;
            }
            else if (rank == 3)
            {
                texts[0].color = bronzeColor;
                texts[1].fontStyle = FontStyles.Bold;
            }
        }
    }

    private void CreateEmptyMessage()
    {
        GameObject emptyObj = new GameObject("EmptyMessage");
        emptyObj.transform.SetParent(leaderboardContent);
        
        TextMeshProUGUI text = emptyObj.AddComponent<TextMeshProUGUI>();
        text.text = "No hay puntuaciones registradas aún.\n¡Juega para aparecer en el leaderboard!";
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.gray;
        
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 200);
    }

    private void UpdateStats()
    {
        if (totalGamesText != null)
        {
            int totalGames = LeaderboardManager.Instance.GetTotalGamesPlayed();
            totalGamesText.text = $"Partidas Totales: {totalGames}";
        }

        if (averageScoreText != null)
        {
            float avgScore = LeaderboardManager.Instance.GetAverageScore();
            averageScoreText.text = $"Puntuación Media: {avgScore:F0}";
        }

        if (highScoreText != null)
        {
            int highScore = LeaderboardManager.Instance.GetHighestScore();
            highScoreText.text = $"Mejor Puntuación: {highScore}";
        }
    }

    private List<LeaderboardEntry> GetFilteredEntries()
    {
        int filterValue = filterDropdown?.value ?? 0;
        
        switch (filterValue)
        {
            case 0: return LeaderboardManager.Instance.GetTopScores(10);
            case 1: return LeaderboardManager.Instance.GetTopScores(25);
            case 2: return LeaderboardManager.Instance.GetTopScores(50);
            case 3: return LeaderboardManager.Instance.GetAllEntries();
            default: return LeaderboardManager.Instance.GetTopScores(10);
        }
    }

    private void OnFilterChanged(int value)
    {
        RefreshLeaderboard();
    }

    private void FilterByPlayerName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            RefreshLeaderboard();
            return;
        }

        List<LeaderboardEntry> allEntries = LeaderboardManager.Instance.GetAllEntries();
        List<LeaderboardEntry> filtered = allEntries.FindAll(e => 
            e.playerName.ToLower().Contains(playerName.ToLower()));
        
        DisplayLeaderboard(filtered);
    }

    private void OnClearLeaderboard()
    {
        // Confirmar antes de borrar
        if (ConfirmClear())
        {
            LeaderboardManager.Instance.ClearLeaderboard();
            RefreshLeaderboard();
        }
    }

    private bool ConfirmClear()
    {
        // En una implementación real, mostrarías un diálogo de confirmación
        // Por ahora, retornamos true directamente
        // TODO: Implementar diálogo de confirmación
        
        return true;
    }

    private void OnBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Métodos públicos para usar desde otros scripts
    public void HighlightPlayer(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return;
        
        searchInput.text = playerName;
        FilterByPlayerName(playerName);
    }

    public void ShowPlayerStats(string playerName)
    {
        LeaderboardEntry bestScore = LeaderboardManager.Instance.GetPlayerBestScore(playerName);
        if (bestScore != null)
        {
            int rank = LeaderboardManager.Instance.GetPlayerRank(playerName);
            float avgScore = LeaderboardManager.Instance.GetPlayerAverageScore(playerName);
            
            
        }
    }
}

