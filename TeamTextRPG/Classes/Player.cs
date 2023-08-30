/// <summary>
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
        public List<Skill> Skills { get; set; }

        public Player(string name, JOB job, int level, int atk, int def, int maxHp, int maxMp, int gold
            , int exp = 0, int cc = 10, int cd = 160, int dc = 5, int currentHp = -1, int currentMp = -1)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            MaxHp = maxHp;
            MaxMp = maxMp;
            Gold = gold;

            if (currentHp == -1) CurrentHp = maxHp;
            else CurrentHp = currentHp;

            if (currentMp == -1) CurrentMp = maxMp;
            else CurrentMp = currentMp;

            Exp = exp;

            CriticalChance = cc;
            CriticalDamage = cd;
            DodgeChance = dc;

            Inventory = new List<Item>();
            Equipments = new Item[Enum.GetValues(typeof(Parts)).Length];
            Skills = new List<Skill>();
        }

        public void ChangeHP(int hp)
        {
            var totalHp = MaxHp + GetEquipmentStatBonus(Stats.MAXHP);

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

        public int GetEquipmentStatBonus(Stats stat)
        {
            int bonus = 0;
            switch (stat)
            {
                case Stats.MAXHP:
                    if(Equipments[(int)Parts.HELMET] != null)  bonus += Equipments[(int)Parts.HELMET].Stat; 
                    break;
                case Stats.MAXMP:
                    break;
                case Stats.ATK:
                    if (Equipments[(int)Parts.WEAPON] != null) bonus += Equipments[(int)Parts.WEAPON].Stat;
                    break;
                case Stats.DEF:
                    if (Equipments[(int)Parts.CHESTPLATE] != null) bonus += Equipments[(int)Parts.CHESTPLATE].Stat;
                    if (Equipments[(int)Parts.LEGGINGS] != null) bonus += Equipments[(int)Parts.LEGGINGS].Stat;
                    break;
                case Stats.CRITICALCHANCE:
                    break;
                case Stats.CRITICALDAMAGE:
                    break;
                case Stats.DODGECHANCE:
                    if (Equipments[(int)Parts.BOOTS] != null) bonus += Equipments[(int)Parts.BOOTS].Stat;
                    break;
            }

            return bonus;
        }

        public void Wear(Item item)
        {
            Equipments[(int)item.Part] = item;

            if (item.Part == Parts.HELMET || item.Part == Parts.BOOTS)
            {
                ChangeHP(item.Stat + item.BonusStat);
            }

            item.IsEquipped = true;
        }

        public void Unwear(Parts part)
        {
            if (part == Parts.HELMET)
            {
                int hp;
                if (CurrentHp <= Equipments[(int)part].Stat + Equipments[(int)part].BonusStat)
                    hp = (int)CurrentHp - 1;
                else
                    hp = Equipments[(int)part].Stat + Equipments[(int)part].BonusStat;

                ChangeHP(-hp);
            }

            Item item = Equipments[(int)part];
            Equipments[(int)part] = null;
            item.IsEquipped = false;
        }
    }
}
