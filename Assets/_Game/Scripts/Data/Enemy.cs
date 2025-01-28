using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Data
{
    [System.Serializable]
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
                hp = 0;
                Die();
            }
        }

        private void Die()
        {
            DropLoot();
        }

        private void DropLoot() =>
           // Call generate random loot pop up from game manager
           Object.FindFirstObjectByType<GameManager>().GenerateLoot(level); // Assuming GameManager has a singleton instance
    }
}