   using UnityEngine;

namespace Data
{
   [CreateAssetMenu(fileName = "NewPlayerData", menuName = "Character/PlayerData")]
    public class PlayerDataSO : CharacterDataSO
    {
        public ItemDataSO pistol;
        public ItemDataSO rifle;
    }
}