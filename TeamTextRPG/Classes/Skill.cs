using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public bool IsAoE { get; set; }
        public Character Caster { get; set; }
        public Character Target { get; set; }

        public Skill(string name, string description, int manaCost, SkillType type, Stats stat, int value, int duration, bool isAoE)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            SkillType = type;
            Stat = stat;
            Value = value;
            Duration = duration;
            ValueType = ValueTypeEnum.PROPOTIONAL;
            IsAoE = isAoE;
        }

        public Skill(string name, string description, int manaCost, SkillType type, int value, int duration, bool isAoE)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            SkillType = type;
            Value = value;
            Duration = duration;
            ValueType = ValueTypeEnum.FIXED;
            IsAoE = isAoE;
        }

        public Skill UseSkill(Character caster, Character target)
        {
            Skill skillToken = null;

            switch (ValueType)
            {
                case ValueTypeEnum.PROPOTIONAL:
                    skillToken = new Skill(Name, Description, ManaCost, SkillType, Stat, Value, Duration, IsAoE);
                    break;
                case ValueTypeEnum.FIXED:
                    skillToken = new Skill(Name, Description, ManaCost, SkillType, Value, Duration, IsAoE);
                    break;
            }

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
