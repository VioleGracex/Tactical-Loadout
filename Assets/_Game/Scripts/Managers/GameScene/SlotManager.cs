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
        [SerializeField] InventoryManager inventoryManager;

        public void InitializeSlots(int slotCount)
        {
            foreach (Transform child in inventoryParent)
            {
                Destroy(child.gameObject);
            }
            inventoryManager.slots.Clear();
            inventoryManager.itemDictionary.Clear();
            inventoryManager.itemSlotDictionary.Clear();

            for (int i = 0; i < slotCount; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, inventoryParent);
                Slot slot = newSlot.GetComponent<Slot>();
                slot.id = i;
                slot.OnSlotClicked += itemPopupManager.ShowItemPopup;
                slot.OnItemSwapped += HandleItemSwapped;
                inventoryManager.slots.Add(slot);
            }
        }


        public void HandleItemSwapped(Slot originalSlot, Slot newSlot)
        {
            ItemDataSO originalItem = originalSlot.CurrentItem;
            ItemDataSO newItem = newSlot.CurrentItem ;

            if (originalItem != null)
            {
                inventoryManager.itemSlotDictionary[originalItem.itemName].Remove(originalSlot);
                inventoryManager.itemSlotDictionary[originalItem.itemName].Add(newSlot);
            }

            if (newItem != null)
            {
                inventoryManager.itemSlotDictionary[newItem.itemName].Remove(newSlot);
                inventoryManager.itemSlotDictionary[newItem.itemName].Add(originalSlot);
            }
        }
       
    }
}