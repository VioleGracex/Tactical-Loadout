using System.Collections;
using Data;
using UnityEngine;
using System.Collections.Generic;

namespace Managers
{
    public class InitialPlayerItems : MonoBehaviour
    {
        public static InitialPlayerItems instance; // Singleton instance

        [SerializeField] Sprite defaultItemImage;

        [SerializeField] InventoryManager inventoryManager;

        private void Awake()
        {
            // Implementing the singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

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
                inventoryManager = FindFirstObjectByType<InventoryManager>();
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
                ResourcesCreator.CreateMedkit(),
                ResourcesCreator.CreateBasicBodyArmor(),
                ResourcesCreator.CreateBasicHelmet(),
                ResourcesCreator.CreatePistolAmmo(),
                ResourcesCreator.CreateRifleAmmo(),
            };
        }

        public static ItemDataSO CreateItem(string itemName, ItemType type, EquipmentType equipmentType, int maxStack, float weightPerUnit, string description, int damageModifier, int defenseModifier, int healValue, string spritePath)
        {
            Sprite itemImage = Resources.Load<Sprite>(spritePath);

            // Create the item and assign its properties
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
            newItem.itemImage = itemImage != null ? itemImage : instance.defaultItemImage;

            return newItem;
        }
    }
}