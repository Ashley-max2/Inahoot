using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class KahootCreatorData
{
    public string kahootName;
    public string theme;
    public List<Question> questions = new List<Question>();
}

public class KahootCreatorManager : MonoBehaviour
{
    public static KahootCreatorManager Instance { get; private set; }

    [Header("Current Kahoot Being Edited")]
    private KahootCreatorData currentKahoot;
    private int currentQuestionIndex = -1;

    private string savePath;

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
        savePath = DataPathManager.GetCustomKahootsPath() + "/";
    }

    public void CreateNewKahoot(string name, string theme)
    {
        currentKahoot = new KahootCreatorData
        {
            kahootName = name,
            theme = theme,
            questions = new List<Question>()
        };
        
        currentQuestionIndex = -1;
        
    }

    public void AddQuestion(Question question)
    {
        if (currentKahoot != null)
        {
            currentKahoot.questions.Add(question);
            
        }
    }

    public void UpdateQuestion(int index, Question question)
    {
        if (currentKahoot != null && index >= 0 && index < currentKahoot.questions.Count)
        {
            currentKahoot.questions[index] = question;
            
        }
    }

    public void DeleteQuestion(int index)
    {
        if (currentKahoot != null && index >= 0 && index < currentKahoot.questions.Count)
        {
            currentKahoot.questions.RemoveAt(index);
            
        }
    }

    public bool SaveKahoot()
    {
        if (currentKahoot == null || string.IsNullOrEmpty(currentKahoot.kahootName))
        {
            
            return false;
        }

        if (currentKahoot.questions.Count == 0)
        {
            
            return false;
        }

        try
        {
            // Convertir a QuestionSet para compatibilidad
            QuestionSet questionSet = new QuestionSet
            {
                setName = currentKahoot.kahootName,
                theme = currentKahoot.theme,
                questions = currentKahoot.questions
            };

            string json = JsonUtility.ToJson(questionSet, true);
            string fileName = SanitizeFileName(currentKahoot.kahootName) + ".json";
            string fullPath = Path.Combine(savePath, fileName);

            File.WriteAllText(fullPath, json);
            
            
            return true;
        }
        catch (System.Exception e)
        {
            
            return false;
        }
    }

    public bool LoadKahoot(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(savePath, fileName);
            
            if (!File.Exists(fullPath))
            {
                
                return false;
            }

            string json = File.ReadAllText(fullPath);
            QuestionSet questionSet = JsonUtility.FromJson<QuestionSet>(json);

            currentKahoot = new KahootCreatorData
            {
                kahootName = questionSet.setName,
                theme = questionSet.theme,
                questions = questionSet.questions
            };

            
            return true;
        }
        catch (System.Exception e)
        {
            
            return false;
        }
    }

    public List<string> GetSavedKahoots()
    {
        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
                return new List<string>();
            }

            string[] files = Directory.GetFiles(savePath, "*.json");
            List<string> kahootNames = new List<string>();

            foreach (string file in files)
            {
                kahootNames.Add(Path.GetFileName(file));
            }

            return kahootNames;
        }
        catch (System.Exception e)
        {
            
            return new List<string>();
        }
    }

    public bool DeleteKahoot(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(savePath, fileName);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                
                return true;
            }
            
            return false;
        }
        catch (System.Exception e)
        {
            
            return false;
        }
    }

    /// <summary>
    /// Obtiene información básica de un Kahoot sin cargarlo completamente
    /// </summary>
    public (string name, int questionCount) GetKahootInfo(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(savePath, fileName);
            
            if (!File.Exists(fullPath))
            {
                return (Path.GetFileNameWithoutExtension(fileName), 0);
            }

            string json = File.ReadAllText(fullPath);
            QuestionSet questionSet = JsonUtility.FromJson<QuestionSet>(json);

            return (questionSet.setName, questionSet.questions?.Count ?? 0);
        }
        catch (System.Exception e)
        {
            
            return (Path.GetFileNameWithoutExtension(fileName), 0);
        }
    }

    private string SanitizeFileName(string name)
    {
        string invalid = new string(Path.GetInvalidFileNameChars());
        foreach (char c in invalid)
        {
            name = name.Replace(c.ToString(), "");
        }
        return name;
    }

    public KahootCreatorData GetCurrentKahoot()
    {
        return currentKahoot;
    }

    public int GetQuestionCount()
    {
        return currentKahoot?.questions.Count ?? 0;
    }

    public Question GetQuestion(int index)
    {
        if (currentKahoot != null && index >= 0 && index < currentKahoot.questions.Count)
        {
            return currentKahoot.questions[index];
        }
        return null;
    }

    public void ClearCurrentKahoot()
    {
        currentKahoot = null;
        currentQuestionIndex = -1;
    }

    public string GetSavePath()
    {
        return savePath;
    }
}

