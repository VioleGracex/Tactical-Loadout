using System.Collections.Generic;
using Data;
using Inventory;

namespace SaveSystem
{
    [System.Serializable]
    public class SaveData
    {
        // Player Data
        public float playerHP;
        public int playerLevel;

        // Enemy Data
        public float enemyHP;
        public int enemyLevel;

        // Inventory Data
        public List<SlotSaveData> inventorySlots = new List<SlotSaveData>();

        // Equipment Slot Data
        public int headSlotId;
        public int torsoSlotId;
        public int pistolSlotId;
        public int rifleSlotId;

        public string saveTime;
    }

    [System.Serializable]
    public class SlotSaveData
    {
        public ItemDataDTO item;
        public int amount;
        public int slotId;
        public bool isEquipped; // Track if the slot is equipped

        // Convert SlotSaveData back to InventorySlot
        public Slot ToInventorySlot()
        {
            return new Slot(item.ToItemDataSO(), amount, isEquipped);
        }
    }
}