/// <summary>
/// 캐릭터 클래스
/// </summary>

using TeamTextRPG.Common;

namespace TeamTextRPG.Classes
{
    public class Character
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public int MaxMp { get; set; }
        public int CurrentMp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Exp { get; set; }
        public int Gold { get; set; }
        public int CriticalChance { get; set; } //0~100
        public int CriticalDamage { get; set; } //140% = 140
        public int DodgeChance { get; set; } // 0~100
        public List<Item> Inventory { get; set; }

        public int[] BuffStat = new int[Enum.GetValues(typeof(Stats)).Length];

        public virtual void ChangeHP(int hp)
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

        public int GetBuffStatBonus(Stats stat)
        {
            int bonus = 0;
            
            bonus += BuffStat[(int)stat];

            return bonus;
        }

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }

        public int GetStatValue(Stats stat)
        {
            switch (stat)
            {
                case Stats.MAXHP:
                    return MaxHp + GetEquipmentStatBonus(Stats.MAXHP) + GetBuffStatBonus(Stats.MAXHP);
                case Stats.MAXMP:
                    return MaxMp + GetBuffStatBonus(Stats.MAXMP);
                case Stats.ATK:
                    return Atk + GetEquipmentStatBonus(Stats.ATK) + GetBuffStatBonus(Stats.ATK);
                case Stats.DEF:
                    return Def + GetEquipmentStatBonus(Stats.DEF) + GetBuffStatBonus(Stats.DEF);
                case Stats.CRITICALCHANCE:
                    return CriticalChance + GetBuffStatBonus(Stats.CRITICALCHANCE);
                case Stats.CRITICALDAMAGE:
                    return CriticalDamage + GetBuffStatBonus(Stats.CRITICALDAMAGE);
                case Stats.DODGECHANCE:
                    return DodgeChance + GetEquipmentStatBonus(Stats.DODGECHANCE) + GetBuffStatBonus(Stats.DODGECHANCE);
            }
            return 0;
        }
    }
}
