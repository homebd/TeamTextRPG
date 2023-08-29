/// <summary
/// 캐릭터 클래스
/// </summary>

namespace TeamTextRPG.Classes
{
    internal class Character
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Exp { get; set; }
        public int Gold { get; set; }
        public float CriticalChance { get; set; } //0~1
        public float CriticalDamage { get; set; } //140% = 1.4f
        public float DodgeChance { get; set; } // 0~1
        public List<Item> Inventory { get; set; }

        public Character()
        {

        }

        public void ChangeHP(int hp)
        {
            var totalHp = MaxHp;

            CurrentHp += hp;

            if (totalHp < CurrentHp)
            {
                CurrentHp = totalHp;
            }

            if (CurrentHp < 0)
            {
                CurrentHp = 0;
            }
        }
    }
}
