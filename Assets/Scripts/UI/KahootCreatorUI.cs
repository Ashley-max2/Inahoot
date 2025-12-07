using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KahootCreatorUI : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TMP_InputField kahootNameInput;
    [SerializeField] private TMP_InputField kahootThemeInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button loadButton;
    
    [Header("Question Editor Panel")]
    [SerializeField] private GameObject editorPanel;
    [SerializeField] private TMP_InputField questionInput;
    [SerializeField] private TMP_InputField[] answerInputs; // 4 respuestas
    [SerializeField] private Toggle[] correctAnswerToggles; // 4 toggles
    [SerializeField] private ToggleGroup toggleGroup; // Grupo para los toggles
    [SerializeField] private TMP_InputField categoryInput;
    [SerializeField] private TMP_InputField timeLimitInput;
    [SerializeField] private Button addQuestionButton;
    [SerializeField] private Button saveKahootButton;
    [SerializeField] private Button backButton;
    
    [Header("Questions List")]
    [SerializeField] private GameObject questionsListPanel;
    [SerializeField] private Transform questionsListContent;
    [SerializeField] private GameObject questionItemPrefab;
    
    [Header("Load Kahoot Panel")]
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private Transform loadListContent;
    [SerializeField] private GameObject loadItemPrefab;
    
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI questionCountText;

    private bool isEditingQuestion = false;
    private int editingQuestionIndex = -1;

    void Start()
    {
        SetupButtons();
        ShowMainPanel();
    }

    private void SetupButtons()
    {
        if (createButton != null)
            createButton.onClick.AddListener(OnCreateNewKahoot);
        
        if (loadButton != null)
            loadButton.onClick.AddListener(OnShowLoadPanel);
        
        if (addQuestionButton != null)
            addQuestionButton.onClick.AddListener(OnAddQuestion);
        
        if (saveKahootButton != null)
            saveKahootButton.onClick.AddListener(OnSaveKahoot);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackToMainMenu);

        // Configurar toggles para que solo uno esté activo
        SetupToggleGroup();
    }

    private void SetupToggleGroup()
    {
        if (correctAnswerToggles != null && correctAnswerToggles.Length > 0)
        {
            // Crear ToggleGroup si no existe
            if (toggleGroup == null && editorPanel != null)
            {
                toggleGroup = editorPanel.GetComponentInChildren<ToggleGroup>();
                
                if (toggleGroup == null)
                {
                    // Crear un GameObject para el ToggleGroup
                    GameObject toggleGroupObj = new GameObject("ToggleGroup");
                    toggleGroupObj.transform.SetParent(editorPanel.transform);
                    toggleGroup = toggleGroupObj.AddComponent<ToggleGroup>();
                    
                }
            }
            
            // Asignar todos los toggles al grupo
            if (toggleGroup != null)
            {
                toggleGroup.allowSwitchOff = false; // Al menos uno debe estar activo
                
                foreach (Toggle toggle in correctAnswerToggles)
                {
                    if (toggle != null)
                    {
                        toggle.group = toggleGroup;
                        
                    }
                }
            }
            else
            {
                
                
                // Fallback: configurar manualmente
                for (int i = 0; i < correctAnswerToggles.Length; i++)
                {
                    int index = i;
                    if (correctAnswerToggles[i] != null)
                    {
                        correctAnswerToggles[i].onValueChanged.AddListener((value) => {
                            if (value)
                            {
                                for (int j = 0; j < correctAnswerToggles.Length; j++)
                                {
                                    if (j != index && correctAnswerToggles[j] != null)
                                    {
                                        correctAnswerToggles[j].isOn = false;
                                    }
                                }
                            }
                        });
                    }
                }
            }
        }
    }

    private void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (editorPanel != null) editorPanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);
        if (questionsListPanel != null) questionsListPanel.SetActive(false);
    }

    private void ShowEditorPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (editorPanel != null) editorPanel.SetActive(true);
        if (questionsListPanel != null) questionsListPanel.SetActive(true);
        if (loadPanel != null) loadPanel.SetActive(false);
        
        UpdateQuestionsList();
        UpdateQuestionCount();
    }

    private void OnCreateNewKahoot()
    {
        string name = kahootNameInput?.text ?? "";
        string theme = kahootThemeInput?.text ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowStatus("Por favor, introduce un nombre para el Kahoot", Color.red);
            return;
        }

        if (string.IsNullOrWhiteSpace(theme))
        {
            theme = "General";
        }

        KahootCreatorManager.Instance.CreateNewKahoot(name, theme);
        ShowEditorPanel();
        ShowStatus($"Kahoot '{name}' creado. Añade preguntas.", Color.green);
        ClearQuestionInputs();
    }

    private void OnShowLoadPanel()
    {
        if (loadPanel != null) loadPanel.SetActive(true);
        if (mainPanel != null) mainPanel.SetActive(false);
        
        RefreshLoadList();
    }

    private void RefreshLoadList()
    {
        // Limpiar lista
        if (loadListContent != null)
        {
            foreach (Transform child in loadListContent)
            {
                Destroy(child.gameObject);
            }
        }

        // Obtener Kahoots guardados
        List<string> savedKahoots = KahootCreatorManager.Instance.GetSavedKahoots();

        if (savedKahoots.Count == 0)
        {
            ShowStatus("No hay Kahoots guardados", Color.yellow);
            return;
        }

        // Crear items de lista
        foreach (string kahootFile in savedKahoots)
        {
            GameObject item = Instantiate(loadItemPrefab, loadListContent);
            
            // Obtener información del Kahoot
            var (kahootName, questionCount) = KahootCreatorManager.Instance.GetKahootInfo(kahootFile);
            
            // Buscar componentes por nombre en la jerarquía del prefab
            Transform[] allChildren = item.GetComponentsInChildren<Transform>(true);
            
            // Configurar texto del nombre del Kahoot
            TextMeshProUGUI nameText = null;
            foreach (Transform child in allChildren)
            {
                if (child.name == "KahootNameText" || child.name == "NameText" || child.name == "Text")
                {
                    nameText = child.GetComponent<TextMeshProUGUI>();
                    if (nameText != null)
                    {
                        nameText.text = kahootName;
                        break;
                    }
                }
            }
            
            // Si no encontró por nombre, usar el primer TextMeshProUGUI
            if (nameText == null)
            {
                TextMeshProUGUI[] allTexts = item.GetComponentsInChildren<TextMeshProUGUI>();
                if (allTexts.Length > 0)
                {
                    allTexts[0].text = kahootName;
                }
            }
            
            // Configurar texto del contador de preguntas
            TextMeshProUGUI questionCountTextUI = null;
            foreach (Transform child in allChildren)
            {
                if (child.name == "QuestionCountText" || child.name == "CountText" || child.name == "QuestionsText")
                {
                    questionCountTextUI = child.GetComponent<TextMeshProUGUI>();
                    if (questionCountTextUI != null)
                    {
                        questionCountTextUI.text = $"{questionCount} pregunta{(questionCount != 1 ? "s" : "")}";
                        break;
                    }
                }
            }
            
            // Configurar botón de cargar
            Button loadBtn = null;
            foreach (Transform child in allChildren)
            {
                if (child.name == "LoadButton" || child.name == "SelectButton" || child.name == "Button")
                {
                    loadBtn = child.GetComponent<Button>();
                    if (loadBtn != null)
                    {
                        string fileName = kahootFile; // Captura para closure
                        loadBtn.onClick.RemoveAllListeners();
                        loadBtn.onClick.AddListener(() => OnLoadKahoot(fileName));
                        break;
                    }
                }
            }
            
            // Si no encontró por nombre, usar el primer Button
            if (loadBtn == null)
            {
                Button[] allButtons = item.GetComponentsInChildren<Button>();
                if (allButtons.Length > 0)
                {
                    string fileName = kahootFile;
                    allButtons[0].onClick.RemoveAllListeners();
                    allButtons[0].onClick.AddListener(() => OnLoadKahoot(fileName));
                }
            }
            
            // Configurar botón de eliminar
            Button deleteBtn = null;
            foreach (Transform child in allChildren)
            {
                if (child.name == "DeleteButton" || child.name == "RemoveButton" || child.name == "DeleteBtn")
                {
                    deleteBtn = child.GetComponent<Button>();
                    if (deleteBtn != null)
                    {
                        string fileName = kahootFile; // Captura para closure
                        deleteBtn.onClick.RemoveAllListeners();
                        deleteBtn.onClick.AddListener(() => OnDeleteKahoot(fileName));
                        break;
                    }
                }
            }
            
            // Si hay más de un botón, asumir que el segundo es eliminar
            if (deleteBtn == null)
            {
                Button[] allButtons = item.GetComponentsInChildren<Button>();
                if (allButtons.Length > 1)
                {
                    string fileName = kahootFile;
                    allButtons[1].onClick.RemoveAllListeners();
                    allButtons[1].onClick.AddListener(() => OnDeleteKahoot(fileName));
                }
            }
        }
    }

    private void OnDeleteKahoot(string fileName)
    {
        if (KahootCreatorManager.Instance.DeleteKahoot(fileName))
        {
            ShowStatus($"Kahoot '{Path.GetFileNameWithoutExtension(fileName)}' eliminado", Color.yellow);
            RefreshLoadList(); // Refrescar la lista después de eliminar
        }
        else
        {
            ShowStatus("Error al eliminar el Kahoot", Color.red);
        }
    }

    private void OnLoadKahoot(string fileName)
    {
        if (KahootCreatorManager.Instance.LoadKahoot(fileName))
        {
            ShowEditorPanel();
            ShowStatus($"Kahoot '{fileName}' cargado correctamente", Color.green);
            UpdateQuestionsList();
        }
        else
        {
            ShowStatus("Error al cargar el Kahoot", Color.red);
        }
    }

    private void OnAddQuestion()
    {
        // Validar campos
        string questionText = questionInput?.text ?? "";
        if (string.IsNullOrWhiteSpace(questionText))
        {
            ShowStatus("Por favor, escribe una pregunta", Color.red);
            return;
        }

        // Obtener respuestas
        List<string> answers = new List<string>();
        for (int i = 0; i < answerInputs.Length; i++)
        {
            string answer = answerInputs[i]?.text ?? "";
            if (string.IsNullOrWhiteSpace(answer))
            {
                ShowStatus($"Por favor, completa la respuesta {i + 1}", Color.red);
                return;
            }
            answers.Add(answer);
        }

        // Obtener respuesta correcta
        int correctIndex = -1;
        for (int i = 0; i < correctAnswerToggles.Length; i++)
        {
            if (correctAnswerToggles[i].isOn)
            {
                correctIndex = i;
                break;
            }
        }

        if (correctIndex == -1)
        {
            ShowStatus("Por favor, marca la respuesta correcta", Color.red);
            return;
        }

        // Obtener categoría y tiempo
        string category = categoryInput?.text ?? "General";
        int timeLimit = 20;
        if (timeLimitInput != null && !string.IsNullOrWhiteSpace(timeLimitInput.text))
        {
            int.TryParse(timeLimitInput.text, out timeLimit);
        }

        // Crear pregunta
        Question newQuestion = new Question(
            questionText,
            answers,
            correctIndex,
            category,
            timeLimit
        );

        // Añadir o actualizar pregunta
        if (isEditingQuestion && editingQuestionIndex >= 0)
        {
            KahootCreatorManager.Instance.UpdateQuestion(editingQuestionIndex, newQuestion);
            ShowStatus("Pregunta actualizada", Color.green);
            isEditingQuestion = false;
            editingQuestionIndex = -1;
        }
        else
        {
            KahootCreatorManager.Instance.AddQuestion(newQuestion);
            ShowStatus("Pregunta añadida correctamente", Color.green);
        }

        ClearQuestionInputs();
        UpdateQuestionsList();
        UpdateQuestionCount();
    }

    private void OnSaveKahoot()
    {
        if (KahootCreatorManager.Instance.GetQuestionCount() == 0)
        {
            ShowStatus("Añade al menos una pregunta antes de guardar", Color.red);
            return;
        }

        if (KahootCreatorManager.Instance.SaveKahoot())
        {
            ShowStatus("Kahoot guardado correctamente", Color.green);
        }
        else
        {
            ShowStatus("Error al guardar el Kahoot", Color.red);
        }
    }

    private void OnBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ClearQuestionInputs()
    {
        if (questionInput != null) questionInput.text = "";
        
        foreach (var input in answerInputs)
        {
            if (input != null) input.text = "";
        }
        
        foreach (var toggle in correctAnswerToggles)
        {
            if (toggle != null) toggle.isOn = false;
        }
        
        if (categoryInput != null) categoryInput.text = "";
        if (timeLimitInput != null) timeLimitInput.text = "20";

        isEditingQuestion = false;
        editingQuestionIndex = -1;
    }

    private void UpdateQuestionsList()
    {
        if (questionsListContent == null) return;

        // Limpiar lista
        foreach (Transform child in questionsListContent)
        {
            Destroy(child.gameObject);
        }

        // Obtener preguntas
        int count = KahootCreatorManager.Instance.GetQuestionCount();
        
        for (int i = 0; i < count; i++)
        {
            Question q = KahootCreatorManager.Instance.GetQuestion(i);
            if (q == null) continue;

            GameObject item = Instantiate(questionItemPrefab, questionsListContent);
            
            TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{i + 1}. {q.questionText}";
            }

            Button[] buttons = item.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                int index = i; // Captura para closure
                buttons[0].onClick.AddListener(() => OnEditQuestion(index));
                buttons[1].onClick.AddListener(() => OnDeleteQuestion(index));
            }
        }
    }

    private void OnEditQuestion(int index)
    {
        Question q = KahootCreatorManager.Instance.GetQuestion(index);
        if (q == null) return;

        isEditingQuestion = true;
        editingQuestionIndex = index;

        // Rellenar campos
        if (questionInput != null) questionInput.text = q.questionText;
        
        for (int i = 0; i < answerInputs.Length && i < q.answers.Count; i++)
        {
            if (answerInputs[i] != null)
                answerInputs[i].text = q.answers[i];
        }
        
        for (int i = 0; i < correctAnswerToggles.Length; i++)
        {
            if (correctAnswerToggles[i] != null)
                correctAnswerToggles[i].isOn = (i == q.correctAnswerIndex);
        }
        
        if (categoryInput != null) categoryInput.text = q.category;
        if (timeLimitInput != null) timeLimitInput.text = q.timeLimit.ToString();

        ShowStatus($"Editando pregunta {index + 1}", Color.cyan);
    }

    private void OnDeleteQuestion(int index)
    {
        KahootCreatorManager.Instance.DeleteQuestion(index);
        UpdateQuestionsList();
        UpdateQuestionCount();
        ShowStatus("Pregunta eliminada", Color.yellow);
    }

    private void UpdateQuestionCount()
    {
        if (questionCountText != null)
        {
            int count = KahootCreatorManager.Instance.GetQuestionCount();
            questionCountText.text = $"Preguntas: {count}";
        }
    }

    private void ShowStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
            StartCoroutine(ClearStatusAfterDelay(3f));
        }
        
        
    }

    private IEnumerator ClearStatusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
}

