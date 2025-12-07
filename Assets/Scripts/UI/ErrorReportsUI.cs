using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ErrorReportsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform errorListContainer;
    [SerializeField] private GameObject errorItemPrefab;
    [SerializeField] private TMP_Text errorDetailText;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button clearAllButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject noErrorsPanel;
    [SerializeField] private TMP_Text errorCountText;

    private List<string> errorReports;
    private string selectedReport;

    void Start()
    {
        SetupButtons();
        LoadErrorReports();
    }

    private void SetupButtons()
    {
        if (refreshButton != null)
        {
            refreshButton.onClick.RemoveAllListeners();
            refreshButton.onClick.AddListener(RefreshReports);
        }

        if (clearAllButton != null)
        {
            clearAllButton.onClick.RemoveAllListeners();
            clearAllButton.onClick.AddListener(OnClearAllButtonClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void LoadErrorReports()
    {
        if (ErrorReportManager.Instance == null)
        {
            
            ShowNoErrors();
            return;
        }

        errorReports = ErrorReportManager.Instance.GetErrorReportsList();

        if (errorReports.Count == 0)
        {
            ShowNoErrors();
        }
        else
        {
            DisplayErrorList();
        }

        UpdateErrorCount();
    }

    private void ShowNoErrors()
    {
        if (noErrorsPanel != null)
        {
            noErrorsPanel.SetActive(true);
        }

        if (errorDetailText != null)
        {
            errorDetailText.text = "No hay informes de error.\n\nCuando se produzcan errores en la carga de archivos JSON o XML, " +
                                   "se generarán informes automáticamente que podrás revisar aquí.";
        }
    }

    private void DisplayErrorList()
    {
        if (noErrorsPanel != null)
        {
            noErrorsPanel.SetActive(false);
        }

        // Limpiar lista anterior
        if (errorListContainer != null)
        {
            foreach (Transform child in errorListContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Crear botón para cada informe de error
        foreach (string reportFileName in errorReports)
        {
            GameObject itemObj = Instantiate(errorItemPrefab, errorListContainer);

            // Configurar el botón
            Button itemButton = itemObj.GetComponent<Button>();
            if (itemButton != null)
            {
                string fileName = reportFileName; // Captura local para el lambda
                itemButton.onClick.AddListener(() => OnErrorReportSelected(fileName));
            }

            // Configurar el texto
            TMP_Text itemText = itemObj.GetComponentInChildren<TMP_Text>();
            if (itemText != null)
            {
                // Mostrar nombre sin extensión y formatear fecha
                string displayName = reportFileName.Replace("ErrorReport_", "").Replace(".txt", "");
                itemText.text = FormatReportName(displayName);
            }

            // Añadir botón de eliminar individual
            Button deleteButton = FindDeleteButtonInItem(itemObj);
            if (deleteButton != null)
            {
                string fileName = reportFileName; // Captura local para el lambda
                deleteButton.onClick.AddListener(() => OnDeleteReport(fileName));
            }
        }

        // Seleccionar el primer informe automáticamente
        if (errorReports.Count > 0)
        {
            OnErrorReportSelected(errorReports[0]);
        }
    }

    private Button FindDeleteButtonInItem(GameObject item)
    {
        // Buscar botón "Delete" o "X" dentro del item
        Button[] buttons = item.GetComponentsInChildren<Button>();
        if (buttons.Length > 1)
        {
            return buttons[1]; // El segundo botón es el de eliminar
        }
        return null;
    }

    private string FormatReportName(string rawName)
    {
        // Convertir "20231215_143025" a "15/12/2023 14:30:25"
        if (rawName.Length >= 15)
        {
            string year = rawName.Substring(0, 4);
            string month = rawName.Substring(4, 2);
            string day = rawName.Substring(6, 2);
            string hour = rawName.Substring(9, 2);
            string minute = rawName.Substring(11, 2);
            string second = rawName.Substring(13, 2);

            return $"{day}/{month}/{year} {hour}:{minute}:{second}";
        }

        return rawName;
    }

    private void OnErrorReportSelected(string fileName)
    {
        selectedReport = fileName;

        if (ErrorReportManager.Instance != null)
        {
            string reportContent = ErrorReportManager.Instance.ReadErrorReport(fileName);

            if (errorDetailText != null)
            {
                errorDetailText.text = reportContent;
            }
        }
    }

    private void OnDeleteReport(string fileName)
    {
        if (ErrorReportManager.Instance != null)
        {
            ErrorReportManager.Instance.DeleteErrorReport(fileName);
            RefreshReports();
        }
    }

    private void RefreshReports()
    {
        LoadErrorReports();
    }

    private void OnClearAllButtonClicked()
    {
        // Confirmar con el usuario (en producción podrías usar un diálogo)
        if (ErrorReportManager.Instance != null)
        {
            ErrorReportManager.Instance.ClearAllReports();
            RefreshReports();
            
        }
    }

    private void UpdateErrorCount()
    {
        if (errorCountText != null)
        {
            int count = errorReports != null ? errorReports.Count : 0;
            errorCountText.text = count == 1 ? "1 informe" : $"{count} informes";
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

