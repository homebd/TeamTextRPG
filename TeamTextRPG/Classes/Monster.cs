﻿using TeamTextRPG.Managers;

namespace TeamTextRPG.Classes
{
    internal class Monster : Character
    {
        public int Id { get; }

        public Monster(string name, int id, int level, int atk, int def, int maxHp, int gold, int exp, int rewardItemId = -1,
             int cc = 10, int cd = 160, int dc = 5)
        {
            Name = name;
            Id = id;
            Level = level;
            Atk = atk;
            Def = def;
            MaxHp = maxHp;
            CurrentHp = MaxHp;
            Gold = gold;
            Exp = exp;
            CriticalChance = cc;
            CriticalDamage = cd;
            DodgeChance = dc;

            Inventory = new List<Item>();
            if (rewardItemId > -1)
            {
                Inventory.Add(GameManager.Instance.DataManager.MakeNewItem(rewardItemId));
                
            }
        }
    }
}
