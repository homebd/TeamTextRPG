/// <summary
/// 아이템이 가져야 할 속성들을 정의하는 클래스
/// </summary>

namespace TeamTextRPG.Classes
{
    internal class Item
    {
        public enum Parts
        {
            WEAPON,
            HELMET,
            CHESTPLATE,
            LEGGINGS,
            BOOTS
        }
        public string Name { get; }
        public int Id { get; }
        public Parts Part { get; }
        public int Stat { get; }
        public string Description { get; }
        public int Price { get; }
        public int Level { get; set; }
        public bool IsEquipped { get; set; }
        public int BonusStat
        {
            get
            {
                return Stat * 2 * Level * Level / 100;
            }
        }

        public Item(string name, int id, Parts part, int level, int stat, int price, string description, bool isEquipped = false)
        {
            Name = name;
            Id = id;
            Part = part;
            Stat = stat;
            Description = description;
            Price = price;
            Level = level;
            IsEquipped = isEquipped;
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
                case Parts.BOOTS:
                    statByPart = "체  력";
                    break;
                case Parts.CHESTPLATE:
                case Parts.LEGGINGS:
                    statByPart = "방어력";
                    break;
            }

            Console.Write($"- {printNum}{equip}{level}{Name}");
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
                case Parts.BOOTS:
                    statByPart = "체  력";
                    break;
                case Parts.CHESTPLATE:
                case Parts.LEGGINGS:
                    statByPart = "방어력";
                    break;
            }

            Console.Write($"- {printNum}{equip}{level}{Name}");
            Console.SetCursorPosition(25, Console.GetCursorPosition().Top);
            Console.Write($"| {statByPart} + {Stat}{bonus}");
            Console.SetCursorPosition(45, Console.GetCursorPosition().Top);

            int prb = (100 >> Level) + (100 >> (Level + 1));
            if (prb > 100) prb = 100;

            int cost = Price * (6 << Level) / 100;

            Console.WriteLine($"| 성공 확률: {prb.ToString().PadLeft(3, ' ')} %| 비용: {cost.ToString().PadLeft(10, ' ')} G");
             
        }
    }
}
