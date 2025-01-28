using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class Player : Character
    {
        public PlayerDataSO playerData;
        public ItemDataSO pistol;
        public ItemDataSO rifle;

        public override void Initialize(CharacterDataSO data)
        {
            base.Initialize(data);
            playerData = (PlayerDataSO)data;
            pistol = playerData.pistol;
            rifle = playerData.rifle;
        }

        public void TakeDamageFromEnemy()
        {
            int damage = 15;
            if (Random.Range(0, 2) == 0) // Randomly choose between head and torso
            {
                hp -= Mathf.Max(0, damage - (headArmor != null ? headArmor.defenseModifier : 0));
                Debug.Log("shot player in head");
            }
            else
            {
                hp -= Mathf.Max(0, damage - (torsoArmor != null ? torsoArmor.defenseModifier : 0));
                Debug.Log("shot player in torso");
            }

            if (hp <= 0)
            {
                hp = 0;
                Debug.Log("Player died.");
                Object.FindFirstObjectByType<GameManager>().ShowGameOver();
                // Call game over pop up
            }
        }

        public void Heal(int healAmount)
        {
            hp = Mathf.Min(hp + healAmount, 100);
        }
    }
}
