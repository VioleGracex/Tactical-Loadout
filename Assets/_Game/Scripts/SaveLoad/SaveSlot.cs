using UnityEngine;
using UnityEngine.UI;
using System;
using SaveSystem;
using TMPro;
using Game;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSlot : MonoBehaviour
{
    // Reference to the UI Text component to display save time
    [SerializeField] private int slotId = 0;
    public TextMeshProUGUI saveTimeText;

    public Button saveSlotButton;

    // Reference to the SaveData assigned to this slot
    private SaveData _saveData;

    // Flag to determine if the button is used for saving or loading
    [SerializeField]private bool _isSaving = false; // Default to saving mode

    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        // Add listener to the button
        saveSlotButton.onClick.AddListener(OnSaveSlotButtonClicked);
        SetSaveData();
    }

    // Function to set the save data and update the UI
    public void SetSaveData()
    {
        
        string savePath = Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.json");
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            _saveData = JsonUtility.FromJson<SaveData>(json);
            UpdateSaveSlotUI();
            Debug.Log($"slot info loaded {slotId} at {savePath}");
        }
        else
        {
            saveTimeText.text = "No save";
            Debug.Log($"No save file found for Slot {slotId}.");
        }       
    }

    // Function to update the UI based on the current SaveData
    private void UpdateSaveSlotUI()
    {
        if (_saveData != null && !string.IsNullOrEmpty(_saveData.saveTime))
        {
            // Parse the save time string into a DateTime object
            if (DateTime.TryParse(_saveData.saveTime, out DateTime saveDateTime))
            {
                // Format the DateTime as "Time / Day / Month"
                string formattedTime = saveDateTime.ToString("HH:mm / dd / MMMM");
                saveTimeText.text = formattedTime; // Update the UI text
            }
            else
            {
                Debug.LogWarning("Failed to parse save time.");
                saveTimeText.text = "Invalid Save Time";
            }
        }
        else
        {
            // If there's no save data, display a default message
            saveTimeText.text = "Empty Slot";
        }
    }

    // Function to clear the save slot
    public void ClearSlot()
    {
        _saveData = null;
        saveTimeText.text = "Empty Slot";
    }

    // Function to set the mode (saving or loading)
    public void SetMode(bool isSaving)
    {
        _isSaving = isSaving;
    }

    // Function to handle button click
    private void OnSaveSlotButtonClicked()
    {
        DataCarrier.Instance.CurrentSaveSlotId = slotId;
        if (_isSaving)
        {
            // Handle saving logic
            SaveGame();
            SetSaveData();
        }
        else
        {
            // Handle loading logic
            LoadGame();
        }  
    }

    // Function to handle saving the game
    private void SaveGame()
    {
        gameManager.SaveGame(slotId);
    }

    // Function to handle loading the game
    private void LoadGame()
    {
        if (_saveData == null)
            Debug.Log("No save data to load starting newgame.");
            
        SceneManager.LoadScene("GameScene01");
    }
}