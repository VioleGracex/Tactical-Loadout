using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class ItemDataSO : ScriptableObject
    {
        private static int nextId = 1;

        [SerializeField]
        private int id;
        public string itemName;
        public ItemType type;
        public EquipmentType equipmentType;
        public int maxStack;
        public float weightPerUnit; // Added weight property
        public Sprite itemImage;
        public string description;
        public int damageModifier;
        public int defenseModifier;
        public int healValue;

        public int Id => id;

        private void OnEnable()
        {
            // Assign a unique ID if this item doesn't already have one
            if (id == 0)
            {
                id = nextId++;
            }
        }
    }

    public enum ItemType
    {
        Ammo,
        Consumable,
        Equipment,
        Pistol,
        Rifle,
    }

    public enum EquipmentType
    {
        None,
        Head,
        Torso
    }
}