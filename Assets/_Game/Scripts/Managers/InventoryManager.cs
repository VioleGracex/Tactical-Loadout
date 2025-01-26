using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Game;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class InventoryManager : MonoBehaviour
    {
        #region Variables
        [Header("Prefabs")]
        public GameObject slotPrefab;

        [Header("Text Information")]
        public TextMeshProUGUI pistolAmmoText;
        public TextMeshProUGUI rifleAmmoText;
        public TextMeshProUGUI weightText;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemWeightText;
        public TextMeshProUGUI itemDescriptionText;

        [Header("Stat Information")]
        [SerializeField] private StatInfo damageInfo;
        [SerializeField] private StatInfo defenseInfo;
        [SerializeField] private StatInfo healInfo;
        [SerializeField] private StatInfo weightInfo;

        [Header("Item Information")]
        public GameObject itemPopup;
        public Image itemImage;
        public Button actionButton;
        public Button throwButton;

        [Header("Other")]
        public Transform inventoryParent;
        public GridLayoutGroup gridLayoutGroup;
        public Button sortButton;

        [Header("Managers")]
        [SerializeField] private GameManager gameManager;

        [Header("Inventory Data")]
        private List<Slot> slots = new List<Slot>();
        private Dictionary<int, ItemDataSO> itemDictionary = new Dictionary<int, ItemDataSO>();
        private Dictionary<string, List<Slot>> itemSlotDictionary = new Dictionary<string, List<Slot>>();
        private string savePath;
        public int pistolAmmoCount = 0;
        public int rifleAmmoCount = 0;
        private float totalWeight = 0;

        #endregion

        #region Unity Methods
        private void Start()
        {
            savePath = Path.Combine(Application.persistentDataPath, "inventory.json");
            sortButton.onClick.AddListener(SortInventory);
            InitializeInventory();
            itemPopup.SetActive(false);
            UpdateAmmoAndWeight();

        }
        #endregion

        #region Inventory Initialization and Loading
        private void InitializeInventory()
        {
            foreach (Transform child in inventoryParent)
            {
                Destroy(child.gameObject);
            }
            slots.Clear();
            itemDictionary.Clear();
            itemSlotDictionary.Clear();

            for (int i = 0; i < 30; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, inventoryParent);
                Slot slot = newSlot.GetComponent<Slot>();
                slot.id = i;
                slot.OnSlotClicked += ShowItemPopup;
                slot.OnItemSwapped += HandleItemSwapped;
                slots.Add(slot);
            }
        }

        public void SaveInventory()
        {
            // Implement Save functionality if needed
        }
        #endregion

        #region Inventory Management
        private void SortInventory()
        {
            slots = slots.OrderBy(slot => slot.GetItem()?.itemName).ToList();

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.SetSiblingIndex(i);
            }

            SaveInventory();
        }

        private void ShowItemPopup(ItemDataSO item, Slot slot)
        {
            if (item == null) return;

            itemNameText.text = item.itemName;
            itemImage.sprite = item.itemImage;
            itemDescriptionText.text = item.description;

            // Hide all stat texts initially
            damageInfo.Hide();
            defenseInfo.Hide();
            weightInfo.Hide();
            healInfo.Hide();

            // Update and show stat texts based on item properties
            if (item.type == ItemType.Consumable)
            {
                if (item.healValue > 0)
                {
                    healInfo.SetStat(item.healValue.ToString());
                }
            }
            else if (item.type == ItemType.Equipment || item.type == ItemType.Pistol || item.type == ItemType.Rifle)
            {
                if (item.damageModifier > 0)
                {
                    damageInfo.SetStat(item.damageModifier.ToString());
                }
                if (item.defenseModifier > 0)
                {
                    defenseInfo.SetStat(item.defenseModifier.ToString());
                }
                
            }
            float totalWeight = item.weightPerUnit;
            if (slot.GetCurrentAmount() > 1)
            {
                totalWeight *= slot.GetCurrentAmount();
            }
            weightInfo.SetStat(totalWeight.ToString("F2"));

            actionButton.onClick.RemoveAllListeners();
            throwButton.onClick.RemoveAllListeners();

            switch (item.type)
            {
                case ItemType.Ammo:
                    actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
                    actionButton.onClick.AddListener(() => BuyItem(item, 100));
                    break;
                case ItemType.Consumable:
                    actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Heal";
                    actionButton.onClick.AddListener(() => ConsumeHealItem(item, slot, 1));
                    break;
                case ItemType.Equipment:
                case ItemType.Pistol:
                case ItemType.Rifle:
                    actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                    actionButton.onClick.AddListener(() => Equip(item, slot));
                    break;
            }

            throwButton.onClick.AddListener(() => ThrowItem(slot));

            itemPopup.SetActive(true);
        }

        private void BuyItem(ItemDataSO item, int amount)
        {
            AddItem(item, amount);
            itemPopup.SetActive(false);
        }
        private void ConsumeHealItem(ItemDataSO item, Slot slot, int amount)
        {
            gameManager.Heal(item.healValue);
            ConsumeItem(item, slot, amount);
        }
        private void ConsumeItem(ItemDataSO item, Slot slot, int amount)
        {
            DeductItem(item, amount);
            itemPopup.SetActive(false);
        }

        private void Equip(ItemDataSO item, Slot slot)
        {
            gameManager.Equip(item, slot);
            slot.SetEquippedText(); // Set the slot text to "E"
            UpdateAmmoAndWeight(); // Update inventory stats
            itemPopup.SetActive(false);
        }

        private void ThrowItem(Slot slot)
        {
            RemoveItem(slot.id);
            itemPopup.SetActive(false);
        }

        public void AddItem(ItemDataSO item, int amount)
        {
            if (!itemSlotDictionary.ContainsKey(item.itemName))
            {
                itemSlotDictionary[item.itemName] = new List<Slot>();
            }

            foreach (Slot s in itemSlotDictionary[item.itemName])
            {
                if (s.GetCurrentAmount() < item.maxStack)
                {
                    int addedAmount = Mathf.Min(amount, item.maxStack - s.GetCurrentAmount());
                    s.SetCurrentAmount(s.GetCurrentAmount() + addedAmount);
                    amount -= addedAmount;
                    if (amount <= 0)
                        break;
                }
            }

            while (amount > 0)
            {
                Slot emptySlot = slots.FirstOrDefault(slot => slot.IsEmpty());
                if (emptySlot != null)
                {
                    int addedAmount = Mathf.Min(amount, item.maxStack);
                    emptySlot.SetItem(item, addedAmount);
                    itemSlotDictionary[item.itemName].Add(emptySlot);
                    amount -= addedAmount;
                }
                else
                {
                    Debug.LogWarning("No empty slot available to add the item.");
                    break;
                }
            }

            SaveInventory();
            UpdateAmmoAndWeight();
        }

        public void RemoveItem(int slotId)
        {
            Slot slot = slots.FirstOrDefault(s => s.id == slotId);
            if (slot != null)
            {
                ItemDataSO item = slot.GetItem();
                if (item != null)
                {
                    itemDictionary.Remove(item.Id);
                    itemSlotDictionary[item.itemName].Remove(slot);
                }
                slot.SetItem(null, 0);
                SaveInventory();
                UpdateAmmoAndWeight();
            }
            else
            {
                Debug.LogWarning("Slot with ID " + slotId + " not found.");
            }
        }

        public void ConsumeItemByName(string itemName, int amount)
        {
            DeductItem(itemName, amount);
        }

        private void DeductItem(ItemDataSO item, int amount)
        {
            if (!itemSlotDictionary.ContainsKey(item.itemName)) return;
            foreach (Slot s in itemSlotDictionary[item.itemName])
            {
                int currentAmount = s.GetCurrentAmount();
                if (currentAmount > 0)
                {
                    if (currentAmount > amount)
                    {
                        s.DeductAmount(amount);
                        break;
                    }
                    else
                    {
                        s.DeductAmount(currentAmount);
                        amount -= currentAmount;
                    }
                }
            }

            UpdateAmmoAndWeight();
        }

        private void DeductItem(string itemName, int amount)
        {
            if (!itemSlotDictionary.ContainsKey(itemName)) return;

            foreach (Slot s in itemSlotDictionary[itemName])
            {
                int currentAmount = s.GetCurrentAmount();
                if (currentAmount > 0)
                {
                    if (currentAmount >= amount)
                    {
                        s.DeductAmount(amount);
                        break;
                    }
                    else
                    {
                        s.DeductAmount(currentAmount);
                        amount -= currentAmount;
                    }
                }
            }
            UpdateAmmoAndWeight();
        }

        private void HandleItemSwapped(Slot originalSlot, Slot newSlot) //**
        {
            ItemDataSO originalItem = originalSlot.GetItem();
            ItemDataSO newItem = newSlot.GetItem();

            if (originalItem != null)
            {
                itemSlotDictionary[originalItem.itemName].Remove(originalSlot);
                itemSlotDictionary[originalItem.itemName].Add(newSlot);
            }

            if (newItem != null)
            {
                itemSlotDictionary[newItem.itemName].Remove(newSlot);
                itemSlotDictionary[newItem.itemName].Add(originalSlot);
            }
        }
        #endregion

        #region Ammo and Weight Calculation
        public void UpdateAmmoAndWeight()
        {
            pistolAmmoCount = 0;
            rifleAmmoCount = 0;
            totalWeight = 0;

            foreach (var slot in slots)
            {
                ItemDataSO item = slot.GetItem();
                if (item != null)
                {
                    if (item.type == ItemType.Ammo)
                    {
                        if (item.itemName.Contains("9x18mm"))
                        {
                            pistolAmmoCount += slot.GetCurrentAmount();
                        }
                        else if (item.itemName.Contains("5.45х39mm"))
                        {
                            rifleAmmoCount += slot.GetCurrentAmount();
                        }
                    }

                    totalWeight += item.weightPerUnit * slot.GetCurrentAmount();
                }
            }

            UpdateAmmoText();
            UpdateWeightText();
        }

        private void UpdateAmmoText()
        {
            pistolAmmoText.text = $"Pistol Ammo: {pistolAmmoCount}";
            rifleAmmoText.text = $"Rifle Ammo: {rifleAmmoCount}";
        }

        private void UpdateWeightText()
        {
            weightText.text = $"{totalWeight:F2} кг";
        }
        #endregion
    }

    [System.Serializable]
    public class InventoryData
    {
        public List<int> items = new List<int>();
    }
}