using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private string selectedSavePath;

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

    public void SetSelectedSavePath(string savePath)
    {
        selectedSavePath = savePath;
        Debug.Log($"Selected save path: {selectedSavePath}");
    }

    public string GetSelectedSavePath()
    {
        return selectedSavePath;
    }
}