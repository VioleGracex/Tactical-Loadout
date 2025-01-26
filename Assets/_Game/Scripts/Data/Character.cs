using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Data
{
    public class Character 
    {
        public CharacterDataSO characterData;

        public float hp = 100;
        public int level = 1;
        public ItemDataSO headArmor;
        public ItemDataSO torsoArmor;

        public virtual void Initialize(CharacterDataSO data)
        {
            characterData = data;
            hp = data.hp;
            level = data.level;
            headArmor = data.headArmor;
            torsoArmor = data.torsoArmor;
        }
    }

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
            }
            else
            {
                hp -= Mathf.Max(0, damage - (torsoArmor != null ? torsoArmor.defenseModifier : 0));
            }

            if (hp <= 0)
            {
                hp = 0;
                Debug.Log("Player died.");
                GameManager.Instance.ShowGameOver();
                // Call game over pop up
            }
        }

        public void Heal(int healAmount)
        {
            hp = Mathf.Min(hp + healAmount, 100);
        }
    }

    public class Enemy : Character
    {
        public override void Initialize(CharacterDataSO data)
        {
            base.Initialize(data);
        }

        public void TakeDamage(int damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            DropLoot();
        }

        private void DropLoot() =>
            // Call generate random loot pop up from game manager
            GameManager.Instance.GenerateLoot(level); // Assuming GameManager has a singleton instance
    }
}