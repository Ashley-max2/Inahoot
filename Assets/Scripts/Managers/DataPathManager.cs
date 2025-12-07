using UnityEngine;
using System.IO;

public class DataPathManager : MonoBehaviour
{
    private static DataPathManager instance;
    public static DataPathManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DataPathManager");
                instance = go.AddComponent<DataPathManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private static string dataRootPath;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDataPath();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDataPath()
    {
#if UNITY_EDITOR
        dataRootPath = Path.Combine(Application.dataPath, "..", "GameData");
#else
        dataRootPath = Path.Combine(Application.dataPath, "..", "GameData");
#endif

        CreateDirectory(GetCustomKahootsPath());
        CreateDirectory(GetLeaderboardsPath());
        CreateDirectory(GetErrorReportsPath());
        CopyDefaultKahoots();
    }

    private void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private void CopyDefaultKahoots()
    {
        string destPath = GetCustomKahootsPath();
        TextAsset[] defaultKahoots = Resources.LoadAll<TextAsset>("DefaultKahoots");
        
        foreach (TextAsset kahoot in defaultKahoots)
        {
            string fileName = kahoot.name + ".json";
            string filePath = Path.Combine(destPath, fileName);
            
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, kahoot.text);
            }
        }
    }

    public static string GetCustomKahootsPath()
    {
        if (string.IsNullOrEmpty(dataRootPath))
        {
            Instance.InitializeDataPath();
        }
        return Path.Combine(dataRootPath, "CustomKahoots");
    }

    public static string GetLeaderboardsPath()
    {
        if (string.IsNullOrEmpty(dataRootPath))
        {
            Instance.InitializeDataPath();
        }
        return Path.Combine(dataRootPath, "Leaderboards");
    }

    public static string GetErrorReportsPath()
    {
        if (string.IsNullOrEmpty(dataRootPath))
        {
            Instance.InitializeDataPath();
        }
        return Path.Combine(dataRootPath, "ErrorReports");
    }

    public static string GetGlobalLeaderboardPath()
    {
        if (string.IsNullOrEmpty(dataRootPath))
        {
            Instance.InitializeDataPath();
        }
        return Path.Combine(dataRootPath, "leaderboard.json");
    }

    public static string GetReportsPath()
    {
        if (string.IsNullOrEmpty(dataRootPath))
        {
            Instance.InitializeDataPath();
        }
        return Path.Combine(dataRootPath, "reports.json");
    }

    public static string GetDataRootPath()
    {
        if (string.IsNullOrEmpty(dataRootPath))
        {
            Instance.InitializeDataPath();
        }
        return dataRootPath;
    }

    public static void OpenDataFolder()
    {
        string path = GetDataRootPath();
        if (Directory.Exists(path))
        {
            Application.OpenURL("file://" + path);
        }
    }

    public static bool FileExists(string relativePath)
    {
        string fullPath = Path.Combine(GetDataRootPath(), relativePath);
        return File.Exists(fullPath);
    }

    public static string ReadFile(string relativePath)
    {
        string fullPath = Path.Combine(GetDataRootPath(), relativePath);
        if (File.Exists(fullPath))
        {
            return File.ReadAllText(fullPath);
        }
        return null;
    }

    public static void WriteFile(string relativePath, string content)
    {
        string fullPath = Path.Combine(GetDataRootPath(), relativePath);
        string directory = Path.GetDirectoryName(fullPath);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(fullPath, content);
    }
}

