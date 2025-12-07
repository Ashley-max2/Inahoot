using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Referencias UI - Se buscan automáticamente")]
    private TMP_InputField playerNameInput;
    private TMP_Dropdown kahootDropdown;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI infoText;
    private TextMeshProUGUI versionText;

    [Header("Botones - Se buscan automáticamente")]
    private Button playButton;
    private Button creatorButton;
    private Button leaderboardButton;
    private Button reportsButton;
    private Button aboutButton;
    private Button exitButton;

    [Header("Configuración")]
    [SerializeField] private int minNameLength = 2;

    private string playerName = "";
    private List<string> availableKahoots = new List<string>();

    void Start()
    {
        EnsureManagersExist();
        FindUIElements(); // Nuevo: buscar elementos automáticamente
        SetupUI();
        SetupButtons();
    }
    
    private void FindUIElements()
    {
        // Buscar todos los elementos UI automáticamente por nombre
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            switch (obj.name)
            {
                case "PlayerNameInput":
                    playerNameInput = obj.GetComponent<TMP_InputField>();
                    break;
                case "KahootDropdown":
                    kahootDropdown = obj.GetComponent<TMP_Dropdown>();
                    break;
                case "TitleText":
                    titleText = obj.GetComponent<TextMeshProUGUI>();
                    break;
                case "InfoText":
                    infoText = obj.GetComponent<TextMeshProUGUI>();
                    break;
                case "VersionText":
                    versionText = obj.GetComponent<TextMeshProUGUI>();
                    break;
                case "PlayButton":
                    playButton = obj.GetComponent<Button>();
                    break;
                case "CreatorButton":
                    creatorButton = obj.GetComponent<Button>();
                    break;
                case "LeaderboardButton":
                    leaderboardButton = obj.GetComponent<Button>();
                    break;
                case "ReportsButton":
                    reportsButton = obj.GetComponent<Button>();
                    break;
                case "AboutButton":
                    aboutButton = obj.GetComponent<Button>();
                    break;
                case "ExitButton":
                    exitButton = obj.GetComponent<Button>();
                    break;
            }
        }
        
        Debug.Log($"MainMenuManager: UI elements encontrados - " +
                  $"PlayerInput: {playerNameInput != null}, " +
                  $"Dropdown: {kahootDropdown != null}, " +
                  $"PlayButton: {playButton != null}");
    }

    private void EnsureManagersExist()
    {
        // Asegurar que GameManager existe
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            
        }

        // Asegurar que QuestionManager existe
        if (QuestionManager.Instance == null)
        {
            GameObject questionManagerObj = new GameObject("QuestionManager");
            questionManagerObj.AddComponent<QuestionManager>();
            
        }

        // Asegurar que LeaderboardManager existe
        if (LeaderboardManager.Instance == null)
        {
            GameObject leaderboardManagerObj = new GameObject("LeaderboardManager");
            leaderboardManagerObj.AddComponent<LeaderboardManager>();
            
        }

        // Asegurar que ReportsManager existe
        if (ReportsManager.Instance == null)
        {
            GameObject reportsManagerObj = new GameObject("ReportsManager");
            reportsManagerObj.AddComponent<ReportsManager>();
            
        }
    }

    private void SetupUI()
    {
        // Configurar input de nombre
        if (playerNameInput != null)
        {
            playerNameInput.onValueChanged.AddListener(OnNameInputChanged);
        }
        
        // Configurar dropdown de Kahoots
        SetupKahootDropdown();

        // Mostrar versión
        if (versionText != null)
        {
            versionText.text = $"Versión {AboutScreenManager.GetVersion()}";
        }

        // Mostrar información del quiz
        UpdateInfoText();
    }
    
    private void SetupKahootDropdown()
    {
        if (kahootDropdown == null || QuestionManager.Instance == null)
        {
            
            return;
        }
        
        kahootDropdown.ClearOptions();
        availableKahoots.Clear();
        
        List<string> dropdownOptions = new List<string>();
        
        // Opción por defecto
        dropdownOptions.Add("Quiz por defecto (Inazuma Eleven)");
        availableKahoots.Add(""); // String vacío indica usar el por defecto
        
        // Obtener Kahoots personalizados
        List<string> customKahoots = QuestionManager.Instance.GetAvailableCustomKahoots();
        
        foreach (string kahoot in customKahoots)
        {
            dropdownOptions.Add($"{kahoot}");
            availableKahoots.Add(kahoot);
        }
        
        kahootDropdown.AddOptions(dropdownOptions);
        kahootDropdown.onValueChanged.AddListener(OnKahootSelectionChanged);
        
        
    }
    
    private void OnKahootSelectionChanged(int index)
    {
        if (index < 0 || index >= availableKahoots.Count)
        {
            return;
        }
        
        string selectedKahoot = availableKahoots[index];
        
        if (string.IsNullOrEmpty(selectedKahoot))
        {
            // Usar quiz por defecto
            QuestionManager.Instance.ClearSelectedKahoot();
            
        }
        else
        {
            // Usar Kahoot personalizado
            QuestionManager.Instance.SetSelectedKahoot(selectedKahoot);
            
        }
        
        UpdateInfoText();
    }
    
    private void UpdateInfoText()
    {
        if (infoText == null || QuestionManager.Instance == null)
        {
            return;
        }
        
        string kahootName = QuestionManager.Instance.GetCurrentKahootName();
        infoText.text = $"Quiz: {kahootName}\n¡Demuestra tus conocimientos!";
    }

    private void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            playButton.interactable = false; // Deshabilitado hasta que se ingrese nombre
        }

        if (creatorButton != null)
        {
            creatorButton.onClick.AddListener(OnCreatorButtonClicked);
        }

        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.AddListener(OnLeaderboardButtonClicked);
        }

        if (reportsButton != null)
        {
            reportsButton.onClick.AddListener(OnReportsButtonClicked);
        }

        if (aboutButton != null)
        {
            aboutButton.onClick.AddListener(OnAboutButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    private void OnNameInputChanged(string name)
    {
        playerName = name.Trim();

        // Habilitar botón solo si hay nombre
        if (playButton != null)
        {
            playButton.interactable = playerName.Length >= minNameLength;
        }
    }

    private void OnPlayButtonClicked()
    {
        if (string.IsNullOrWhiteSpace(playerName) || playerName.Length < minNameLength)
        {
            
            return;
        }

        // Iniciar nuevo juego
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame(playerName);
        }

        // Iniciar tracking de reportes
        if (ReportsManager.Instance != null)
        {
            ReportsManager.Instance.StartNewGameTracking();
        }

        // Cargar escena de juego
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void OnCreatorButtonClicked()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("KahootCreator");
    }

    private void OnLeaderboardButtonClicked()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Leaderboards");
    }

    private void OnReportsButtonClicked()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Reports");
    }

    private void OnAboutButtonClicked()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("About");
    }

    private void OnExitButtonClicked()
    {
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Método público para cargar una escena desde otros scripts
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

