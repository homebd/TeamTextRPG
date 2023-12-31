﻿/// <summary
/// 아이템이 가져야 할 속성들을 정의하는 클래스
/// </summary>

using TeamTextRPG.Common;

namespace TeamTextRPG.Classes
{
    public class Item
    {
        public string Name { get; }
        public int Id { get; private set; }
        public Parts Part { get; }
        public UsableItemTypes? UsableItemType { get; set; }
        public int Stat { get; }
        public string Description { get; }
        public int Price { get; }
        public int Level { get; set; }
        public int Stack { get; set; }
        public int BonusStat
        {
            get
            {
                return (int)(Stat * (Math.Pow(Level * 5, 1.5) / 100));
            }
        }

        public bool IsEquipped { get; set; }

        public Item(string name, int id, Parts part, int level, int stat, int price, string description, bool isEquipped = false, UsableItemTypes? usableItemType = null)
        {
            Name = name;
            Id = id;
            Part = part;
            Stat = stat;
            Description = description;
            Price = price;
            Level = level;
            IsEquipped = isEquipped;
            Stack = 1;
            UsableItemType = usableItemType;
        }

        public void PrintInfo(bool showPrice, int num = 0, float sale = 1)
        {
            string equip = (IsEquipped) ? "[E]" : "";
            string printNum = (num == 0) ? "" : $"{num} ";
            string level = (Level == 0) ? "" : $"(+{Level})";
            string bonus = (Level == 0) ? "" : $"(+{BonusStat})";

            string statByPart = "";
            switch (Part)
            {
                case Parts.WEAPON:
                    statByPart = "공격력";
                    break;
                case Parts.HELMET:
                    statByPart = "체  력";
                    break;
                case Parts.CHESTPLATE:
                    statByPart = "방어력";
                    break;
                case Parts.LEGGINGS:
                    statByPart = "치명률";
                    break;
                case Parts.BOOTS:
                    statByPart = "회피율";
                    break;
                case Parts.USEABLE:
                    switch (UsableItemType)
                    {
                        case UsableItemTypes.ATTACK_BUFF:
                            statByPart = "공격력";
                            break;
                        case UsableItemTypes.CRITICAL_CHANCE_BUFF:
                            statByPart = "치명률";
                            break;
                        case UsableItemTypes.CRITICAL_DAMAGE_BUFF:
                            statByPart = "치명타";
                            break;
                        case UsableItemTypes.DAMAGE:
                            statByPart = "데미지";
                            break;
                        case UsableItemTypes.DEFENCE_BUFF:
                            statByPart = "방어력";
                            break;
                        case UsableItemTypes.DODGE_CHANCE_BUFF:
                            statByPart = "회피율";
                            break;
                        case UsableItemTypes.HEAL_HP:
                            statByPart = "HP회복";
                            break;
                        case UsableItemTypes.HEAL_MP:
                            statByPart = "MP회복";
                            break;
                        default:
                            statByPart = "아이템";
                            break;
                    }
                    break;
            }
            if (Stack >= 2)
            {
                Console.Write($"- {printNum}{equip}{level}{Name} X {Stack}");
            }
            else
            {
                Console.Write($"- {printNum}{equip}{level}{Name}");
            }
            Console.SetCursorPosition(25, Console.GetCursorPosition().Top);
            Console.Write($"| {statByPart} + {Stat}{bonus}");
            Console.SetCursorPosition(45, Console.GetCursorPosition().Top);

            if(showPrice) Console.WriteLine($"| {((int)(Price * sale)).ToString().PadLeft(8, ' ')} G");
            else Console.Write($"| {Description}\n");
        }
       

        public void PrintInfoAtSmithy(int num)
        {
            string equip = (IsEquipped) ? "[E]" : "";
            string printNum = (num == 0) ? "" : $"{num} ";
            string level = (Level == 0) ? "" : $"(+{Level})";
            string bonus = (Level == 0) ? "" : $"(+{BonusStat})";

            string statByPart = "";
            switch (Part)
            {
                case Parts.WEAPON:
                    statByPart = "공격력";
                    break;
                case Parts.HELMET:
                    statByPart = "체  력";
                    break;
                case Parts.CHESTPLATE:
                    statByPart = "방어력";
                    break;
                case Parts.LEGGINGS:
                    statByPart = "치명률";
                    break;
                case Parts.BOOTS:
                    statByPart = "회피율";
                    break;

            }
            Console.Write($"- {printNum}{equip}{level}{Name}");
            Console.SetCursorPosition(25, Console.GetCursorPosition().Top);
            Console.Write($"| {statByPart} + {Stat}{bonus}");
            Console.SetCursorPosition(45, Console.GetCursorPosition().Top);

            int prb = (100 >> Level) + (100 >> (Level + 1));
            if (prb > 100) prb = 100;

            int cost = (int)((Price / 20) * Math.Pow(Level, 2));

            Console.WriteLine($"| 성공 확률: {prb.ToString().PadLeft(3, ' ')} %| 비용: {cost.ToString().PadLeft(10, ' ')} G");

           
        }
    }
}
