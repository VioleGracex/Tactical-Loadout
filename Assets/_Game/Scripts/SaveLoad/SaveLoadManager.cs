using System.IO;
using Data;
using Game;
using Managers;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveLoadManager
    {
        // Base save path
        private static string saveFolderPath = Application.persistentDataPath;

        // Method to get the save file path for a specific slot
        private static string GetSaveFilePath(int slotId)
        {
            return Path.Combine(saveFolderPath, $"save_slot_{slotId}.json");
        }

        // Save game data to a specific slot
        public static void SaveGame(int slotId, GameManager gameManager, InventoryManager inventoryManager, EquipmentManager equipmentManager)
        {
            SaveData saveData = new SaveData();

            // Save the current time
            saveData.saveTime = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            // Save Player Data
            saveData.playerHP = gameManager.player.hp;
            saveData.playerLevel = gameManager.player.level;

            // Save Enemy Data
            saveData.enemyHP = gameManager.enemy.hp;
            saveData.enemyLevel = gameManager.enemy.level;

            // Save Inventory Data
            foreach (var slot in inventoryManager.slots)
            {
                if (slot.CurrentItem != null)
                {
                    saveData.inventorySlots.Add(new SlotSaveData
                    {
                        item = new ItemDataDTO(slot.CurrentItem),
                        amount = slot.CurrentAmount,
                        slotId = slot.id,
                        isEquipped = slot.IsEquipped // Save equipped state
                    });
                }
            }

            // Save Equipment Slot IDs
            saveData.headSlotId = equipmentManager.headSlot?.id ?? -1;
            saveData.torsoSlotId = equipmentManager.torsoSlot?.id ?? -1;
            saveData.pistolSlotId = equipmentManager.pistolSlot?.id ?? -1;
            saveData.rifleSlotId = equipmentManager.rifleSlot?.id ?? -1;

            // Convert to JSON and save to file
            string json = JsonUtility.ToJson(saveData, true);
            string savePath = GetSaveFilePath(slotId);

            try
            {
                File.WriteAllText(savePath, json);
                Debug.Log($"Game saved to Slot {slotId} at {savePath}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to save game to Slot {slotId}: {ex.Message}");
            }
        }

        // Load game data from a specific slot
        public static SaveData LoadGame(int slotId)
        {
            string savePath = GetSaveFilePath(slotId);

            if (File.Exists(savePath))
            {
                try
                {
                    string json = File.ReadAllText(savePath);
                    SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log($"Game loaded from Slot {slotId} at {savePath}");
                    return saveData;
                }
                catch (IOException ex)
                {
                    Debug.LogError($"Failed to load game from Slot {slotId}: {ex.Message}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Unexpected error loading game from Slot {slotId}: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"No save file found for Slot {slotId}.");
            }

            return null;
        }
    }
}