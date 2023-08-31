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
        public void SetStatsPerLevel(int addAtk, int addDef, int addMaxHp, int addMaxMp, int addCriticalChance, int addDodgeChance)
        {
            StatsPerLevel.Add("Atk", addAtk);
            StatsPerLevel.Add("Def", addDef);
            StatsPerLevel.Add("MaxHp", addMaxHp);
            StatsPerLevel.Add("MaxMp", addMaxMp);
            StatsPerLevel.Add("CriticalChance", addCriticalChance);
            StatsPerLevel.Add("DodgeChance", addDodgeChance);
        }

        public override void ChangeHP(int hp)
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

        public void ChangeMP(int mp)
        {
            var totalMp = MaxMp + GetEquipmentStatBonus(Stats.MAXMP);

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

        public void ChangeATk(int atk)
        {
            Atk += atk;
        }
      
        public override int GetEquipmentStatBonus(Stats stat)
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
            if(item.Part == Parts.USEABlE)
            {
                ItemUse(item);
            }
            if (item.Part == Parts.HELMET)
            {
                ChangeHP(item.Stat + item.BonusStat);
                item.IsEquipped = true;
            }
 
        }
        public void ItemUse(Item item)
        {

            //아이템 id 에 따라 다르게 작동하도록 한다. 
            switch (item.Id)
            {
                case 90:
                    //HP 회복
                    ChangeHP(item.Stat);
                    break; 
                case 91:
                    //MP 회복
                    ChangeMP(item.Stat);
                    break;
                case 92:
                    //공격력 증가
                   ChangeATk(item.Stat);
                    break;
                case 3:
                    break;
                case 4:
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
            DodgeChance += StatsPerLevel["DodgeChance"];
            Level++;
        }
    }
}
