using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Data;

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
            CurrentItem = item;
            CurrentAmount = amount;
            this.isEquipped = isEquipped; // Initialize equipped state
            UpdateUI();
        }

        public void SetItem(ItemDataSO item, int amount)
        {
            CurrentItem = item;
            CurrentAmount = amount;
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

                    // Swap equipped state
                    bool originalSlotEquipped = originalSlot.IsEquipped;
                    bool thisSlotEquipped = this.IsEquipped;

                    // Swap items and amounts
                    (CurrentItem, originalSlot.CurrentItem) = (originalSlot.CurrentItem, CurrentItem);
                    (CurrentAmount, originalSlot.CurrentAmount) = (originalSlot.CurrentAmount, CurrentAmount);

                    // Update the equipped state for both slots
                    originalSlot.SetEquippedText(thisSlotEquipped);
                    this.SetEquippedText(originalSlotEquipped);

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
        public void OnPointerClick(PointerEventData eventData)
        {
            OnSlotClicked?.Invoke(CurrentItem, this);
        }
    }
}