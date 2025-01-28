using UnityEditor;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class ItemDataDTO
    {
        public int id;
        public string itemName;
        public int type; // Serialized as int
        public int equipmentType; // Serialized as int
        public int maxStack;
        public float weightPerUnit;
        public string itemImagePath;
        public string description;
        public int damageModifier;
        public int defenseModifier;
        public int healValue;

        // Constructor to create DTO from ItemDataSO
        public ItemDataDTO(ItemDataSO itemData)
        {
            id = itemData.Id;
            itemName = itemData.itemName;
            type = (int)itemData.type; // Convert enum to int
            equipmentType = (int)itemData.equipmentType; // Convert enum to int
            maxStack = itemData.maxStack;
            weightPerUnit = itemData.weightPerUnit;
            itemImagePath = itemData.itemImage ? AssetDatabase.GetAssetPath(itemData.itemImage) : null;
            description = itemData.description;
            damageModifier = itemData.damageModifier;
            defenseModifier = itemData.defenseModifier;
            healValue = itemData.healValue;
        }

        // Method to convert DTO back to ItemDataSO
        public ItemDataSO ToItemDataSO()
        {
            ItemDataSO itemData = ScriptableObject.CreateInstance<ItemDataSO>();
            itemData.itemName = itemName;
            itemData.type = (ItemType)type; // Convert int back to enum
            itemData.equipmentType = (EquipmentType)equipmentType; // Convert int back to enum
            itemData.maxStack = maxStack;
            itemData.weightPerUnit = weightPerUnit;
            itemData.itemImage = AssetDatabase.LoadAssetAtPath<Sprite>(itemImagePath);
            itemData.description = description;
            itemData.damageModifier = damageModifier;
            itemData.defenseModifier = defenseModifier;
            itemData.healValue = healValue;
            return itemData;
        }
    }
}