
using Newtonsoft.Json.Linq;
using System.Numerics;
/// <summary
/// 캐릭터 클래스
/// </summary>

namespace TeamTextRPG.Classes
{
    public enum JOB
    {
        WARRIOR,
        WIZARD,
        ARCHER,
    }
    internal class Character
    {
        public string Name { get; }
        public JOB Job { get; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MaxHp { get; }
        public int CurrentHp { get; private set; }
        public int Gold { get; set; }
        public Item[]? Equipments { get; set; }
        public float CriticalChance { get; set; } //0~1
        public float CriticalDamage { get; set; } //140% = 1.4f
        public float DodgeRate { get; set; } // 0~1

        public Character(string name, JOB job, int level, int atk, int def, int maxHp, int gold)
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

        public int GetAtkBonus(bool print = true)
        {
            int atkBonus = 0;

            if (Equipments[(int)Item.Parts.WEAPON] != null)
            {
                atkBonus = Equipments[(int)Item.Parts.WEAPON].Stat
                    + Equipments[(int)Item.Parts.WEAPON].BonusStat;
            }

            return atkBonus;
        }

        public int GetDefBonus(bool print = true)
        {
            int defBonus = 0;

            if (Equipments[(int)Item.Parts.CHESTPLATE] != null)
                defBonus += Equipments[(int)Item.Parts.CHESTPLATE].Stat
                    + Equipments[(int)Item.Parts.CHESTPLATE].BonusStat;

            if (Equipments[(int)Item.Parts.LEGGINGS] != null)
                defBonus += Equipments[(int)Item.Parts.LEGGINGS].Stat
                    + Equipments[(int)Item.Parts.LEGGINGS].BonusStat;

            return defBonus;
        }

        public int GetHpBonus(bool print = true)
        {
            int hpBonus = 0;

            if (Equipments[(int)Item.Parts.HELMET] != null)
                hpBonus += Equipments[(int)Item.Parts.HELMET].Stat
                    + Equipments[(int)Item.Parts.HELMET].BonusStat;

            if (Equipments[(int)Item.Parts.BOOTS] != null)
                hpBonus += Equipments[(int)Item.Parts.BOOTS].Stat
                    + Equipments[(int)Item.Parts.BOOTS].BonusStat;

            return hpBonus;
        }
    }
}
