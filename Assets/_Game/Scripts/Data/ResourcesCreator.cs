using Data;
using UnityEngine;

namespace Managers
{
    public static class ResourcesCreator
    {
        public static ItemDataSO CreateMedkit()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Health Pack",
                type: ItemType.Consumable,
                equipmentType: EquipmentType.None,
                maxStack: 6,
                weightPerUnit: 0.5f,
                description: "Restores 50 HP",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 50,
                spritePath: "Sprites/medkitSprite"
            );
        }

        public static ItemDataSO CreatePistolAmmo()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "9x18mm Ammo",
                type: ItemType.Ammo,
                equipmentType: EquipmentType.None,
                maxStack: 50,
                weightPerUnit: 0.01f,
                description: "Basic ammo for pistols",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/ammo9x18Sprite"
            );
        }

        public static ItemDataSO CreateMediumMedkit()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Big Health Pack",
                type: ItemType.Consumable,
                equipmentType: EquipmentType.None,
                maxStack: 1,
                weightPerUnit: 0.5f,
                description: "Restores 100 HP",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 100,
                spritePath: "Sprites/medium_medkitSprite"
            );
        }

        public static ItemDataSO CreateRifleAmmo()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "5.45х39mm Ammo",
                type: ItemType.Ammo,
                equipmentType: EquipmentType.None,
                maxStack: 100,
                weightPerUnit: 0.03f,
                description: "Advanced ammo for rifles",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/ammo545x39Sprite"
            );
        }

        public static ItemDataSO CreateSniperAmmo()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "High-Caliber Ammo",
                type: ItemType.Ammo,
                equipmentType: EquipmentType.None,
                maxStack: 100,
                weightPerUnit: 0.05f,
                description: "High-caliber ammo for advanced weapons",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/ammo545x39Sprite"
            );
        }

        public static ItemDataSO CreateBasicBodyArmor()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Basic Body Armor",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Torso,
                maxStack: 1,
                weightPerUnit: 5.0f,
                description: "Looks Nice",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/basic_body_armorSprite"
            );
        }

        public static ItemDataSO CreateAdvancedBodyArmor()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Advanced Body Armor",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Torso,
                maxStack: 1,
                weightPerUnit: 10.0f,
                description: "Provides advanced torso defense",
                damageModifier: 0,
                defenseModifier: 10,
                healValue: 0,
                spritePath: "Sprites/advanced_body_armorSprite"
            );
        }

        public static ItemDataSO CreateBasicHelmet()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Basic Helmet",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Head,
                maxStack: 1,
                weightPerUnit: 1.0f,
                description: "Provides basic head defense",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/basic_helmetSprite"
            );
        }

        public static ItemDataSO CreateAdvancedHelmet()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Advanced Helmet",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Head,
                maxStack: 1,
                weightPerUnit: 2.0f,
                description: "Provides advanced head defense",
                damageModifier: 0,
                defenseModifier: 10,
                healValue: 0,
                spritePath: "Sprites/advanced_helmetSprite"
            );
        }
    }
}