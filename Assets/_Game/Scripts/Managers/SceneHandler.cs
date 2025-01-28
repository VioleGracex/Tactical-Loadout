using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    // Reload the current level
    public void ReloadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Debug.Log("Reloaded current level.");
    }

    // Load a specific level by name
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
        Debug.Log($"Loaded level: {levelName}");
    }

    // Load a specific level by build index
    public void LoadLevelByBuildIndex(int buildIndex)
    {
        if (buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(buildIndex);
            Debug.Log($"Loaded level with build index: {buildIndex}");
        }
        else
        {
            Debug.LogError($"Invalid build index: {buildIndex}. Scene not loaded.");
        }
    }


    // Pause the game
    public void PauseGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f; // Freeze time
            isPaused = true;
            Debug.Log("Game paused.");
        }
    }

    // Resume the game
    public void ResumeGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1f; // Unfreeze time
            isPaused = false;
            Debug.Log("Game resumed.");
        }
    }

    // Toggle pause/resume
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exit play mode in the editor
#else
        Application.Quit(); // Quit the application in a build
#endif
        Debug.Log("Game quit.");
    }
}