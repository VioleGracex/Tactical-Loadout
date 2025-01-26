using System.Collections;
using Data;
using UnityEngine;

namespace Managers
{
    public class InitialPlayerItems : MonoBehaviour
    {
        [SerializeField] private Sprite ammo9x18Sprite; // Assign this in the inspector
        [SerializeField] private Sprite ammo545x39Sprite; // Assign this in the inspector
        [SerializeField] private Sprite medkitSprite; // Assign this in the inspector
        [SerializeField] private Sprite jacketSprite; // Assign this in the inspector
        [SerializeField] private Sprite vestSprite; // Assign this in the inspector
        [SerializeField] private Sprite capSprite; // Assign this in the inspector
        [SerializeField] private Sprite helmetSprite; // Assign this in the inspector

        [SerializeField] private InventoryManager inventoryManager;

        private void Start()
        {
            StartCoroutine(InitializeAfterDelay());
        }

        private IEnumerator InitializeAfterDelay()
        {
            // Wait for the end of the frame to ensure other Start methods are called first
            yield return new WaitForEndOfFrame();

            if (inventoryManager == null)
            {
                // Try to find the InventoryManager if not assigned
                inventoryManager = FindFirstObjectByType <InventoryManager>();
            }

            if (inventoryManager == null)
            {
                Debug.LogError("InventoryManager not found in the scene.");
                yield break;
            }

            // Add predefined items to the inventory
            AddInitialItems(inventoryManager);
        }

        private void AddInitialItems(InventoryManager inventoryManager)
        {
            ItemDataSO[] items = GetInitialItems();
            foreach (var item in items)
            {
                inventoryManager.AddItem(item, item.maxStack);
            }
        }

        private ItemDataSO[] GetInitialItems()
        {
            return new ItemDataSO[]
            {
                CreateItem("9x18mm Ammo", ItemType.Ammo, EquipmentType.None, 50, 0.01f, "Pistol ammo", 0, 0, 0, ammo9x18Sprite),
                CreateItem("5.45x39mm Ammo", ItemType.Ammo, EquipmentType.None, 100, 0.03f, "Rifle ammo", 0, 0, 0, ammo545x39Sprite),
                CreateItem("Medkit", ItemType.Consumable, EquipmentType.None, 6, 1.0f, "Restores 50 HP", 0, 0, 50, medkitSprite),
                CreateItem("Jacket", ItemType.Equipment, EquipmentType.Torso, 1, 1.0f, "Provides +3 torso defense", 0, 3, 0, jacketSprite),
                CreateItem("Bulletproof Vest", ItemType.Equipment, EquipmentType.Torso, 1, 10.0f, "Provides +10 torso defense", 0, 10, 0, vestSprite),
                CreateItem("Cap", ItemType.Equipment, EquipmentType.Head, 1, 0.1f, "Provides +3 head defense", 0, 3, 0, capSprite),
                CreateItem("Helmet", ItemType.Equipment, EquipmentType.Head, 1, 1.0f, "Provides +10 head defense", 0, 10, 0, helmetSprite)
            };
        }

        public ItemDataSO CreateItem(string itemName, ItemType type, EquipmentType equipmentType, int maxStack, float weightPerUnit, string description, int damageModifier, int defenseModifier, int healValue, Sprite itemImage)
        {
            ItemDataSO newItem = ScriptableObject.CreateInstance<ItemDataSO>();
            newItem.itemName = itemName;
            newItem.type = type;
            newItem.equipmentType = equipmentType;
            newItem.maxStack = maxStack;
            newItem.weightPerUnit = weightPerUnit;
            newItem.description = description;
            newItem.damageModifier = damageModifier;
            newItem.defenseModifier = defenseModifier;
            newItem.healValue = healValue;
            newItem.itemImage = itemImage;

            return newItem;
        }
    }
}