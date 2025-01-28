using System.Linq;
using Data;
using Game;
using Inventory;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; } // Singleton instance

        [Header("References")]
        [SerializeField] private Player player;
        [SerializeField] private InventoryManager inventoryManager;

        [Header("Stat Information")]
        [SerializeField] private TextMeshProUGUI headArmorText;
        [SerializeField] private TextMeshProUGUI torsoArmorText, pistolDamageText, rifleDamageText;

        [Header("Equipment Slots")]
        public Slot headSlot, torsoSlot, pistolSlot, rifleSlot;

        void Start()
        {
            player = GameManager.Instance.player;
            UpdatePlayerStatsUI();
        }

        private void UpdatePlayerStatsUI()
        {
            if(player == null)
                return;
            // Update head armor text
            headArmorText.text = player.headArmor != null
                ? $"{player.headArmor.defenseModifier}"
                : "0";

            // Update body armor text
            torsoArmorText.text = player.torsoArmor != null
                ? $"{player.torsoArmor.defenseModifier}"
                : "0";

            // Update pistol damage text
            pistolDamageText.text = player.pistol != null
                ? $"{player.pistol.damageModifier}"
                : "5";

            // Update rifle damage text
            rifleDamageText.text = player.rifle != null
                ? $"{player.rifle.damageModifier}"
                : "15";
        }

        public void EquipOrUnequip(ItemDataSO item, Slot slot)
        {
            if (item == null)
            {
                Debug.LogWarning("Invalid item for equipping/unequipping.");
                return;
            }

            bool isEquipped = slot.IsEquipped;

            if (item.equipmentType != EquipmentType.None)
            {
                HandleEquipment(item, slot, isEquipped);
            }
            else if (item.type == ItemType.Pistol || item.type == ItemType.Rifle)
            {
                HandleWeapon(item, slot, isEquipped);
            }
            else
            {
                Debug.LogWarning("Invalid item for equipping/unequipping.");
                return;
            }

            UpdatePlayerStatsUI();
            inventoryManager.UpdateAmmoAndWeight();
        }

        private void HandleEquipment(ItemDataSO item, Slot slot, bool isEquipped)
        {
            switch (item.equipmentType)
            {
                case EquipmentType.Head:
                    if (isEquipped)
                    {
                        Unequip(player.headArmor, headSlot);
                        player.headArmor = null;
                    }
                    else
                    {
                        if (player.headArmor != null)
                        {
                            // Unequip the currently equipped head armor
                            Unequip(player.headArmor, headSlot);
                        }
                        player.headArmor = item;
                        headSlot = slot; // Update the head slot reference
                        headSlot.SetEquippedText(true); // Mark the new slot as equipped
                    }
                    break;

                case EquipmentType.Torso:
                    if (isEquipped)
                    {
                        Unequip(player.torsoArmor, torsoSlot);
                        player.torsoArmor = null;
                    }
                    else
                    {
                        if (player.torsoArmor != null)
                        {
                            // Unequip the currently equipped torso armor
                            Unequip(player.torsoArmor, torsoSlot);
                        }
                        player.torsoArmor = item;
                        torsoSlot = slot; // Update the torso slot reference
                        torsoSlot.SetEquippedText(true); // Mark the new slot as equipped
                    }
                    break;

                default:
                    Debug.LogWarning("Invalid equipment type.");
                    return;
            }
        }

        private void Unequip(ItemDataSO item, Slot slot)
        {
            if (item == null)
            {
                Debug.LogWarning("Invalid item for unequipping.");
                return;
            }

            // Clear the "E" text from the slot
            if (slot != null)
            {
                slot.SetEquippedText(false);
            }

            if (item.equipmentType != EquipmentType.None)
            {
                switch (item.equipmentType)
                {
                    case EquipmentType.Head:
                        player.headArmor = null;
                        break;
                    case EquipmentType.Torso:
                        player.torsoArmor = null;
                        break;
                    default:
                        Debug.LogWarning("Invalid equipment type.");
                        return;
                }
            }
            else if (item.type == ItemType.Pistol || item.type == ItemType.Rifle)
            {
                switch (item.type)
                {
                    case ItemType.Pistol:
                        player.pistol = null;
                        break;
                    case ItemType.Rifle:
                        player.rifle = null;
                        break;
                    default:
                        Debug.LogWarning("Invalid weapon type.");
                        return;
                }
            }
        }

        private void HandleWeapon(ItemDataSO item, Slot slot, bool isEquipped)
        {
            switch (item.type)
            {
                case ItemType.Pistol:
                    if (isEquipped)
                    {
                        Unequip(player.pistol, pistolSlot);
                        player.pistol = null;
                    }
                    else
                    {
                        if (player.pistol != null)
                        {
                            // Unequip the currently equipped pistol
                            Unequip(player.pistol, pistolSlot);
                        }
                        player.pistol = item;
                        pistolSlot = slot; // Update the pistol slot reference
                        pistolSlot.SetEquippedText(true); // Mark the new slot as equipped
                    }
                    break;

                case ItemType.Rifle:
                    if (isEquipped)
                    {
                        Unequip(player.rifle, rifleSlot);
                        player.rifle = null;
                    }
                    else
                    {
                        if (player.rifle != null)
                        {
                            // Unequip the currently equipped rifle
                            Unequip(player.rifle, rifleSlot);
                        }
                        player.rifle = item;
                        rifleSlot = slot; // Update the rifle slot reference
                        rifleSlot.SetEquippedText(true); // Mark the new slot as equipped
                    }
                    break;

                default:
                    Debug.LogWarning("Invalid weapon type.");
                    return;
            }
        }

        public void LoadEquipment(SaveSystem.SaveData saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("No save data provided for loading equipment.");
                return;
            }

            // Re-equip Head Armor
            if (saveData.headSlotId != -1)
            {
                Slot headSlot = inventoryManager.slots.Find(s => s.id == saveData.headSlotId);
                if (headSlot != null && headSlot.CurrentItem != null)
                {
                    EquipOrUnequip(headSlot.CurrentItem, headSlot);
                }
            }

            // Re-equip Torso Armor
            if (saveData.torsoSlotId != -1)
            {
                Slot torsoSlot = inventoryManager.slots.Find(s => s.id == saveData.torsoSlotId);
                if (torsoSlot != null && torsoSlot.CurrentItem != null)
                {
                    EquipOrUnequip(torsoSlot.CurrentItem, torsoSlot);
                }
            }

            // Re-equip Pistol
            if (saveData.pistolSlotId != -1)
            {
                Slot pistolSlot = inventoryManager.slots.Find(s => s.id == saveData.pistolSlotId);
                if (pistolSlot != null && pistolSlot.CurrentItem != null)
                {
                    EquipOrUnequip(pistolSlot.CurrentItem, pistolSlot);
                }
            }
            // Re-equip Rifle
            if (saveData.rifleSlotId != -1)
            {
                Slot rifleSlot = inventoryManager.slots.Find(s => s.id == saveData.rifleSlotId);
                if (rifleSlot != null && rifleSlot.CurrentItem != null)
                {
                    EquipOrUnequip(rifleSlot.CurrentItem, rifleSlot);
                }
            }

            UpdatePlayerStatsUI();
        }
        public void SwapEquippedSlot(ItemDataSO item, Slot newSlot)
        {
            if (item == null || newSlot == null)
            {
                Debug.LogWarning("Invalid item or slot for swapping equipped slot.");
                return;
            }

            // Check the type of the item and swap the corresponding slot
            if (item.equipmentType != EquipmentType.None)
            {
                switch (item.equipmentType)
                {
                    case EquipmentType.Head:
                        if (headSlot != null)
                        {
                            headSlot.SetEquippedText(false); // Unequip the old head slot
                        }
                        headSlot = newSlot; // Update the head slot reference
                        headSlot.SetEquippedText(true); // Equip the new head slot
                        player.headArmor = item; // Update the player's head armor
                        break;

                    case EquipmentType.Torso:
                        if (torsoSlot != null)
                        {
                            torsoSlot.SetEquippedText(false); // Unequip the old torso slot
                        }
                        torsoSlot = newSlot; // Update the torso slot reference
                        torsoSlot.SetEquippedText(true); // Equip the new torso slot
                        player.torsoArmor = item; // Update the player's torso armor
                        break;

                    default:
                        Debug.LogWarning("Invalid equipment type for swapping equipped slot.");
                        return;
                }
            }
            else
            {
                Debug.LogWarning("Invalid item type for swapping equipped slot.");
                return;
            }

            // Update the player's stats UI
            UpdatePlayerStatsUI();
        }
    }
}