namespace TeamTextRPG.Classes
{
    internal class Monster
    {
        public string Name { get; }
        public int Id { get; }
        public int Level { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MaxHp { get; }
        public float CurrentHp { get; private set; }
        public List<int> Reward { get; } // Reward[0]은 골드, Reward[1]부터는 아이템 id
        public int RewardExp { get; }

        public Monster(string name, int id, int level, int atk, int def, int maxHp, int gold, int exp, int rewardItemId = -1)
        {
            Name = name;
            Id = id;
            Level = level;
            Atk = atk;
            Def = def;
            MaxHp = maxHp;
            CurrentHp = MaxHp;
            Reward = new List<int>();
            Reward.Add(gold);
            RewardExp = exp;
            if (rewardItemId > -1)
            {
                Reward.Add(rewardItemId);
            }
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
            

        public bool IsDead()
        {
            return CurrentHp == 0;
        }
    }
}
