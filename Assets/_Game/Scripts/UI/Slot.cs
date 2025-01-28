using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Data;
using Managers;

namespace Inventory
{
    public class Slot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        public Image itemImage;
        public TextMeshProUGUI itemAmountText;

        [SerializeField] private ItemDataSO currentItem;
        [SerializeField] private int currentAmount;
        [SerializeField] private bool isEquipped; // Track equipped state independently
        [SerializeField] public int id;

        public ItemDataSO CurrentItem
        {
            get => currentItem;
            set
            {
                currentItem = value;
                UpdateUI();
            }
        }

        public int CurrentAmount
        {
            get => currentAmount;
            set
            {
                currentAmount = value;
                UpdateItemAmountText();
            }
        }

        public bool IsEquipped => isEquipped; // Use the dedicated field

        public delegate void SlotClickedHandler(ItemDataSO item, Slot slot);
        public event SlotClickedHandler OnSlotClicked;

        public delegate void ItemSwappedHandler(Slot originalSlot, Slot newSlot);
        public event ItemSwappedHandler OnItemSwapped;

        public Slot(ItemDataSO item = null, int amount = 0, bool isEquipped = false)
        {
            Debug.Log("Name in slot set " + item.name);
            CurrentItem = item;
            CurrentAmount = amount;
            this.isEquipped = isEquipped; // Initialize equipped state
            UpdateUI();
        }

        public void SetItem(ItemDataSO item, int amount)
        {
            if (item != null)
            {
                Debug.Log("Name in slot " + item.name);
                CurrentItem = item;
                currentItem.itemName = item.itemName;
                CurrentAmount = amount;
            }


        }

        public void SetEquippedText(bool equipped)
        {
            isEquipped = equipped; // Update the equipped state
            itemAmountText.text = equipped ? "E" : string.Empty;
        }

        public void DeductAmount(int amount)
        {
            CurrentAmount -= amount;
            if (CurrentAmount <= 0)
            {
                CurrentItem = null;
            }
        }

        private void UpdateUI()
        {
            // Check if itemImage is null or destroyed
            if (itemImage == null)
            {
                Debug.LogWarning("itemImage is null or destroyed.");
                return;
            }

            if (currentItem != null && currentItem.itemImage != null)
            {
                itemImage.sprite = currentItem.itemImage;
                itemImage.enabled = true;

                // Update the item amount text if the item is not equipped
                if (!isEquipped)
                {
                    UpdateItemAmountText();
                }
            }
            else
            {
                itemImage.enabled = false;
                itemAmountText.text = string.Empty;
            }
        }

        private void UpdateItemAmountText()
        {
            if (itemAmountText == null)
            {
                Debug.LogWarning("itemAmountText is not assigned.");
                return;
            }

            if (currentItem != null && currentAmount > 0 && currentItem.maxStack > 1)
            {
                itemAmountText.text = currentAmount.ToString();
            }
            else
            {
                itemAmountText.text = string.Empty;
            }
        }

        public bool CanBeDragged()
        {
            return currentItem != null && itemImage.enabled;
        }
        public void OnDrop(PointerEventData eventData)
        {
            DragDropItem draggedItem = eventData.pointerDrag.GetComponent<DragDropItem>();
            if (draggedItem != null)
            {
                Slot originalSlot = draggedItem.originalParent.GetComponent<Slot>();

                if (originalSlot != null && originalSlot != this && originalSlot.CanBeDragged())
                {
                    ItemDataSO draggedItemData = originalSlot.CurrentItem;
                    int draggedAmount = originalSlot.CurrentAmount;

                    // Check if the original slot was equipped
                    bool wasOriginalSlotEquipped = originalSlot.IsEquipped;

                    // Swap items and amounts
                    (CurrentItem, originalSlot.CurrentItem) = (originalSlot.CurrentItem, CurrentItem);
                    (CurrentAmount, originalSlot.CurrentAmount) = (originalSlot.CurrentAmount, CurrentAmount);

                    // Update the equipped state for both slots
                    originalSlot.SetEquippedText(this.IsEquipped); // Original slot gets this slot's equipped state
                    this.SetEquippedText(wasOriginalSlotEquipped); // This slot gets the original slot's equipped state

                    // If the original slot was equipped and the item is equippable, update the equipped slot
                    if (wasOriginalSlotEquipped && draggedItemData != null && IsItemEquippable(draggedItemData))
                    {
                        EquipmentManager.Instance.SwapEquippedSlot(draggedItemData, this);
                    }

                    // If this slot was equipped and the new item is equippable, update the equipped slot
                    if (this.IsEquipped && CurrentItem != null && IsItemEquippable(CurrentItem))
                    {
                        EquipmentManager.Instance.SwapEquippedSlot(CurrentItem, this);
                    }

                    // Update the UI for both slots
                    originalSlot.UpdateUI();
                    this.UpdateUI();

                    // Notify the equipment manager of the swap
                    OnItemSwapped?.Invoke(originalSlot, this);
                }
                else
                {
                    Debug.Log("Invalid target for swap. Returning item to original slot.");
                    draggedItem.ResetToOriginalParent();
                }
            }
        }
        private bool IsItemEquippable(ItemDataSO item)
        {
            // Check if the item is of a type that can be equipped
            return item.equipmentType != EquipmentType.None;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSlotClicked?.Invoke(CurrentItem, this);
        }
    }
}