using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory;
using Data;

namespace Managers
{
    public class LootManager : MonoBehaviour
    {
        [Header("Loot UI Elements")]
        public GameObject lootPopup;
        public Transform lootContent;
        public GameObject lootItemPrefab;
        public Button lootAllButton;
        public Button lootSelectedButton;

        private InventoryManager inventoryManager;
        private List<ItemDataSO> generatedLoot = new List<ItemDataSO>();
        private List<ItemDataSO> selectedLoot = new List<ItemDataSO>();

        void Start()
        {
            lootAllButton.onClick.AddListener(() => EndLoot(true));
            lootSelectedButton.onClick.AddListener(() => EndLoot(false));

            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }

        public void GenerateLoot(int enemyLevel)
        {
            foreach (Transform child in lootContent)
            {
                Destroy(child.gameObject);
            }
            generatedLoot.Clear();
            selectedLoot.Clear();

            int lootCount = Random.Range(1, 4);
            for (int i = 0; i < lootCount; i++)
            {
                ItemDataSO lootItem = GenerateRandomLootItem(enemyLevel);
                generatedLoot.Add(lootItem);

                GameObject lootItemObj = Instantiate(lootItemPrefab, lootContent);
                lootItemObj.GetComponentInChildren<TextMeshProUGUI>().text = lootItem.itemName;
                lootItemObj.GetComponentInChildren<Image>().sprite = lootItem.itemImage;
                Toggle lootToggle = lootItemObj.GetComponent<Toggle>();
                lootToggle.isOn = false;
                lootToggle.onValueChanged.AddListener((isOn) => OnLootItemSelected(isOn, lootItem));
            }

            lootPopup.SetActive(true);
        }

        ItemDataSO GenerateRandomLootItem(int enemyLevel)
        {
            List<ItemDataSO> possibleLoot = new List<ItemDataSO>();

            if (enemyLevel <= 3)
            {
                possibleLoot.Add(CreateItem("Small Health Potion", ItemType.Consumable, EquipmentType.None, 1, 0.5f, "Restores 20 HP", 0, 0, 20));
                possibleLoot.Add(CreateItem("Basic Ammo", ItemType.Ammo, EquipmentType.None, 50, 0.01f, "Basic ammo for pistols", 0, 0, 0));
            }
            else if (enemyLevel <= 5)
            {
                possibleLoot.Add(CreateItem("Medium Health Potion", ItemType.Consumable, EquipmentType.None, 1, 0.5f, "Restores 50 HP", 0, 0, 50));
                possibleLoot.Add(CreateItem("Advanced Ammo", ItemType.Ammo, EquipmentType.None, 100, 0.03f, "Advanced ammo for rifles", 0, 0, 0));
            }
            else
            {
                possibleLoot.Add(CreateItem("Large Health Potion", ItemType.Consumable, EquipmentType.None, 1, 1.0f, "Restores 100 HP", 0, 0, 100));
                possibleLoot.Add(CreateItem("High-Caliber Ammo", ItemType.Ammo, EquipmentType.None, 100, 0.05f, "High-caliber ammo for advanced weapons", 0, 0, 0));
            }

            int randomIndex = Random.Range(0, possibleLoot.Count);
            return possibleLoot[randomIndex];
        }

        ItemDataSO CreateItem(string itemName, ItemType type, EquipmentType equipmentType, int maxStack, float weightPerUnit, string description, int damageModifier, int defenseModifier, int healValue)
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

            return newItem;
        }

        void OnLootItemSelected(bool isOn, ItemDataSO lootItem)
        {
            if (isOn)
            {
                selectedLoot.Add(lootItem);
            }
            else
            {
                selectedLoot.Remove(lootItem);
            }
        }

        void EndLoot(bool lootAll)
        {
            if (lootAll)
            {
                foreach (var lootItem in generatedLoot)
                {
                    inventoryManager.AddItem(lootItem, lootItem.maxStack);
                }
            }
            else
            {
                foreach (var lootItem in selectedLoot)
                {
                    inventoryManager.AddItem(lootItem, lootItem.maxStack);
                }
            }

            selectedLoot.Clear();
            lootPopup.SetActive(false);
            SaveInventory();
        }

        void SaveInventory()
        {
            if (inventoryManager != null)
            {
                inventoryManager.SaveInventory();
            }
            else
            {
                Debug.LogError("InventoryManager not found in the scene.");
            }
        }
    }
}