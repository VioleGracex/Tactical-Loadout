using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadPopupManager : MonoBehaviour
{
    // Reference to the pop-up GameObject
    [SerializeField] private GameObject popupWindow;

    // Reference to the title text
    [SerializeField] private TextMeshProUGUI titleText;

    // Reference to the toggle group
    [SerializeField] private ToggleGroup toggleGroup;

    // References to the Save and Load toggles
    [SerializeField] private Toggle saveToggle;
    [SerializeField] private Toggle loadToggle;

    // Reference to the SaveSlot scripts (assuming there are 3 slots)
    [SerializeField] private SaveSlot[] saveSlots;

    // Internal state to track if the pop-up is in save or load mode
    private bool _isSaving = false;

    private void Start()
    {
        // Ensure the pop-up is closed at the start
        ClosePopup();
    }

    // Function to open the pop-up in Save mode
    public void OpenSavePopup()
    {
        OpenPopup(true); // Open in Save mode
    }

    // Function to open the pop-up in Load mode
    public void OpenLoadPopup()
    {
        OpenPopup(false); // Open in Load mode
    }

    // Function to open the pop-up and set the mode
    private void OpenPopup(bool isSaving)
    {
        _isSaving = isSaving;

        // Update the title text based on the mode
        titleText.text = _isSaving ? "Save Game" : "Load Game";

        // Set the appropriate toggle based on the mode
        if (_isSaving)
        {
            saveToggle.isOn = true;
        }
        else
        {
            loadToggle.isOn = true;
        }

        // Update the mode for all save slots
        foreach (var saveSlot in saveSlots)
        {
            saveSlot.SetMode(_isSaving);
        }

        // Activate the pop-up window
        popupWindow.SetActive(true);
    }

    // Function to close the pop-up
    public void ClosePopup()
    {
        popupWindow.SetActive(false);
    }

    // Function to handle toggle changes (optional, if you want to dynamically update the mode)
    public void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // Update the mode based on the selected toggle
            _isSaving = saveToggle.isOn;

            // Update the title text
            titleText.text = _isSaving ? "Save Game" : "Load Game";

            // Update the mode for all save slots
            foreach (var saveSlot in saveSlots)
            {
                saveSlot.SetMode(_isSaving);
            }
        }
    }
}