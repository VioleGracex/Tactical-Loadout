using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
    public class CharacterDataSO : ScriptableObject
    {
        public string characterName;
        public float hp = 100;
        public int level = 1;
        public ItemDataSO headArmor;
        public ItemDataSO torsoArmor;
    }

    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "Character/PlayerData")]
    public class PlayerDataSO : CharacterDataSO
    {
        public ItemDataSO pistol;
        public ItemDataSO rifle;
    }

    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Character/EnemyData")]
    public class EnemyDataSO : CharacterDataSO
    {
    }
}