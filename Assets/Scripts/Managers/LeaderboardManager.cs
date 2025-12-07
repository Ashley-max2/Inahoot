using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public float accuracy;
    public string date;
    public int questionsAnswered;
    public int correctAnswers;

    public LeaderboardEntry(string name, int score, float accuracy, int questionsAnswered, int correctAnswers)
    {
        this.playerName = name;
        this.score = score;
        this.accuracy = accuracy;
        this.questionsAnswered = questionsAnswered;
        this.correctAnswers = correctAnswers;
        this.date = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private LeaderboardData leaderboardData;
    private string saveFilePath;
    private const int MAX_ENTRIES = 100; // Guardar más entradas para estadísticas
    private const int TOP_DISPLAY = 10; // Mostrar top 10 en la UI

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

        // Usar DataPathManager para rutas visibles
        saveFilePath = DataPathManager.GetGlobalLeaderboardPath();
        LoadLeaderboard();
    }

    public void AddScore(PlayerData playerData)
    {
        if (playerData == null || string.IsNullOrEmpty(playerData.playerName))
        {
            
            return;
        }

        LeaderboardEntry newEntry = new LeaderboardEntry(
            playerData.playerName,
            playerData.score,
            playerData.GetAccuracy(),
            playerData.totalQuestions,
            playerData.correctAnswers
        );

        leaderboardData.entries.Add(newEntry);
        
        // Ordenar por puntuación descendente
        leaderboardData.entries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .Take(MAX_ENTRIES)
            .ToList();

        SaveLeaderboard();
        
        
    }

    public List<LeaderboardEntry> GetTopScores(int count = 10)
    {
        if (leaderboardData == null || leaderboardData.entries == null)
        {
            return new List<LeaderboardEntry>();
        }

        return leaderboardData.entries.Take(count).ToList();
    }

    public List<LeaderboardEntry> GetAllEntries()
    {
        return leaderboardData?.entries ?? new List<LeaderboardEntry>();
    }

    public int GetPlayerRank(string playerName)
    {
        if (string.IsNullOrEmpty(playerName) || leaderboardData?.entries == null)
            return -1;

        for (int i = 0; i < leaderboardData.entries.Count; i++)
        {
            if (leaderboardData.entries[i].playerName.Equals(playerName, System.StringComparison.OrdinalIgnoreCase))
            {
                return i + 1;
            }
        }

        return -1;
    }

    public LeaderboardEntry GetPlayerBestScore(string playerName)
    {
        if (string.IsNullOrEmpty(playerName) || leaderboardData?.entries == null)
            return null;

        return leaderboardData.entries
            .Where(e => e.playerName.Equals(playerName, System.StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.score)
            .FirstOrDefault();
    }

    public int GetTotalGamesPlayed()
    {
        return leaderboardData?.entries?.Count ?? 0;
    }

    public float GetAverageScore()
    {
        if (leaderboardData?.entries == null || leaderboardData.entries.Count == 0)
            return 0f;

        return (float)leaderboardData.entries.Average(e => e.score);
    }

    public int GetHighestScore()
    {
        if (leaderboardData?.entries == null || leaderboardData.entries.Count == 0)
            return 0;

        return leaderboardData.entries.Max(e => e.score);
    }

    public void ClearLeaderboard()
    {
        leaderboardData.entries.Clear();
        SaveLeaderboard();
        
    }

    private void SaveLeaderboard()
    {
        try
        {
            string json = JsonUtility.ToJson(leaderboardData, true);
            File.WriteAllText(saveFilePath, json);
            
        }
        catch (System.Exception ex)
        {
            
            if (ErrorReportManager.Instance != null)
            {
                ErrorReportManager.Instance.ReportJSONError(
                    saveFilePath,
                    "No se pudo guardar el leaderboard",
                    ex
                );
            }
        }
    }

    private void LoadLeaderboard()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
                
                if (leaderboardData == null || leaderboardData.entries == null)
                {
                    throw new System.Exception("Leaderboard JSON corrupto o vacío");
                }
                
                
            }
            else
            {
                leaderboardData = new LeaderboardData();
                
            }
        }
        catch (System.Exception ex)
        {
            
            if (ErrorReportManager.Instance != null)
            {
                ErrorReportManager.Instance.ReportJSONError(
                    saveFilePath,
                    "Error al leer o parsear el archivo de leaderboard. Se creará uno nuevo.",
                    ex
                );
            }
            leaderboardData = new LeaderboardData();
        }
    }

    public string GetSaveFilePath()
    {
        return saveFilePath;
    }

    // Métodos para estadísticas avanzadas
    public Dictionary<string, int> GetPlayerGameCounts()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        
        if (leaderboardData?.entries != null)
        {
            foreach (var entry in leaderboardData.entries)
            {
                if (!counts.ContainsKey(entry.playerName))
                {
                    counts[entry.playerName] = 0;
                }
                counts[entry.playerName]++;
            }
        }

        return counts;
    }

    public float GetPlayerAverageScore(string playerName)
    {
        if (string.IsNullOrEmpty(playerName) || leaderboardData?.entries == null)
            return 0f;

        var playerEntries = leaderboardData.entries
            .Where(e => e.playerName.Equals(playerName, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (playerEntries.Count == 0)
            return 0f;

        return (float)playerEntries.Average(e => e.score);
    }

    public List<LeaderboardEntry> GetRecentGames(int count = 10)
    {
        if (leaderboardData?.entries == null)
            return new List<LeaderboardEntry>();

        // Las entradas más recientes están al final antes de ordenar por score
        // Necesitamos mantener un registro temporal de fecha de inserción
        return leaderboardData.entries.Take(count).ToList();
    }
}

