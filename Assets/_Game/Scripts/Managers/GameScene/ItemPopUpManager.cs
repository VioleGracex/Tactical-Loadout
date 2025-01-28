using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory;
using Data;
using Game;

namespace Managers
{
    public class ItemPopupManager : MonoBehaviour
    {
        [Header("Stat Information")]
        [SerializeField] private StatInfo damageInfo;
        [SerializeField] private StatInfo defenseInfo, healInfo, weightInfo;

        [Header("Item Information")]
        public GameObject itemPopupWindow;
        public TextMeshProUGUI itemNameText;
        public Image itemImage;
        public TextMeshProUGUI itemDescriptionText;
        public Button actionButton, throwButton;

        [Header("Managers")]
        [SerializeField] EquipmentManager equipmentManager;
        [SerializeField] InventoryManager inventoryManager;

        [SerializeField] GameManager gameManager;

        private ItemDataSO currentItem;
        private Slot currentSlot;

        private void Awake()
        {
            // Ensure buttons are cleared initially
            gameManager = FindFirstObjectByType<GameManager>();
            actionButton.onClick.RemoveAllListeners();
            throwButton.onClick.RemoveAllListeners();
        }

        public void ShowItemPopup(ItemDataSO item, Slot slot)
        {
            if (item == null) return;

            currentItem = item;
            currentSlot = slot;

            // Update UI elements
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
            if (slot.CurrentAmount > 1)
            {
                totalWeight *= slot.CurrentAmount;
            }
            weightInfo.SetStat(totalWeight.ToString("F2"));

            // Configure action button based on item type and equipped state
            actionButton.onClick.RemoveAllListeners();
            throwButton.onClick.RemoveAllListeners();

            if (slot.IsEquipped)
            {
                // If the item is equipped, show the Unequip button
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Снять";
                actionButton.onClick.AddListener(() => Equip(item, slot));
            }
            else
            {
                // If the item is not equipped, show the appropriate action button
                switch (item.type)
                {
                    case ItemType.Ammo:
                        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Купить";
                        actionButton.onClick.AddListener(() => BuyItem(item, 100));
                        break;
                    case ItemType.Consumable:
                        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Лечить";
                        actionButton.onClick.AddListener(() => ConsumeHealItem(item, slot, 1));
                        break;
                    case ItemType.Equipment:
                    case ItemType.Pistol:
                    case ItemType.Rifle:
                        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Экипировать";
                        actionButton.onClick.AddListener(() => Equip(item, slot));
                        break;
                }
            }

            throwButton.onClick.AddListener(() => ThrowItem(slot));

            // Show the popup
            itemPopupWindow.SetActive(true);
        }

        private void BuyItem(ItemDataSO item, int amount)
        {
            inventoryManager.AddItem(item, amount);
            HidePopup();
        }

        private void ConsumeHealItem(ItemDataSO item, Slot slot, int amount)
        {
            gameManager.Heal(amount);
            inventoryManager.ConsumeItem(item, slot, amount);
            HidePopup();
        }

        private void Equip(ItemDataSO item, Slot slot)
        {
            equipmentManager.EquipOrUnequip(item, slot);
            HidePopup();
        }

        private void ThrowItem(Slot slot)
        {
            // Check if the item is equipped and unequip it before throwing
            if (slot.IsEquipped)
            {
                equipmentManager.EquipOrUnequip(slot.CurrentItem, slot);
            }
            inventoryManager.RemoveItem(slot.id);
            HidePopup();
        }

        public void HidePopup()
        {
            itemPopupWindow.SetActive(false);
        }
    }
}