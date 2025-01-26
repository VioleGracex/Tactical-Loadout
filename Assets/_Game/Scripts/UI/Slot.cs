using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Inventory
{
    public class Slot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        public Image itemImage;
        public TextMeshProUGUI itemAmountText;
        [SerializeField] private ItemDataSO currentItem;
        [SerializeField] public int id;
        private int currentAmount = 0;

        public delegate void SlotClickedHandler(ItemDataSO item, Slot slot);
        public event SlotClickedHandler OnSlotClicked;

        public delegate void ItemSwappedHandler(Slot originalSlot, Slot newSlot);
        public event ItemSwappedHandler OnItemSwapped;

        public void SetItem(ItemDataSO item, int amount = 1)
        {
            currentItem = item;
            currentAmount = amount;
            UpdateUI();
        }

        public bool IsEmpty()
        {
            return currentItem == null;
        }

        public ItemDataSO GetItem()
        {
            return currentItem;
        }

        public int GetCurrentAmount()
        {
            return currentAmount;
        }

        public void SetCurrentAmount(int amount)
        {
            currentAmount = amount;
            UpdateItemAmountText();
        }

        public void DeductAmount(int amount)
        {
            currentAmount -= amount;
            if (currentAmount <= 0)
            {
                SetItem(null);
            }
            else
            {
                UpdateItemAmountText();
            }
        }

        private void UpdateUI()
        {
            if (currentItem != null && currentItem.itemImage != null)
            {
                itemImage.sprite = currentItem.itemImage;
                itemImage.enabled = true;
                UpdateItemAmountText();
            }
            else
            {
                itemImage.enabled = false;
                if (itemAmountText != null)
                {
                    itemAmountText.text = string.Empty;
                    itemAmountText.enabled = false;
                }
            }
        }

        private void UpdateItemAmountText()
        {
            if (itemAmountText == null)
            {
                Debug.LogWarning("itemAmountText is not assigned.");
                return;
            }

            if (currentItem != null && (currentItem.type == ItemType.Consumable || currentItem.type == ItemType.Ammo))
            {
                itemAmountText.text = currentAmount.ToString();
                itemAmountText.enabled = true;
            }
            else
            {
                itemAmountText.text = string.Empty;
                itemAmountText.enabled = false;
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
                    Debug.Log($"Swapping items between slot {originalSlot.id} and slot {id}");

                    // Swap item data and images
                    ItemDataSO tempItem = originalSlot.GetItem();
                    int tempAmount = originalSlot.GetCurrentAmount();

                    originalSlot.SetItem(currentItem, currentAmount);
                    SetItem(tempItem, tempAmount);

                    OnItemSwapped?.Invoke(originalSlot, this);
                }
                else
                {
                    Debug.Log("Invalid target for swap. Returning item to original slot.");
                    // If invalid target, return the item to its original slot
                    draggedItem.ResetToOriginalParent();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSlotClicked?.Invoke(currentItem, this);
        }
    }
}