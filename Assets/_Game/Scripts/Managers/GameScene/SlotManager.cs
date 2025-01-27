using System.Collections.Generic;
using System.Linq;
using Data;
using Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class SlotManager : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject slotPrefab;

        [Header("Other")]
        public Transform inventoryParent;
        public GridLayoutGroup gridLayoutGroup;

        [Header("Managers")]
        [SerializeField] ItemPopupManager itemPopupManager;
        [SerializeField] EquipmentManager equipmentManager;

        public void InitializeSlots(int slotCount)
        {
            foreach (Transform child in inventoryParent)
            {
                Destroy(child.gameObject);
            }
            InventoryManager.Instance.slots.Clear();
            InventoryManager.Instance.itemDictionary.Clear();
            InventoryManager.Instance.itemSlotDictionary.Clear();

            for (int i = 0; i < slotCount; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, inventoryParent);
                Slot slot = newSlot.GetComponent<Slot>();
                slot.id = i;
                slot.OnSlotClicked += itemPopupManager.ShowItemPopup;
                slot.OnItemSwapped += HandleItemSwapped;
                InventoryManager.Instance.slots.Add(slot);
            }
        }


        public void HandleItemSwapped(Slot originalSlot, Slot newSlot)
        {
            ItemDataSO originalItem = originalSlot.CurrentItem;
            ItemDataSO newItem = newSlot.CurrentItem ;

            if (originalItem != null)
            {
                InventoryManager.Instance.itemSlotDictionary[originalItem.itemName].Remove(originalSlot);
                InventoryManager.Instance.itemSlotDictionary[originalItem.itemName].Add(newSlot);
            }

            if (newItem != null)
            {
                InventoryManager.Instance.itemSlotDictionary[newItem.itemName].Remove(newSlot);
                InventoryManager.Instance.itemSlotDictionary[newItem.itemName].Add(originalSlot);
            }
            if(newSlot.IsEquipped || originalSlot.IsEquipped)
                equipmentManager.HandleSwappedSlots(originalSlot, newSlot);
        }
    }
}