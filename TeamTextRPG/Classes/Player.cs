/// <summary
/// 플레이어 클래스
/// </summary>

using System.Transactions;
using TeamTextRPG.Common;

namespace TeamTextRPG.Classes
{
    internal class Player : Character
    {
        public JOB Job { get; }
        public Item[]? Equipments { get; set; }
        //public List<Skill> Skills { get; set; }

        public Player(string name, JOB job, int level, int atk, int def, int maxHp, int gold
            , int currentHp = -1, int exp = 0, float cc = 0.15f, float cd = 1.6f, float dc = 0.05f)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            MaxHp = maxHp;
            Gold = gold;

            if (currentHp == -1) CurrentHp = maxHp;
            else CurrentHp = currentHp;
            Exp = exp;

            CriticalChance = cc;
            CriticalDamage = cd;
            DodgeChance = dc;

            Inventory = new List<Item>();
            Equipments = new Item[Enum.GetValues(typeof(Parts)).Length];
            //Skills = new List<Skill>();
        }

        public void ChangeHP(int hp)
        {
            var totalHp = MaxHp;

            var helmet = Equipments[(int)(Parts.HELMET)];
            var boots = Equipments[(int)(Parts.BOOTS)];

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

            if (Equipments[(int)Parts.WEAPON] != null)
            {
                atkBonus = Equipments[(int)Parts.WEAPON].Stat
                    + Equipments[(int)Parts.WEAPON].BonusStat;
            }

            return atkBonus;
        }

        public int GetDefBonus(bool print = true)
        {
            int defBonus = 0;

            if (Equipments[(int)Parts.CHESTPLATE] != null)
                defBonus += Equipments[(int)Parts.CHESTPLATE].Stat
                    + Equipments[(int)Parts.CHESTPLATE].BonusStat;

            if (Equipments[(int)Parts.LEGGINGS] != null)
                defBonus += Equipments[(int)Parts.LEGGINGS].Stat
                    + Equipments[(int)Parts.LEGGINGS].BonusStat;

            return defBonus;
        }

        public int GetHpBonus(bool print = true)
        {
            int hpBonus = 0;

            if (Equipments[(int)Parts.HELMET] != null)
                hpBonus += Equipments[(int)Parts.HELMET].Stat
                    + Equipments[(int)Parts.HELMET].BonusStat;

            if (Equipments[(int)Parts.BOOTS] != null)
                hpBonus += Equipments[(int)Parts.BOOTS].Stat
                    + Equipments[(int)Parts.BOOTS].BonusStat;

            return hpBonus;
        }

        public void Wear(Item item)
        {
            Equipments[(int)item.Part] = item;

            if (item.Part == Parts.HELMET || item.Part == Parts.BOOTS)
            {
                ChangeHP(item.Stat + item.BonusStat);
            }
        }

        public void Unwear(Parts part)
        {
            if (part == Parts.HELMET || part == Parts.BOOTS)
            {
                int hp;
                if (CurrentHp <= Equipments[(int)part].Stat + Equipments[(int)part].BonusStat)
                    hp = (int)CurrentHp - 1;
                else
                    hp = Equipments[(int)part].Stat + Equipments[(int)part].BonusStat;

                ChangeHP(-hp);
            }
            Equipments[(int)part] = null;

        }
    }
}
