using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AboutScreenManager : MonoBehaviour
{
    [Header("Info Display")]
    [SerializeField] private TextMeshProUGUI gameNameText;
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI disclaimerText;
    
    [Header("Links")]
    [SerializeField] private Button githubButton;
    [SerializeField] private Button licenseButton;
    
    [Header("Navigation")]
    [SerializeField] private Button backButton;

    // Información del juego
    private const string GAME_NAME = "Inahoot - Inazuma Eleven Quiz";
    private const string VERSION = "1.0.0";
    private const string DESCRIPTION = @"Inahoot es un juego de trivia educativo basado en el universo de Inazuma Eleven.

Pon a prueba tus conocimientos sobre:
• Personajes icónicos
• Técnicas especiales
• Equipos y competiciones
• Historia de la serie

¡Compite por el mejor puntaje y demuestra que eres el mayor fan de Inazuma Eleven!";

    private const string CREDITS = @"<b>Desarrollo</b>
Desarrollado con Unity 2022.3.45f1

<b>Diseño y Programación</b>
• GitHub Copilot (AI Assistant)
• Unity Technologies

<b>UI Framework</b>
• TextMesh Pro
• Unity UI System

<b>Assets y Recursos</b>
• Logo: Inazuma Eleven (Level-5)
• Preguntas: Basadas en la serie Inazuma Eleven

<b>Agradecimientos Especiales</b>
• Level-5 Inc. por crear Inazuma Eleven
• Comunidad de fans de Inazuma Eleven";

    private const string DISCLAIMER = @"<b>Aviso Legal</b>

Inazuma Eleven es una marca registrada de Level-5 Inc.

Este proyecto es una aplicación educativa no comercial creada con fines académicos. Todos los derechos de Inazuma Eleven pertenecen a sus respectivos dueños.

Este juego NO está afiliado, respaldado, ni autorizado oficialmente por Level-5 Inc. o cualquier compañía asociada con Inazuma Eleven.

El uso de nombres, imágenes y marcas registradas es solo para fines de identificación y referencia educativa.";

    private const string GITHUB_URL = "https://github.com/AshleyCastellvi"; // Cambiar por tu repo
    private const string LICENSE_URL = "https://opensource.org/licenses/MIT";

    void Start()
    {
        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        if (gameNameText != null)
            gameNameText.text = GAME_NAME;
        
        if (versionText != null)
            versionText.text = $"Versión {VERSION}";
        
        if (descriptionText != null)
            descriptionText.text = DESCRIPTION;
        
        if (creditsText != null)
            creditsText.text = CREDITS;
        
        if (disclaimerText != null)
            disclaimerText.text = DISCLAIMER;
    }

    private void SetupButtons()
    {
        if (githubButton != null)
            githubButton.onClick.AddListener(OnGithubClicked);
        
        if (licenseButton != null)
            licenseButton.onClick.AddListener(OnLicenseClicked);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnGithubClicked()
    {
        Application.OpenURL(GITHUB_URL);
        
    }

    private void OnLicenseClicked()
    {
        Application.OpenURL(LICENSE_URL);
        
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Métodos públicos para obtener información
    public static string GetGameName()
    {
        return GAME_NAME;
    }

    public static string GetVersion()
    {
        return VERSION;
    }

    public static string GetDescription()
    {
        return DESCRIPTION;
    }

    public static string GetFullInfo()
    {
        return $"{GAME_NAME}\n{VERSION}\n\n{DESCRIPTION}\n\n{CREDITS}\n\n{DISCLAIMER}";
    }
}

