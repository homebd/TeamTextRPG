using Newtonsoft.Json;
using TeamTextRPG.Common;

namespace TeamTextRPG.Classes
{
    public class Skill
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ManaCost { get; set; }
        public SkillType SkillType { get; set; }
        public ValueTypeEnum ValueType { get; set; }
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
            SkillType = type;
            Stat = stat;
            Value = value;
            Duration = duration;
            ValueType = ValueTypeEnum.PROPOTIONAL;
        }

        public Skill(string name, string description, int manaCost, SkillType type, int value, int duration)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            SkillType = type;
            Value = value;
            Duration = duration;
            ValueType = ValueTypeEnum.FIXED;
        }

        public Skill UseSkill(Character caster, Character target)
        {
            Skill skillToken = new Skill(Name, Description, ManaCost, SkillType, Stat, Value, Duration);
            skillToken.Caster = caster;
            skillToken.Target = target;

            skillToken.Caster.ChangeMP(-ManaCost);

            return skillToken;
        }

        [JsonConstructor]
        public Skill()
        {

        }

        public int DoSkill()
        {
            Duration--;
            int value = 0;

            switch (ValueType)
            {
                case ValueTypeEnum.PROPOTIONAL:
                    value = (int)(Caster.GetStatValue(Stat) * Value / 100f);
                    break;
                case ValueTypeEnum.FIXED:
                    value = Value;
                    break;
            }

            return value;
        }
    }
}
