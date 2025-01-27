using UnityEngine;

namespace Utility
{
    public static class GameObjectUtility
    {
        // Toggle the active state of a GameObject
        public static void ToggleActive(GameObject gameObject)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
            else
            {
                Debug.LogWarning("GameObject is null. Cannot toggle active state.");
            }
        }
    }
}

