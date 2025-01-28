using Data;
using UnityEngine;

namespace Managers
{
    public static class ResourcesCreator
    {
        public static ItemDataSO CreateMedkit()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Аптечка",
                type: ItemType.Consumable,
                equipmentType: EquipmentType.None,
                maxStack: 6,
                weightPerUnit: 0.5f,
                description: "Восстанавливает 50 HP",
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
                description: "Основные боеприпасы для пистолетов",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/ammo9x18Sprite"
            );
        }

        public static ItemDataSO CreateMediumMedkit()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Большая аптечка",
                type: ItemType.Consumable,
                equipmentType: EquipmentType.None,
                maxStack: 1,
                weightPerUnit: 0.5f,
                description: "Восстанавливает 100 HP",
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
                description: "Усовершенствованные боеприпасы для винтовок",
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
                description: "Крупнокалиберные боеприпасы для современного оружия",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/ammo545x39Sprite"
            );
        }

        public static ItemDataSO CreateBasicBodyArmor()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Базовая броня",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Torso,
                maxStack: 1,
                weightPerUnit: 5.0f,
                description: "Выглядит неплохо",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/basic_body_armorSprite"
            );
        }

        public static ItemDataSO CreateAdvancedBodyArmor()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Улучшенная броня",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Torso,
                maxStack: 1,
                weightPerUnit: 10.0f,
                description: "Обеспечивает улучшенную защиту торса",
                damageModifier: 0,
                defenseModifier: 10,
                healValue: 0,
                spritePath: "Sprites/advanced_body_armorSprite"
            );
        }

        public static ItemDataSO CreateBasicHelmet()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Базовый шлем",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Head,
                maxStack: 1,
                weightPerUnit: 1.0f,
                description: "Обеспечивает базовую защиту головы",
                damageModifier: 0,
                defenseModifier: 0,
                healValue: 0,
                spritePath: "Sprites/basic_helmetSprite"
            );
        }

        public static ItemDataSO CreateAdvancedHelmet()
        {
            return InitialPlayerItems.CreateItem(
                itemName: "Улучшенный шлем",
                type: ItemType.Equipment,
                equipmentType: EquipmentType.Head,
                maxStack: 1,
                weightPerUnit: 2.0f,
                description: "Обеспечивает улучшенную защиту головы",
                damageModifier: 0,
                defenseModifier: 10,
                healValue: 0,
                spritePath: "Sprites/advanced_helmetSprite"
            );
        }
    }
}