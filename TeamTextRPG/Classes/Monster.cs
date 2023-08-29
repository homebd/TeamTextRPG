﻿namespace TeamTextRPG.Classes
{
    internal class Monster : Character
    {
        public int Id { get; }

        public Monster(string name, int id, int level, int atk, int def, int maxHp, int gold, int exp,
             float cc = 0.15f, float cd = 1.6f, float dc = 0.05f)
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
