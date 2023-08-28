
using Newtonsoft.Json.Linq;
/// <summary
/// 캐릭터 클래스
/// </summary>

namespace TeamTextRPG.Classes
{
    public enum JOP
    {
        WARRIOR,
        WIZARD,
        ARCHER,
    }
    internal class Character
    {
        public string Name { get; }
        public JOP Job { get; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MaxHp { get; }
        public int CurrentHp { get; private set; }
        public int Gold { get; set; }
        public Item[]? Equipments { get; set; }

        public Character(string name, JOP job, int level, int atk, int def, int maxHp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Exp = 0;
            Atk = atk;
            Def = def;
            MaxHp = maxHp;
            CurrentHp = maxHp;
            Gold = gold;
            Equipments = new Item[Enum.GetValues(typeof(Item.Parts)).Length];
        }

        public void ChangeHP(int hp)
        {
            var totalHp = MaxHp;

            var helmet = Equipments[(int)(Item.Parts.HELMET)];
            var boots = Equipments[(int)(Item.Parts.BOOTS)];

            if (helmet != null)
                totalHp += helmet.Stat + helmet.BonusStat;
            if (boots != null)
                totalHp += boots.Stat + boots.BonusStat;

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
