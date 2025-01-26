using UnityEngine;
using UnityEditor;

namespace Data
{
    [CustomEditor(typeof(ItemDataSO))]
    public class ItemDataEditor : Editor
    {
        private Texture2D customIcon;

        private void OnEnable()
        {
            // Load your custom icon from the Resources folder
            customIcon = Resources.Load<Texture2D>("Icons/ItemIcon");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // Set the custom icon for the ScriptableObject
            if (customIcon != null)
            {
                EditorGUIUtility.SetIconForObject(target, customIcon);
            }
        }
    }
}