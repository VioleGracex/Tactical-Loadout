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
/*         [Header("Managers")]
        ResourcesCreator resourcesCreator; */

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