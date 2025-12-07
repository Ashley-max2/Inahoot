using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script simple para cargar escenas desde botones UI.
/// Añade este script a un GameObject vacío y referéncialo desde botones.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Carga el MainMenu
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        
    }

    /// <summary>
    /// Carga la escena Game
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        
    }

    /// <summary>
    /// Carga la escena Results
    /// </summary>
    public void LoadResults()
    {
        SceneManager.LoadScene("Results");
        
    }

    /// <summary>
    /// Carga la escena Leaderboards
    /// </summary>
    public void LoadLeaderboards()
    {
        SceneManager.LoadScene("Leaderboards");
        
    }

    /// <summary>
    /// Carga la escena Reports
    /// </summary>
    public void LoadReports()
    {
        SceneManager.LoadScene("Reports");
        
    }

    /// <summary>
    /// Carga la escena KahootCreator
    /// </summary>
    public void LoadKahootCreator()
    {
        SceneManager.LoadScene("KahootCreator");
        
    }

    /// <summary>
    /// Carga la escena About
    /// </summary>
    public void LoadAbout()
    {
        SceneManager.LoadScene("About");
        
    }

    /// <summary>
    /// Carga la escena ErrorReports
    /// </summary>
    public void LoadErrorReports()
    {
        SceneManager.LoadScene("ErrorReports");
        
    }

    /// <summary>
    /// Carga cualquier escena por nombre
    /// </summary>
    /// <param name="sceneName">Nombre exacto de la escena</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        
    }

    /// <summary>
    /// Sale del juego
    /// </summary>
    public void QuitGame()
    {
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

