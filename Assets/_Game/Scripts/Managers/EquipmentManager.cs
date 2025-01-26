using Data;
using Game;
using Inventory;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class EquipmentManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player player;
        [SerializeField] private InventoryManager inventoryManager;

        [Header("Stat Information")]
        [SerializeField] private TextMeshProUGUI headArmorText;
        [SerializeField] private TextMeshProUGUI torsoArmorText, pistolDamageText, rifleDamageText;

        [Header("Equipment Slots")]
        [SerializeField] private Slot headSlot; // Slot for head armor
        [SerializeField] private Slot torsoSlot; // Slot for torso armor
        [SerializeField] private Slot pistolSlot; // Slot for pistol
        [SerializeField] private Slot rifleSlot; // Slot for rifle

        void Start()
        {
            player = GameManager.Instance.player;
            UpdatePlayerStatsUI();
        }

        private void UpdatePlayerStatsUI()
        {
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
                : "0";

            // Update rifle damage text
            rifleDamageText.text = player.rifle != null
                ? $"{player.rifle.damageModifier}"
                : "0";
        }

        public void EquipOrUnequip(ItemDataSO item, Slot slot)
        {
            if (item == null)
            {
                Debug.LogWarning("Invalid item for equipping/unequipping.");
                return;
            }

            bool isEquipped = slot.isEquipped();

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
                        Debug.Log($"Unequipped {item.itemName} from head armor.");
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
                        headSlot.SetEquippedText(); // Mark the new slot as equipped
                        Debug.Log($"Equipped {item.itemName} as head armor.");
                    }
                    break;

                case EquipmentType.Torso:
                    if (isEquipped)
                    {
                        Unequip(player.torsoArmor, torsoSlot);
                        player.torsoArmor = null;
                        Debug.Log($"Unequipped {item.itemName} from torso armor.");
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
                        torsoSlot.SetEquippedText(); // Mark the new slot as equipped
                        Debug.Log($"Equipped {item.itemName} as torso armor.");
                    }
                    break;

                default:
                    Debug.LogWarning("Invalid equipment type.");
                    return;
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
                        Debug.Log($"Unequipped {item.itemName} as pistol.");
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
                        pistolSlot.SetEquippedText(); // Mark the new slot as equipped
                        Debug.Log($"Equipped {item.itemName} as pistol.");
                    }
                    break;

                case ItemType.Rifle:
                    if (isEquipped)
                    {
                        Unequip(player.rifle, rifleSlot);
                        player.rifle = null;
                        Debug.Log($"Unequipped {item.itemName} as rifle.");
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
                        rifleSlot.SetEquippedText(); // Mark the new slot as equipped
                        Debug.Log($"Equipped {item.itemName} as rifle.");
                    }
                    break;

                default:
                    Debug.LogWarning("Invalid weapon type.");
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
                slot.SetEquippedText();
            }

            if (item.equipmentType != EquipmentType.None)
            {
                switch (item.equipmentType)
                {
                    case EquipmentType.Head:
                        player.headArmor = null;
                        Debug.Log($"Unequipped {item.itemName} from head armor.");
                        break;
                    case EquipmentType.Torso:
                        player.torsoArmor = null;
                        Debug.Log($"Unequipped {item.itemName} from torso armor.");
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
                        Debug.Log($"Unequipped {item.itemName} as pistol.");
                        break;
                    case ItemType.Rifle:
                        player.rifle = null;
                        Debug.Log($"Unequipped {item.itemName} as rifle.");
                        break;
                    default:
                        Debug.LogWarning("Invalid weapon type.");
                        return;
                }
            }
        }
    }
}