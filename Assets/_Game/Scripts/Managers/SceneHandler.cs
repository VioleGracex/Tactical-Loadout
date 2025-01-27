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

    // Load from a save slot
    public void LoadFromSaveSlot(string saveSlotName)
    {
        // Set the selected save path in the MenuManager
        string savePath = Path.Combine(Application.persistentDataPath, $"{saveSlotName}.json");
        MenuManager.Instance.SetSelectedSavePath(savePath);

        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        Debug.Log($"Loading from save slot: {saveSlotName}");
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
}