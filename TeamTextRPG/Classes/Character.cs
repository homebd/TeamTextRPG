/// <summary>
/// 캐릭터 클래스
/// </summary>

using Newtonsoft.Json.Linq;
using System.Numerics;
using TeamTextRPG.Common;

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

        public void ChangeMP(int mp)
        {
            var totalMp = MaxMp;

            CurrentMp += mp;

            if (totalMp < CurrentMp)
            {
                CurrentMp = totalMp;
            }

            if (CurrentMp < 0)
            {
                CurrentMp = 0;
            }
        }

        public void ChangeStat(Stats stat, int value)
        {
            /*int statValue;
            switch(stat)
            {
                case Stats.MAXHP:
                    statValue = MaxHp;
                    break;
                case Stats.ATK:
                    statValue = Atk;
                    break;
                case Stats.DEF:
                    statValue = Def;
                    break;
                case Stats.CRITICALCHANCE:
                    statValue = CriticalChance;
                    break;
                case Stats.CRITICALDAMAGE:
                    statValue = CriticalDamage;
                    break;
                case Stats.DODGECHANCE:
                    statValue = DodgeChance;
                    break;

            }*/
            BuffStat[(int)stat] += value;
        }

        public virtual int GetEquipmentStatBonus(Stats stat) { return 0; }

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }
    }
}
