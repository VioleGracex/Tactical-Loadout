using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class ItemData
    {
        public int id;
        public string itemName;
        public ItemType type;
        public EquipmentType equipmentType;
        public int maxStack;
        public float weightPerUnit;
        public Sprite itemImage;
        public string description;
        public int damageModifier;
        public int defenseModifier;
        public int healValue;
    }
}