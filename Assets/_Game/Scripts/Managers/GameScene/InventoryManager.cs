using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Game;
using Inventory;
using SaveSystem;
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
        public TextMeshProUGUI pistolAmmoText, rifleAmmoText, weightText;


        [Header("Other")]
        public Transform inventoryParent;
        public GridLayoutGroup gridLayoutGroup;
        public Button sortButton;

        [Header("Managers")]
        [SerializeField] private SlotManager slotManager;
        [SerializeField] private InitialPlayerItems initialPlayerItems;

        [Header("Inventory Data")]
        public List<Slot> slots = new List<Slot>();
        public Dictionary<int, ItemDataSO> itemDictionary = new Dictionary<int, ItemDataSO>();
        public Dictionary<string, List<Slot>> itemSlotDictionary = new Dictionary<string, List<Slot>>();

        [HideInInspector]
        public int pistolAmmoCount = 0, rifleAmmoCount = 0;
        private float totalWeight = 0;

        #endregion

        #region Unity Methods
        private void Awake()
        {
            slotManager.InitializeSlots(30);
        }

        private void Start()
        {
            sortButton.onClick.AddListener(SortInventory);
        }
        #endregion

        #region Inventory Initialization and Loading
        public void LoadInventory(List<SlotSaveData> slotDataList)
        {
            if (slotDataList == null)
            {
                Debug.LogWarning("No inventory data provided for loading.");

                return;
            }

            // Clear existing inventory
            foreach (var slot in slots)
            {
                slot.SetItem(null, 0); // Clear all slots
            }
            Debug.Log(slotDataList.Count);
            // Load items into slots
            foreach (var slotData in slotDataList)
            {
                //Debug.Log(slotData.item.itemName);
                // Ensure the slotId is within the valid range
                if (slotData.slotId >= 0 && slotData.slotId < slots.Count)
                {
                   
                    // Set the item and amount in the corresponding slot
                    slots[slotData.slotId].SetItem(slotData.item.ToItemDataSO(), slotData.amount);

                }
                else
                {
                    Debug.LogWarning($"Invalid slotId {slotData.slotId}. Skipping this item.");
                }
            }
            Debug.Log("finished loading items to slots updating weight.");
            UpdateAmmoAndWeight(); // Update UI after loading inventory
                                   // Re-equip items using EquipmentManager
                                   //FindFirstObjectByType<EquipmentManager>().LoadEquipment(saveData);

        }
        #endregion

        #region Inventory Management
        public Slot GetSlotByItemId(int itemId)
        {
            return slots.FirstOrDefault(slot => slot.CurrentItem?.id == itemId);
        }
        private void SortInventory()
        {
            slots = slots.OrderBy(slot => slot.CurrentItem?.itemName).ToList();

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.SetSiblingIndex(i);
            }
        }

        public void ConsumeItem(ItemDataSO item, Slot slot, int amount)
        {
            if (slot.CurrentItem == item) // Ensure the slot contains the correct item
            {
                int currentAmount = slot.CurrentAmount;
                if (currentAmount >= amount)
                {
                    slot.DeductAmount(amount); // Deduct from the chosen slot
                }
                else
                {
                    Debug.LogWarning("Not enough items in the selected slot.");
                }
            }
            else
            {
                Debug.LogWarning("The selected slot does not contain the specified item.");
            }

            UpdateAmmoAndWeight(); // Update UI after consuming the item
        }

        public void AddItem(ItemDataSO item, int amount)
        {
            if (!itemSlotDictionary.ContainsKey(item.itemName))
            {
                itemSlotDictionary[item.itemName] = new List<Slot>();
            }

            foreach (Slot s in itemSlotDictionary[item.itemName])
            {
                if (s.CurrentAmount < item.maxStack)
                {
                    int addedAmount = Mathf.Min(amount, item.maxStack - s.CurrentAmount);
                    s.CurrentAmount += addedAmount;
                    amount -= addedAmount;
                    if (amount <= 0)
                        break;
                }
            }

            while (amount > 0)
            {
                Slot emptySlot = slots.FirstOrDefault(slot => slot.CurrentItem == null);
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
            UpdateAmmoAndWeight();
        }

        public void RemoveItem(int slotId)
        {
            Slot slot = slots.FirstOrDefault(s => s.id == slotId);
            if (slot != null)
            {
                ItemDataSO item = slot.CurrentItem;
                if (item != null)
                {
                    itemDictionary.Remove(item.Id);
                    itemSlotDictionary[item.itemName].Remove(slot);
                }
                slot.SetItem(null, 0);
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

        private void DeductItem(string itemName, int amount)
        {
            if (!itemSlotDictionary.ContainsKey(itemName)) return;

            foreach (Slot s in itemSlotDictionary[itemName])
            {
                int currentAmount = s.CurrentAmount;
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
        #endregion

        #region Ammo and Weight Calculation
        public void UpdateAmmoAndWeight()
        {
            pistolAmmoCount = 0;
            rifleAmmoCount = 0;
            totalWeight = 0;

            try
            {
                if (slots == null)
                {
                    Debug.LogError("Slots list is null.");
                    return;
                }

                foreach (var slot in slots)
                {
                    if (slot == null)
                    {
                        Debug.LogWarning("Found a null slot in the slots list.");
                        continue; // Skip if slot is null
                    }

                    ItemDataSO item = slot.CurrentItem;
                    if (item == null)
                    {
                        //Debug.LogWarning($"Slot {slot.id} has a null item.");
                        continue; // Skip if item is null
                    }

                    if (item.type == ItemType.Ammo)
                    {
                        if (item.itemName.Contains("9x18mm"))
                        {
                            pistolAmmoCount += slot.CurrentAmount;
                        }
                        else if (item.itemName.Contains("5.45х39mm"))
                        {
                            rifleAmmoCount += slot.CurrentAmount;
                        }
                    }

                    if (item.weightPerUnit <= 0)
                    {
                        Debug.LogWarning($"Item {item.itemName} has an invalid weightPerUnit: {item.weightPerUnit}");
                    }

                    totalWeight += item.weightPerUnit * slot.CurrentAmount;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating ammo and weight: {ex.Message}");
            }

            UpdateAmmoText();
            UpdateWeightText();
        }
        private void UpdateAmmoText()
        {
            pistolAmmoText.text = $"{pistolAmmoCount}";
            rifleAmmoText.text = $"{rifleAmmoCount}";
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