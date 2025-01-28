using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void ReloadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Debug.Log("Reloaded current level.");
    }
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
