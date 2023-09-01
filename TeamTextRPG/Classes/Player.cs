/// <summary>
/// 플레이어 클래스
/// </summary>

using System.Runtime.ConstrainedExecution;
using System.Transactions;
using TeamTextRPG.Common;

namespace TeamTextRPG.Classes
{
    internal class Player : Character
    {
        public JOB Job { get; }
        public Item[]? Equipments { get; set; }
        public List<Skill> Skills { get; set; }
        public Dictionary<string,int> StatsPerLevel;

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
            StatsPerLevel = new Dictionary<string, int>();
        }

        // StatsPerLevel -> 초기설정 함수(reference 참고)
        public void SetStatsPerLevel(int addAtk, int addDef, int addMaxHp, int addMaxMp, int addCriticalChance,int addCriticalDamage, int addDodgeChance)
        {
            StatsPerLevel.Add("Atk", addAtk);
            StatsPerLevel.Add("Def", addDef);
            StatsPerLevel.Add("MaxHp", addMaxHp);
            StatsPerLevel.Add("MaxMp", addMaxMp);
            StatsPerLevel.Add("CriticalChance", addCriticalChance); 
            StatsPerLevel.Add("CriticalDamage", addCriticalDamage);
            StatsPerLevel.Add("DodgeChance", addDodgeChance);
        }

        public override int GetEquipmentStatBonus(Stats stat)
        {
            int bonus = 0;
            switch (stat)
            {
                case Stats.MAXHP:
                    if(Equipments[(int)Parts.HELMET] != null)
                    {
                        bonus += Equipments[(int)Parts.HELMET].Stat + Equipments[(int)Parts.HELMET].BonusStat;
                    }
                    break;
                case Stats.MAXMP:
                    break;
                case Stats.ATK:
                    if (Equipments[(int)Parts.WEAPON] != null) {
                        bonus += Equipments[(int)Parts.WEAPON].Stat + Equipments[(int)Parts.WEAPON].BonusStat;
                    }
                    break;
                case Stats.DEF:
                    if (Equipments[(int)Parts.CHESTPLATE] != null)
                    {
                        bonus += Equipments[(int)Parts.CHESTPLATE].Stat + Equipments[(int)Parts.CHESTPLATE].BonusStat;
                    }
                    break;
                case Stats.CRITICALCHANCE:
                    if (Equipments[(int)Parts.LEGGINGS] != null)
                    {
                        bonus += Equipments[(int)Parts.LEGGINGS].Stat + Equipments[(int)Parts.LEGGINGS].BonusStat;
                    }
                    break;
                case Stats.CRITICALDAMAGE:
                    break;
                case Stats.DODGECHANCE:
                    if (Equipments[(int)Parts.BOOTS] != null)
                    {
                        bonus += Equipments[(int)Parts.BOOTS].Stat + Equipments[(int)Parts.BOOTS].BonusStat;
                    }
                    break;
            }

            return bonus;
        }
        public void Wear(Item item)
        {
            if(item.Part == Parts.USEABLE)
            {
                ItemUse(item);
                return;
            }

            Equipments[(int)item.Part] = item;
            if (item.Part == Parts.HELMET)
            {
                ChangeHP(item.Stat + item.BonusStat);
                item.IsEquipped = true;
            }
            else
            {
                item.IsEquipped = true;
            }
        
        }
        public void ItemUse(Item item)
        {
            switch (item.UsableItemType)
            {
                case UsableItemTypes.ATTACK_BUFF:
                    ChangeStat(Stats.ATK, item.Stat);
                    break; 
                case UsableItemTypes.CRITICAL_CHANCE_BUFF:
                    ChangeStat(Stats.CRITICALCHANCE, item.Stat);
                    break;
                case UsableItemTypes.CRITICAL_DAMAGE_BUFF:
                    ChangeStat(Stats.CRITICALDAMAGE, item.Stat);
                    break;
                case UsableItemTypes.DAMAGE:
                    // 타겟 설정하고 배틀 매니저에서 진행할 수 있도록 해야 함.
                    break;
                case UsableItemTypes.DEFENCE_BUFF:
                    ChangeStat(Stats.DEF, item.Stat);
                    break;
                case UsableItemTypes.DODGE_CHANCE_BUFF:
                    ChangeStat(Stats.DODGECHANCE, item.Stat);
                    break;
                case UsableItemTypes.HEAL_HP:
                    ChangeHP(item.Stat);
                    break;
                case UsableItemTypes.HEAL_MP:
                    ChangeMP(item.Stat);
                    break;
                default:
                    break;

            }
            ItemStackRemove(item);
            if(item.Stack==0)
            {
                Inventory.Remove(item);
            }
            
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
        //소모성 아이템 스택 구현
        public void ItemStackAdd(Item item)
        {
            item.Stack++;
        }
        public void ItemStackRemove(Item item)
        {
            item.Stack--;
        }
        public int CheckStack(Item item)
        {
            return item.Stack;
        }

        // 레벨업 함수입니다 StatsPerLevel에 따라 각각의 스탯과 레벨을 증가 시킵니다.
        public void LevelUp()
        {
            Atk += StatsPerLevel["Atk"];
            Def += StatsPerLevel["Def"];
            MaxHp += StatsPerLevel["MaxHp"];
            MaxMp += StatsPerLevel["MaxMp"];
            CriticalChance += StatsPerLevel["CriticalChance"];
            CriticalDamage += StatsPerLevel["CriticalDamage"];
            DodgeChance += StatsPerLevel["DodgeChance"];
            ChangeHP(GetStatValue(Stats.MAXHP));
            ChangeMP(GetStatValue(Stats.MAXMP));
            Level++;
        }
        public void SetSkillList(dynamic data)
        {
            Skills.Add(data.Skill1.ToObject<Skill>());
            Skills.Add(data.Skill2.ToObject<Skill>());
            Skills.Add(data.Skill3.ToObject<Skill>());
            Skills.Add(data.Skill4.ToObject<Skill>());
        }
    }
}
