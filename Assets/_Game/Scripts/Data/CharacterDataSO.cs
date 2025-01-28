using UnityEngine;

namespace Data
{
    //[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
    public class CharacterDataSO : ScriptableObject
    {
        public string characterName;
        public float hp = 100;
        public int level = 1;
        public ItemDataSO headArmor;
        public ItemDataSO torsoArmor;
    }
}