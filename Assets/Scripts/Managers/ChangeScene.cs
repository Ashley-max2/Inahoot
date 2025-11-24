using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class SceneChangerTMP : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración de Escena")]
    [SerializeField] private string sceneName; // Nombre de la escena a cargar
    [SerializeField] private float delay = 0.2f; // Tiempo de espera antes de cambiar

    [Header("Efectos Visuales (Opcional)")]
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color clickColor = Color.gray;

    private TextMeshProUGUI buttonText;
    private Color originalColor;

    void Start()
    {
        // Obtener el componente TextMeshPro
        buttonText = GetComponent<TextMeshProUGUI>();

        if (buttonText != null)
        {
            originalColor = buttonText.color;
        }

        // Verificar que se haya asignado una escena
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("No se ha asignado un nombre de escena en: " + gameObject.name);
        }
    }

    // Método llamado cuando se hace clic en el texto
    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeScene();
    }

    // Método público para cambiar de escena (puede ser llamado desde el Inspector)
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Efecto visual opcional
            if (buttonText != null)
            {
                buttonText.color = clickColor;
            }

            // Cambiar escena después del delay
            Invoke("LoadScene", delay);
        }
        else
        {
            Debug.LogError("Nombre de escena no asignado en: " + gameObject.name);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    // Efectos visuales opcionales al pasar el mouse (solo si hay EventTrigger)
    public void OnPointerEnter()
    {
        if (buttonText != null)
        {
            buttonText.color = hoverColor;
        }
    }

    public void OnPointerExit()
    {
        if (buttonText != null)
        {
            buttonText.color = originalColor;
        }
    }
}