using UnityEngine;

public class DataCarrier : MonoBehaviour
{
    // Singleton instance
    private static DataCarrier _instance;

    // Property to access the singleton instance
    public static DataCarrier Instance
    {
        get
        {
            if (_instance == null)
            {
                // Look for an existing instance in the scene
                _instance = FindFirstObjectByType <DataCarrier>();

                // If no instance exists, create a new one
                if (_instance == null)
                {
                    GameObject dataCarrierObject = new GameObject("DataCarrier");
                    _instance = dataCarrierObject.AddComponent<DataCarrier>();
                    DontDestroyOnLoad(dataCarrierObject); // Persist across scenes
                }
            }
            return _instance;
        }
    }

    // Current game session save slot ID
    [SerializeField] private int _currentSaveSlotId = -1; // Default to -1 (no slot selected)

    // Public property to get or set the current save slot ID
    public int CurrentSaveSlotId
    {
        get { return _currentSaveSlotId; }
        set { _currentSaveSlotId = value; }
    }

    // Ensure the instance is not destroyed when loading a new scene
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance exists, destroy this one
            Destroy(gameObject);
        }
    }
}