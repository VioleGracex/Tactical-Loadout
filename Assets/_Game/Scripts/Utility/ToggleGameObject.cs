using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    // Reference to the GameObject you want to toggle
    public GameObject targetGameObject;

    // This method will be assigned to the button's OnClick event
    public void OnButtonClick()
    {
        Utility.GameObjectUtility.ToggleActive(targetGameObject);
    }
}