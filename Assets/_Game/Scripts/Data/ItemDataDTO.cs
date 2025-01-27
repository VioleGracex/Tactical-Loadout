using UnityEditor;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class ItemDataDTO
    {
        public int id;
        public string itemName;
        public ItemType type;
        public EquipmentType equipmentType;
        public int maxStack;
        public float weightPerUnit; // Added weight property
        public string itemImagePath; // Use path instead of Sprite
        public string description;
        public int damageModifier;
        public int defenseModifier;
        public int healValue;

        // Constructor to create DTO from ItemDataSO
        public ItemDataDTO(ItemDataSO itemData)
        {
            id = itemData.Id;
            itemName = itemData.itemName;
            type = itemData.type;
            equipmentType = itemData.equipmentType;
            maxStack = itemData.maxStack;
            weightPerUnit = itemData.weightPerUnit;
            itemImagePath = itemData.itemImage ? AssetDatabase.GetAssetPath(itemData.itemImage) : null; // Convert Sprite to path
            description = itemData.description;
            damageModifier = itemData.damageModifier;
            defenseModifier = itemData.defenseModifier;
            healValue = itemData.healValue;
        }

        // Method to convert DTO back to ItemDataSO
        public ItemDataSO ToItemDataSO()
        {
            ItemDataSO itemData = ScriptableObject.CreateInstance<ItemDataSO>();
            itemData.name = itemName;
            itemData.type = type;
            itemData.equipmentType = equipmentType;
            itemData.maxStack = maxStack;
            itemData.weightPerUnit = weightPerUnit;
            itemData.itemImage = AssetDatabase.LoadAssetAtPath<Sprite>(itemImagePath); // Load Sprite from path
            itemData.description = description;
            itemData.damageModifier = damageModifier;
            itemData.defenseModifier = defenseModifier;
            itemData.healValue = healValue;
            return itemData;
        }
    }
}