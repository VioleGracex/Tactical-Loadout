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
        public static InventoryManager Instance;

        [Header("Prefabs")]
        public GameObject slotPrefab;

        [Header("Text Information"), HideInInspector]
        public TextMeshProUGUI pistolAmmoText, rifleAmmoText, weightText;


        [Header("Other")]
        public Transform inventoryParent;
        public GridLayoutGroup gridLayoutGroup;
        public Button sortButton;

        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private SlotManager slotManager;
        [SerializeField] private InitialPlayerItems initialPlayerItems;

        [Header("Inventory Data")]
        public List<Slot> slots = new List<Slot>();
        public Dictionary<int, ItemDataSO> itemDictionary = new Dictionary<int, ItemDataSO>();
        public Dictionary<string, List<Slot>> itemSlotDictionary = new Dictionary<string, List<Slot>>();
        private string savePath;
        
        [HideInInspector]
        public int pistolAmmoCount = 0, rifleAmmoCount = 0;
        private float totalWeight = 0;

        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            slotManager.InitializeSlots(30);
        }

        private void Start()
        {
            savePath = Path.Combine(Application.persistentDataPath, "inventory.json");
            // check if there is a save file else 
            initialPlayerItems.AddInitialItems(Instance);
            sortButton.onClick.AddListener(SortInventory);
            UpdateAmmoAndWeight();
        }
        #endregion

        #region Inventory Initialization and Loading

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

        public void ConsumeItem(ItemDataSO item, Slot slot, int amount)
        {
            DeductItem(item, amount);
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