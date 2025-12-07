using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    [Header("Configuración")]
    [SerializeField] private string questionJsonFileName = "InazumaElevenQuestions";
    [SerializeField] private bool shuffleQuestions = true;
    [SerializeField] private int maxQuestionsPerGame = 10;

    private QuestionSet currentQuestionSet;
    private List<Question> gameQuestions;
    private int currentQuestionIndex = 0;
    private string customKahootsPath;
    private string selectedCustomKahoot = ""; // Nombre del Kahoot personalizado seleccionado

    void Awake()
    {
        // Singleton pattern
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
        customKahootsPath = DataPathManager.GetCustomKahootsPath() + "/";
    }

    void Start()
    {
        LoadQuestions();
    }

    /// <summary>
    /// Recarga las preguntas según el Kahoot seleccionado.
    /// Debe llamarse antes de iniciar cada juego.
    /// </summary>
    public void ReloadQuestionsForGame()
    {
        
        LoadQuestions();
    }

    public void LoadQuestions()
    {
        // Si hay un Kahoot personalizado seleccionado, cargarlo
        if (!string.IsNullOrEmpty(selectedCustomKahoot))
        {
            LoadCustomKahoot(selectedCustomKahoot);
            return;
        }
        
        // Cargar el Kahoot por defecto desde Resources
        try
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("DefaultKahoots/" + questionJsonFileName);
            
            if (jsonFile != null)
            {
                currentQuestionSet = JsonUtility.FromJson<QuestionSet>(jsonFile.text);
                
                if (currentQuestionSet == null || currentQuestionSet.questions == null)
                {
                    throw new System.Exception("El JSON está vacío o mal formateado");
                }
                
                
                PrepareGameQuestions();
            }
            else
            {
                
                if (ErrorReportManager.Instance != null)
                {
                    ErrorReportManager.Instance.ReportJSONError(
                        $"Resources/DefaultKahoots/{questionJsonFileName}",
                        "Archivo JSON no encontrado en Resources"
                    );
                }
                CreateDefaultQuestions();
            }
        }
        catch (System.Exception ex)
        {
            
            if (ErrorReportManager.Instance != null)
            {
                ErrorReportManager.Instance.ReportJSONError(
                    $"DefaultKahoots/{questionJsonFileName}",
                    "Error al parsear el archivo JSON. El formato puede estar corrupto.",
                    ex
                );
            }
            CreateDefaultQuestions();
        }
    }
    
    public bool LoadCustomKahoot(string kahootName)
    {
        try
        {
            string fileName = kahootName;
            if (!fileName.EndsWith(".json"))
            {
                fileName += ".json";
            }
            
            string fullPath = Path.Combine(customKahootsPath, fileName);
            
            if (!File.Exists(fullPath))
            {
                
                if (ErrorReportManager.Instance != null)
                {
                    ErrorReportManager.Instance.ReportJSONError(
                        fullPath,
                        "Archivo de Kahoot personalizado no encontrado"
                    );
                }
                return false;
            }

            string json = File.ReadAllText(fullPath);
            currentQuestionSet = JsonUtility.FromJson<QuestionSet>(json);
            
            if (currentQuestionSet != null && currentQuestionSet.questions != null)
            {
                selectedCustomKahoot = kahootName;
                
                PrepareGameQuestions();
                return true;
            }
            else
            {
                
                if (ErrorReportManager.Instance != null)
                {
                    ErrorReportManager.Instance.ReportJSONError(
                        fullPath,
                        "El archivo JSON del Kahoot está mal formateado o vacío"
                    );
                }
                return false;
            }
        }
        catch (System.Exception ex)
        {
            
            if (ErrorReportManager.Instance != null)
            {
                ErrorReportManager.Instance.ReportJSONError(
                    $"CustomKahoots/{kahootName}",
                    $"Excepción al leer o parsear el Kahoot: {ex.Message}",
                    ex
                );
            }
            return false;
        }
    }
    
    public List<string> GetAvailableCustomKahoots()
    {
        List<string> kahoots = new List<string>();
        
        if (!Directory.Exists(customKahootsPath))
        {
            return kahoots;
        }
        
        string[] files = Directory.GetFiles(customKahootsPath, "*.json");
        
        foreach (string file in files)
        {
            kahoots.Add(Path.GetFileNameWithoutExtension(file));
        }
        
        
        return kahoots;
    }
    
    public void SetSelectedKahoot(string kahootName)
    {
        selectedCustomKahoot = kahootName;
        
        // No cargar inmediatamente, se cargará cuando se inicie el juego
    }
    
    public void ClearSelectedKahoot()
    {
        selectedCustomKahoot = "";
        
    }
    
    public string GetCurrentKahootName()
    {
        if (currentQuestionSet != null)
        {
            return currentQuestionSet.setName;
        }
        return "Sin nombre";
    }

    private void CreateDefaultQuestions()
    {
        currentQuestionSet = new QuestionSet();
        currentQuestionSet.setName = "Preguntas por defecto";
        currentQuestionSet.theme = "Inazuma Eleven";
        
        // Añadir algunas preguntas por defecto
        currentQuestionSet.questions.Add(new Question(
            "¿Cuál es el nombre del protagonista?",
            new List<string> { "Axel Blaze", "Mark Evans", "Nathan Swift", "Jude Sharp" },
            1,
            "Personajes",
            20
        ));
    }

    private void PrepareGameQuestions()
    {
        gameQuestions = new List<Question>(currentQuestionSet.questions);

        if (shuffleQuestions)
        {
            gameQuestions = gameQuestions.OrderBy(x => Random.value).ToList();
        }

        // Limitar el número de preguntas si es necesario
        if (gameQuestions.Count > maxQuestionsPerGame)
        {
            gameQuestions = gameQuestions.Take(maxQuestionsPerGame).ToList();
        }

        currentQuestionIndex = 0;
    }

    public Question GetCurrentQuestion()
    {
        if (currentQuestionIndex < gameQuestions.Count)
        {
            return gameQuestions[currentQuestionIndex];
        }
        return null;
    }

    public Question GetNextQuestion()
    {
        currentQuestionIndex++;
        return GetCurrentQuestion();
    }

    public bool HasMoreQuestions()
    {
        return currentQuestionIndex < gameQuestions.Count;
    }

    public int GetCurrentQuestionNumber()
    {
        return currentQuestionIndex + 1;
    }

    public int GetTotalQuestions()
    {
        return gameQuestions != null ? gameQuestions.Count : 0;
    }

    public void ResetQuestions()
    {
        currentQuestionIndex = 0;
        PrepareGameQuestions();
    }

    public List<Question> GetQuestionsByCategory(string category)
    {
        return currentQuestionSet.questions.Where(q => q.category == category).ToList();
    }

    public List<string> GetAllCategories()
    {
        return currentQuestionSet.questions.Select(q => q.category).Distinct().ToList();
    }
}

