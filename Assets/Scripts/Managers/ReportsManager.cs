using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class GameReport
{
    public string playerName;
    public int totalScore;
    public int questionsAnswered;
    public int correctAnswers;
    public float accuracy;
    public float averageTimePerQuestion;
    public List<string> categoriesPlayed;
    public string date;
    public int longestStreak;
    public int perfectAnswers; // Respuestas con bonus máximo

    public GameReport(PlayerData playerData, List<float> questionTimes, List<string> categories)
    {
        this.playerName = playerData.playerName;
        this.totalScore = playerData.score;
        this.questionsAnswered = playerData.totalQuestions;
        this.correctAnswers = playerData.correctAnswers;
        this.accuracy = playerData.GetAccuracy();
        this.categoriesPlayed = categories;
        this.date = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        
        // Calcular tiempo promedio
        if (questionTimes != null && questionTimes.Count > 0)
        {
            this.averageTimePerQuestion = questionTimes.Average();
        }
    }
}

[System.Serializable]
public class ReportsData
{
    public List<GameReport> reports = new List<GameReport>();
}

public class ReportsManager : MonoBehaviour
{
    public static ReportsManager Instance { get; private set; }

    private ReportsData reportsData;
    private string saveFilePath;
    
    // Tracking de la sesión actual
    private List<float> currentGameQuestionTimes;
    private List<string> currentGameCategories;
    private int currentStreak;
    private int longestStreak;
    private int perfectAnswers;

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
        saveFilePath = DataPathManager.GetReportsPath();
        LoadReports();
        ResetCurrentGameTracking();
    }

    public void StartNewGameTracking()
    {
        ResetCurrentGameTracking();
    }

    public void TrackQuestionTime(float timeSpent)
    {
        currentGameQuestionTimes.Add(timeSpent);
    }

    public void TrackCategory(string category)
    {
        if (!currentGameCategories.Contains(category))
        {
            currentGameCategories.Add(category);
        }
    }

    public void TrackAnswer(bool isCorrect, int bonusPoints)
    {
        if (isCorrect)
        {
            currentStreak++;
            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
            }

            // Considerar perfecta si obtiene el bonus máximo (100 puntos base + bonus)
            if (bonusPoints >= 90) // Ajustar según tu sistema de puntos
            {
                perfectAnswers++;
            }
        }
        else
        {
            currentStreak = 0;
        }
    }

    public void SaveGameReport(PlayerData playerData)
    {
        if (playerData == null) return;

        GameReport report = new GameReport(
            playerData,
            currentGameQuestionTimes,
            currentGameCategories
        );

        report.longestStreak = longestStreak;
        report.perfectAnswers = perfectAnswers;

        reportsData.reports.Add(report);
        
        // Mantener solo los últimos 1000 reportes para no ocupar mucho espacio
        if (reportsData.reports.Count > 1000)
        {
            reportsData.reports = reportsData.reports
                .OrderByDescending(r => System.DateTime.Parse(r.date))
                .Take(1000)
                .ToList();
        }

        SaveReports();
        ResetCurrentGameTracking();
        
        
    }

    private void ResetCurrentGameTracking()
    {
        currentGameQuestionTimes = new List<float>();
        currentGameCategories = new List<string>();
        currentStreak = 0;
        longestStreak = 0;
        perfectAnswers = 0;
    }

    // Métodos de consulta de estadísticas
    public List<GameReport> GetAllReports()
    {
        return reportsData?.reports ?? new List<GameReport>();
    }

    public List<GameReport> GetPlayerReports(string playerName)
    {
        if (string.IsNullOrEmpty(playerName) || reportsData?.reports == null)
            return new List<GameReport>();

        return reportsData.reports
            .Where(r => r.playerName.Equals(playerName, System.StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => System.DateTime.Parse(r.date))
            .ToList();
    }

    public GameReport GetLastReport()
    {
        if (reportsData?.reports == null || reportsData.reports.Count == 0)
            return null;

        return reportsData.reports
            .OrderByDescending(r => System.DateTime.Parse(r.date))
            .FirstOrDefault();
    }

    public int GetTotalGamesPlayed()
    {
        return reportsData?.reports?.Count ?? 0;
    }

    public float GetGlobalAverageAccuracy()
    {
        if (reportsData?.reports == null || reportsData.reports.Count == 0)
            return 0f;

        return reportsData.reports.Average(r => r.accuracy);
    }

    public float GetPlayerAverageAccuracy(string playerName)
    {
        var playerReports = GetPlayerReports(playerName);
        if (playerReports.Count == 0)
            return 0f;

        return playerReports.Average(r => r.accuracy);
    }

    public Dictionary<string, int> GetCategoryStatistics()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>();

        if (reportsData?.reports != null)
        {
            foreach (var report in reportsData.reports)
            {
                foreach (var category in report.categoriesPlayed)
                {
                    if (!stats.ContainsKey(category))
                    {
                        stats[category] = 0;
                    }
                    stats[category]++;
                }
            }
        }

        return stats;
    }

    public List<GameReport> GetRecentReports(int count = 10)
    {
        if (reportsData?.reports == null)
            return new List<GameReport>();

        return reportsData.reports
            .OrderByDescending(r => System.DateTime.Parse(r.date))
            .Take(count)
            .ToList();
    }

    public PlayerStatistics GetPlayerStatistics(string playerName)
    {
        var playerReports = GetPlayerReports(playerName);
        
        return new PlayerStatistics
        {
            totalGames = playerReports.Count,
            averageScore = playerReports.Count > 0 ? (float)playerReports.Average(r => r.totalScore) : 0,
            bestScore = playerReports.Count > 0 ? playerReports.Max(r => r.totalScore) : 0,
            averageAccuracy = playerReports.Count > 0 ? (float)playerReports.Average(r => r.accuracy) : 0,
            totalCorrectAnswers = playerReports.Sum(r => r.correctAnswers),
            totalQuestionsAnswered = playerReports.Sum(r => r.questionsAnswered),
            bestStreak = playerReports.Count > 0 ? playerReports.Max(r => r.longestStreak) : 0,
            totalPerfectAnswers = playerReports.Sum(r => r.perfectAnswers)
        };
    }

    public GlobalStatistics GetGlobalStatistics()
    {
        if (reportsData?.reports == null || reportsData.reports.Count == 0)
            return new GlobalStatistics();

        return new GlobalStatistics
        {
            totalGamesPlayed = reportsData.reports.Count,
            totalPlayers = reportsData.reports.Select(r => r.playerName).Distinct().Count(),
            averageScore = (float)reportsData.reports.Average(r => r.totalScore),
            highestScore = reportsData.reports.Max(r => r.totalScore),
            averageAccuracy = (float)reportsData.reports.Average(r => r.accuracy),
            totalQuestionsAnswered = reportsData.reports.Sum(r => r.questionsAnswered),
            categoryStats = GetCategoryStatistics()
        };
    }

    public void ClearReports()
    {
        reportsData.reports.Clear();
        SaveReports();
        
    }

    private void SaveReports()
    {
        try
        {
            string json = JsonUtility.ToJson(reportsData, true);
            File.WriteAllText(saveFilePath, json);
            
        }
        catch (System.Exception e)
        {
            
        }
    }

    private void LoadReports()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                reportsData = JsonUtility.FromJson<ReportsData>(json);
                
            }
            else
            {
                reportsData = new ReportsData();
                
            }
        }
        catch (System.Exception e)
        {
            
            reportsData = new ReportsData();
        }
    }

    public string GetSaveFilePath()
    {
        return saveFilePath;
    }
}

// Clases auxiliares para estadísticas
[System.Serializable]
public class PlayerStatistics
{
    public int totalGames;
    public float averageScore;
    public int bestScore;
    public float averageAccuracy;
    public int totalCorrectAnswers;
    public int totalQuestionsAnswered;
    public int bestStreak;
    public int totalPerfectAnswers;
}

[System.Serializable]
public class GlobalStatistics
{
    public int totalGamesPlayed;
    public int totalPlayers;
    public float averageScore;
    public int highestScore;
    public float averageAccuracy;
    public int totalQuestionsAnswered;
    public Dictionary<string, int> categoryStats;
}

