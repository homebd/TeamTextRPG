using TeamTextRPG.Common;

namespace TeamTextRPG.Classes
{
    public class Skill
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ManaCost { get; set; }
        public SkillType Type { get; set; } // 아직 안 쓰는 중
        public Stats Stat { get; set; }
        public int Value { get; set; }
        public int Duration { get; set; }
        public Character Caster { get; set; }
        public Character Target { get; set; }

        public Skill(string name, string description, int manaCost, SkillType type, Stats stat, int value, int duration)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            Type = type;
            Stat = stat;
            Value = value;
            Duration = duration;
        }

        public Skill UseSkill(Character caster, Character target)
        {
            Skill skillToken = new Skill(Name, Description, ManaCost, Type, Stat, Value, Duration);
            skillToken.Caster = caster;
            skillToken.Target = target;

            skillToken.Caster.ChangeMP(-ManaCost);

            return skillToken;
        }

        public int DoSkill()
        {
            Duration--;

            switch(Type)
            {
                case SkillType.DAMAGE:
                    return (int)(GetStatValue() * Value / 100f);
                case SkillType.BUFF:
                    return Value;
            }

            return 0;
        }

        public int GetStatValue()
        {
            switch(Stat) {
                case Stats.MAXHP:
                    return Caster.MaxHp + Caster.GetEquipmentStatBonus(Stats.MAXHP);
                case Stats.MAXMP:
                    return Caster.MaxMp;
                case Stats.ATK:
                    return Caster.Atk + Caster.GetEquipmentStatBonus(Stats.ATK);
                case Stats.DEF:
                    return Caster.Def + Caster.GetEquipmentStatBonus(Stats.DEF);
                case Stats.CRITICALCHANCE:
                    return Caster.CriticalChance;
                case Stats.CRITICALDAMAGE:
                    return Caster.CriticalDamage;
                case Stats.DODGECHANCE:
                    return Caster.DodgeChance + Caster.GetEquipmentStatBonus(Stats.DODGECHANCE);
            }
            return 0;
        }
    }
}
