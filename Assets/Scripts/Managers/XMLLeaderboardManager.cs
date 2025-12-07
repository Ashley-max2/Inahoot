using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;

/// <summary>
/// Gestiona leaderboards por Kahoot usando archivos XML
/// Un archivo XML por cada Kahoot
/// </summary>
public class XMLLeaderboardManager : MonoBehaviour
{
    public static XMLLeaderboardManager Instance { get; private set; }
    
    private string leaderboardsPath;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Usar DataPathManager para rutas visibles
        leaderboardsPath = DataPathManager.GetLeaderboardsPath() + "/";
    }

    /// <summary>
    /// Guarda una nueva entrada en el leaderboard del Kahoot especificado
    /// </summary>
    public void SaveEntry(string kahootName, LeaderboardEntry entry)
    {
        if (string.IsNullOrEmpty(kahootName) || entry == null)
        {
            
            return;
        }

        string fileName = SanitizeFileName(kahootName) + ".xml";
        string fullPath = Path.Combine(leaderboardsPath, fileName);

        try
        {
            XmlDocument doc = new XmlDocument();

            // Cargar XML existente o crear nuevo
            if (File.Exists(fullPath))
            {
                doc.Load(fullPath);
            }
            else
            {
                // Crear estructura XML nueva
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(declaration);

                XmlElement newRoot = doc.CreateElement("Leaderboard");
                newRoot.SetAttribute("kahoot", kahootName);
                doc.AppendChild(newRoot);
            }

            // Añadir nueva entrada
            XmlElement root = doc.DocumentElement;
            XmlElement entryElement = CreateEntryElement(doc, entry);
            root.AppendChild(entryElement);

            // Guardar archivo
            doc.Save(fullPath);
            
            
        }
        catch (Exception ex)
        {
            
            
            if (ErrorReportManager.Instance != null)
            {
                ErrorReportManager.Instance.ReportXMLError(
                    fullPath,
                    "Error al escribir en el archivo XML del leaderboard",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Carga las entradas del leaderboard de un Kahoot específico
    /// </summary>
    public List<LeaderboardEntry> LoadLeaderboard(string kahootName)
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        if (string.IsNullOrEmpty(kahootName))
        {
            return entries;
        }

        string fileName = SanitizeFileName(kahootName) + ".xml";
        string fullPath = Path.Combine(leaderboardsPath, fileName);

        if (!File.Exists(fullPath))
        {
            
            return entries;
        }

        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fullPath);

            XmlNodeList entryNodes = doc.SelectNodes("/Leaderboard/Entry");

            foreach (XmlNode node in entryNodes)
            {
                try
                {
                    LeaderboardEntry entry = ParseEntryFromXml(node);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

            // Ordenar por puntuación descendente
            entries = entries.OrderByDescending(e => e.score).ToList();
            
            
        }
        catch (Exception ex)
        {
            
            
            if (ErrorReportManager.Instance != null)
            {
                ErrorReportManager.Instance.ReportXMLError(
                    fullPath,
                    "Error al leer o parsear el archivo XML del leaderboard. Se devolverá un leaderboard vacío.",
                    ex
                );
            }
            
            // Devolver lista vacía como fallback
            return new List<LeaderboardEntry>();
        }

        return entries;
    }

    /// <summary>
    /// Obtiene la lista de Kahoots que tienen leaderboards
    /// </summary>
    public List<string> GetAvailableLeaderboards()
    {
        List<string> kahoots = new List<string>();

        try
        {
            string[] files = Directory.GetFiles(leaderboardsPath, "*.xml");

            foreach (string file in files)
            {
                string kahootName = Path.GetFileNameWithoutExtension(file);
                kahoots.Add(kahootName);
            }
        }
        catch (Exception ex)
        {
            
        }

        return kahoots;
    }

    /// <summary>
    /// Obtiene el top N de jugadores de un Kahoot
    /// </summary>
    public List<LeaderboardEntry> GetTopScores(string kahootName, int count = 10)
    {
        List<LeaderboardEntry> allEntries = LoadLeaderboard(kahootName);
        return allEntries.Take(count).ToList();
    }

    /// <summary>
    /// Elimina el leaderboard de un Kahoot
    /// </summary>
    public void ClearLeaderboard(string kahootName)
    {
        if (string.IsNullOrEmpty(kahootName))
        {
            return;
        }

        string fileName = SanitizeFileName(kahootName) + ".xml";
        string fullPath = Path.Combine(leaderboardsPath, fileName);

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    /// <summary>
    /// Elimina todos los leaderboards
    /// </summary>
    public void ClearAllLeaderboards()
    {
        try
        {
            string[] files = Directory.GetFiles(leaderboardsPath, "*.xml");

            foreach (string file in files)
            {
                File.Delete(file);
            }

            
        }
        catch (Exception ex)
        {
            
        }
    }

    private XmlElement CreateEntryElement(XmlDocument doc, LeaderboardEntry entry)
    {
        XmlElement entryElement = doc.CreateElement("Entry");

        // Crear sub-elementos
        XmlElement playerName = doc.CreateElement("PlayerName");
        playerName.InnerText = entry.playerName;
        entryElement.AppendChild(playerName);

        XmlElement score = doc.CreateElement("Score");
        score.InnerText = entry.score.ToString();
        entryElement.AppendChild(score);

        XmlElement accuracy = doc.CreateElement("Accuracy");
        accuracy.InnerText = entry.accuracy.ToString("F2");
        entryElement.AppendChild(accuracy);

        XmlElement date = doc.CreateElement("Date");
        date.InnerText = entry.date;
        entryElement.AppendChild(date);

        XmlElement questionsAnswered = doc.CreateElement("QuestionsAnswered");
        questionsAnswered.InnerText = entry.questionsAnswered.ToString();
        entryElement.AppendChild(questionsAnswered);

        XmlElement correctAnswers = doc.CreateElement("CorrectAnswers");
        correctAnswers.InnerText = entry.correctAnswers.ToString();
        entryElement.AppendChild(correctAnswers);

        return entryElement;
    }

    private LeaderboardEntry ParseEntryFromXml(XmlNode node)
    {
        string playerName = node.SelectSingleNode("PlayerName")?.InnerText ?? "Unknown";
        int score = int.Parse(node.SelectSingleNode("Score")?.InnerText ?? "0");
        float accuracy = float.Parse(node.SelectSingleNode("Accuracy")?.InnerText ?? "0");
        string date = node.SelectSingleNode("Date")?.InnerText ?? "";
        int questionsAnswered = int.Parse(node.SelectSingleNode("QuestionsAnswered")?.InnerText ?? "0");
        int correctAnswers = int.Parse(node.SelectSingleNode("CorrectAnswers")?.InnerText ?? "0");

        return new LeaderboardEntry(playerName, score, accuracy, questionsAnswered, correctAnswers)
        {
            date = date
        };
    }

    private string SanitizeFileName(string fileName)
    {
        // Eliminar caracteres no válidos para nombres de archivo
        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            fileName = fileName.Replace(c.ToString(), "_");
        }
        return fileName;
    }

    /// <summary>
    /// Obtiene estadísticas del Kahoot
    /// </summary>
    public Dictionary<string, object> GetKahootStats(string kahootName)
    {
        List<LeaderboardEntry> entries = LoadLeaderboard(kahootName);
        
        Dictionary<string, object> stats = new Dictionary<string, object>();
        stats["totalGames"] = entries.Count;
        stats["averageScore"] = entries.Count > 0 ? entries.Average(e => e.score) : 0;
        stats["highestScore"] = entries.Count > 0 ? entries.Max(e => e.score) : 0;
        stats["averageAccuracy"] = entries.Count > 0 ? entries.Average(e => e.accuracy) : 0;
        
        return stats;
    }
}

