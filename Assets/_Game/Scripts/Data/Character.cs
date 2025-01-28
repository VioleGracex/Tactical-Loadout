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
}