using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory;
using Data;
using Game;

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
        private List<(ItemDataSO item, int amount)> generatedLoot = new List<(ItemDataSO, int)>();
        private List<(ItemDataSO item, int amount)> selectedLoot = new List<(ItemDataSO, int)>();

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
                int amount = Random.Range(1, lootItem.maxStack + 1);
                generatedLoot.Add((lootItem, amount));

                // Instantiate the loot item prefab
                GameObject lootItemObj = Instantiate(lootItemPrefab, lootContent);

                // Get the LootToggle component and set the item's name, amount, and image
                LootToggle lootToggle = lootItemObj.GetComponent<LootToggle>();
                lootToggle.SetLootItem(lootItem.itemName, amount, lootItem.itemImage);

                // Add a listener to the toggle
                lootToggle.AddToggleListener((isOn) => OnLootItemSelected(isOn, lootItem, amount));
            }

            lootPopup.SetActive(true);
        }

        ItemDataSO GenerateRandomLootItem(int enemyLevel)
        {
            List<ItemDataSO> possibleLoot = new List<ItemDataSO>();

            if (enemyLevel <= 3)
            {
                possibleLoot.Add(ResourcesCreator.CreateMedkit());
                possibleLoot.Add(ResourcesCreator.CreatePistolAmmo());
            }
            else if (enemyLevel <= 5)
            {
                possibleLoot.Add(ResourcesCreator.CreateMediumMedkit());
                possibleLoot.Add(ResourcesCreator.CreateRifleAmmo());
            }
            else
            {
                possibleLoot.Add(ResourcesCreator.CreateAdvancedBodyArmor());
                possibleLoot.Add(ResourcesCreator.CreateAdvancedHelmet());
            }

            int randomIndex = Random.Range(0, possibleLoot.Count);
            return possibleLoot[randomIndex];
        }

        void OnLootItemSelected(bool isOn, ItemDataSO lootItem, int amount)
        {
            if (isOn)
            {
                selectedLoot.Add((lootItem, amount));
            }
            else
            {
                selectedLoot.Remove((lootItem, amount));
            }
        }

        void EndLoot(bool lootAll)
        {
            if (lootAll)
            {
                foreach (var (lootItem, amount) in generatedLoot)
                {
                    // Use the stored amount when looting all items
                    inventoryManager.AddItem(lootItem, amount);
                }
            }
            else
            {
                foreach (var (lootItem, amount) in selectedLoot)
                {
                    // Use the amount stored when the item was selected
                    inventoryManager.AddItem(lootItem, amount);
                }
            }
            selectedLoot.Clear();
            lootPopup.SetActive(false);
            GameManager.Instance.SpawnNextEnemy();
        }
    }
}