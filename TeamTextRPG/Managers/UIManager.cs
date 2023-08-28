/// <summary
/// Console에 직접적으로 보여지는 부분을 관리하는 클래스
/// </summary>

using TeamTextRPG.Classes;
using System.Text;
using static TeamTextRPG.Managers.SceneManager;

namespace TeamTextRPG.Managers
{
    internal class UIManager
    {   
        public GameManager GameManager;
        public Item.Parts? Category { get; private set; }

        private int _itemsTopPostion = 7;

        private int _categoryTopPostion = 3;

        private int _goldLeftPostion = 64;

        private int _goldTopPostion = 3;

        public List<string> Logs;


        private int _logLeft, _logTop;

        public UIManager()
        {
            Console.Title = "스파르타 RPG";
            Console.SetWindowSize(120, 30);

            Logs = new List<string>();
        }

        public void ClearLine()
        {
            var currentCursorLeft = Console.GetCursorPosition();

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(currentCursorLeft.Left, currentCursorLeft.Top);
        }

        public void PrintTitle(string title, ConsoleColor color = ConsoleColor.DarkRed)
        {
            Console.Clear();

            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{title}]");
            Console.ForegroundColor = currentColor;
        }

        public void PrintItemCategories(bool forRefresh = false)
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(0, _categoryTopPostion);

            Console.WriteLine("┌───────┐───────┐───────┐───────┐───────┐───────┐");
            Console.WriteLine("│  전체 │  무기 │  투구 │  갑옷 │  바지 │  신발 │");
            switch (Category)
            {
                case null:
                    Console.Write("┘       └─────────────────────────────────────────────");
                    break;
                case Item.Parts.WEAPON:
                    Console.Write("────────┘       └─────────────────────────────────────");
                    break;
                case Item.Parts.HELMET:
                    Console.Write("────────────────┘       └─────────────────────────────");
                    break;
                case Item.Parts.CHESTPLATE:
                    Console.Write("────────────────────────┘       └─────────────────────");
                    break;
                case Item.Parts.LEGGINGS:
                    Console.Write("────────────────────────────────┘       └─────────────");
                    break;
                case Item.Parts.BOOTS:
                    Console.Write("────────────────────────────────────────┘       └─────");
                    break;
            }
            Console.Write("".PadRight(Console.WindowWidth - 54, '─'));
            Console.Write("\n\n");

            if (forRefresh) Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintGold()
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion);
            Console.Write("┌─────────────────────────┐");
            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion + 1);
            Console.Write($"│ 소지금│ {GameManager.Instance.DataManager.Player.Gold.ToString().PadLeft(14, ' ')} G│");
            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion + 2);
            Console.Write("┴─────────────────────────┴");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintLevel()
        {
            var currentCursor = Console.GetCursorPosition();
            var player = GameManager.Instance.DataManager.Player;

            int fillExpBar =  (int)(8 * (float)player.Exp / player.Level + 0.5f);
            if (fillExpBar >= 8) fillExpBar = 8;

            Console.SetCursorPosition(0, _goldTopPostion);
            Console.Write("┌───────┬─────────────────┐");
            Console.SetCursorPosition(0, _goldTopPostion + 1);
            Console.Write($"│ Lv {player.Level.ToString().PadLeft(3, ' ')}│ ");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("".PadRight(fillExpBar, '　'));
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("".PadRight(8 - fillExpBar, '　'));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("│");
            Console.SetCursorPosition(0, _goldTopPostion + 2);
            Console.Write("├───────┴─────────────────┴");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintDescription(string description)
        {
            Console.Write($"{description}\n\n");
        }

        public void PrintItems()
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(0, _itemsTopPostion);

            DataManager dm = GameManager.Instance.DataManager;
            bool showPrice = false, printNum = false;
            float sale = 1;
            switch (GameManager.Instance.SceneManager.Scene)
            {
                case Scenes.INVENTORY_MAIN:
                    dm.SortItems(dm.Inventory);
                    showPrice = false;
                    printNum = false;
                    break;
                case Scenes.INVENTORY_EQUIP:
                    dm.SortItems(dm.Inventory);
                    showPrice = false;
                    printNum = true;
                    break;
                case Scenes.INVENTORY_SORT:
                    dm.SortItems(dm.Inventory);
                    showPrice = false;
                    printNum = false;
                    break;
                case Scenes.SHOP_MAIN:
                    dm.SortItems(dm.Shop);
                    showPrice = true;
                    printNum = false;
                    break;
                case Scenes.SHOP_BUY:
                    dm.SortItems(dm.Shop);
                    showPrice = true;
                    printNum = true;
                    break;
                case Scenes.SHOP_SELL:
                    dm.SortItems(dm.Inventory);
                    showPrice = true;
                    printNum = true;
                    sale = 0.85f;
                    break;
            }

            for (int i = 0; i < dm.SortedItems.Count && i < 12; i++)
            {
                var item = dm.SortedItems[i];

                ClearLine();
                if (printNum) item.PrintInfo(showPrice, i + 1, sale);
                else item.PrintInfo(showPrice);
            }

            ClearLine();

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public bool ShiftCategory(string input)
        {
            if (input == "[")
            {
                if (Category == null)
                    Category = (Item.Parts)(Enum.GetValues(typeof(Item.Parts)).Length - 1);
                else if (Category == (Item.Parts)0)
                    Category = null;
                else Category--;

                return true;
            }
            else if (input == "]")
            {
                if (Category == null)
                    Category = (Item.Parts)0;
                else if (Category == (Item.Parts)(Enum.GetValues(typeof(Item.Parts)).Length - 1))
                    Category = null;
                else Category++;

                return true;
            }

            return false;
        }

        private void MakeUIContainer(int left, int top, int right, int bottom)
        {   //┌ ─ ┐ └ ┘ │
            if (left < 0 || top < 0 || --right > Console.WindowWidth || --bottom > Console.WindowHeight) return;

            Console.SetCursorPosition(left, top);
            Console.Write("┌".PadRight(right - left - 1, '─'));
            Console.Write("┐");

            for (int i = top + 1; i < bottom; i++)
            {
                Console.SetCursorPosition(left, i);
                Console.Write("│".PadRight(right - left - 1, ' '));

                Console.SetCursorPosition(right - 1, i);
                Console.Write("│");
            }
            

            Console.SetCursorPosition(left, bottom);
            Console.Write("└".PadRight(right - left - 1, '─'));
            Console.Write("┘");
        }

        public void MakeOptionBox(List<string>? option)
        {
            var currentCursor = Console.GetCursorPosition();

            int left = 0, top = 20, right = 92, bottom = 30;

            MakeUIContainer(left, top, right, bottom);

            if (option != null)
            {
                int tempLeft = left, tempTop = top;

                for (int i = 0; i < option.Count; i++)
                {
                    Console.SetCursorPosition(tempLeft + 2, tempTop + 2);
                    Console.Write(option[i].ToString());

                    tempLeft += right / 3;
                    if(tempLeft >= right - 5)
                    {
                        tempLeft = left;
                        tempTop += 2;
                    }

                }
            }

            Console.SetCursorPosition(left + 2, bottom - 4);
            Console.Write("".PadRight(right - left - 4, '-'));

            Console.SetCursorPosition(left + 2, bottom - 2);
            Console.Write(">> ");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);

        }

        public void SetCursorPositionForOption()
        {
            Console.SetCursorPosition(5, 28);
            Console.Write("                      ");
            Console.SetCursorPosition(5, 28);
        }

        public void MakeLogBox()
        {
            var currentCursor = Console.GetCursorPosition();

            int left = 92, top = 0, right = 120, bottom = 23;
            _logLeft = left + 2;  _logTop = top + 2;

            MakeUIContainer(left, top, right, bottom);

            foreach (string log in Logs)
            {
                PrintLog(log);
            }

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top); MakePlayerBox();

        }

        public void AddLog(string log)
        {
            if (_logTop > 20) {
                Logs.RemoveRange(0, 5);
                MakeLogBox();
            }

            Logs.Add(log);
            PrintLog(log);
        }

        public void PrintLog(string log)
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(_logLeft, _logTop);

            int len = Encoding.Default.GetBytes(log).Length;
            if (len > 33)
            {
                for(int i = 1; i <= 25; i++)
                {
                    string str = log.Substring(log.Length - i, 1);

                    len -= Encoding.Default.GetBytes(str).Length;

                    if(len <= 33)
                    {
                        Console.Write(log.Substring(0, log.Length - i));

                        Console.SetCursorPosition(_logLeft, ++_logTop);
                        Console.Write(log.Substring(log.Length - i, i));
                        break;
                    }
                }
            }
            else
            {
                Console.Write(log);
            }

            _logTop += 2;

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }
        
        public void ClearLog()
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(_logLeft, _logTop);

            Logs.Clear();
            MakeLogBox();

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void MakeStatusBox()
        {
            var currentCursor = Console.GetCursorPosition();

            int left = 0, top = 5, right = 92, bottom = 20;

            MakeUIContainer(left, top, 31, bottom);
            MakeUIContainer(29, top, 62, bottom);
            MakeUIContainer(60, top, right, bottom);

            Console.SetCursorPosition(_goldLeftPostion, top);
            Console.Write("┴");
            Console.SetCursorPosition(right - 2, top);
            Console.Write("┤");

            PrintEquipments();
            PrintStat();

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintEquipments()
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(9, 6);
            Console.Write("< 현재 장비 >");

            var equipments = GameManager.Instance.DataManager.Player.Equipments;
            string name;

            if (equipments[(int)Item.Parts.WEAPON] == null) name = "------- 없음 -------";
            else name = equipments[(int)Item.Parts.WEAPON].Name;
            Console.SetCursorPosition(2, 9);
            Console.Write($"무기  {name}");

            if (equipments[(int)Item.Parts.HELMET] == null) name = "------- 없음 -------";
            else name = equipments[(int)Item.Parts.HELMET].Name;
            Console.SetCursorPosition(2, 11);
            Console.Write($"투구  {name}");

            if (equipments[(int)Item.Parts.CHESTPLATE] == null) name = "------- 없음 -------";
            else name = equipments[(int)Item.Parts.CHESTPLATE].Name;
            Console.SetCursorPosition(2, 13);
            Console.Write($"갑옷  {name}");

            if (equipments[(int)Item.Parts.LEGGINGS] == null) name = "------- 없음 -------";
            else name = equipments[(int)Item.Parts.LEGGINGS].Name;
            Console.SetCursorPosition(2, 15);
            Console.Write($"바지  {name}");

            if (equipments[(int)Item.Parts.BOOTS] == null) name = "------- 없음 -------";
            else name = equipments[(int)Item.Parts.BOOTS].Name;
            Console.SetCursorPosition(2, 17);
            Console.Write($"신발  {name}");


            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintStat()
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;

            // 직업 표시
            string jop = "";
            switch(dm.Player.Job)
            {
                case JOP.WARRIOR:
                    jop = "전사";
                    break;
                case JOP.WIZARD:
                    jop = "마법사";
                    break;
                case JOP.ARCHER:
                    jop = "궁수";
                    break;

            }

            Console.SetCursorPosition(38, 6);
            Console.Write("< 현재 능력치 >");

            Console.SetCursorPosition(31, 9);
            Console.Write($"이  름  {dm.Player.Name}");

            Console.SetCursorPosition(31, 11);
            Console.Write($"직  업  {jop}");

            Console.SetCursorPosition(31, 13);
            Console.Write($"체  력  {dm.Player.CurrentHp} / {dm.Player.MaxHp}(+{dm.GetHpBonus()})");

            Console.SetCursorPosition(31, 15);
            Console.Write($"공격력  {dm.Player.Atk}(+{dm.GetAtkBonus()})");

            Console.SetCursorPosition(31, 17);
            Console.Write($"방어력  {dm.Player.Def}(+{dm.GetDefBonus()})");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void MakeDungeonBox()
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;

            int left = 0, top = 5, right = 92, bottom = 20;

            MakeUIContainer(left, top, 32, bottom);
            MakeUIContainer(30, top, 62, bottom);
            MakeUIContainer(60, top, right, bottom);

            for (int i = 0; i < 3; i++)
            {
                PrintDungeon(i);
                if (dm.StagePage + i + 1 > dm.MaxStage) PrintLockedDungeon(i);
            }

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintDungeon(int num)
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;
            int stage = num + dm.StagePage;

            int left = 2 + (30 * (num));

            Console.SetCursorPosition(left + 11, 6);
            Console.Write($"< {num + 1} >");

            Console.SetCursorPosition(left, 9);
            Console.Write($"이  름  {dm.Dungeons[stage].Name}");

            Console.SetCursorPosition(left, 13);
            Console.Write($"권  장  방어력 {dm.Dungeons[stage].Condition} 이상");

            Console.SetCursorPosition(left, 17);
            Console.Write($"보  상  {dm.Dungeons[stage].Reward[0].ToString().PadLeft(4, ' ')} G");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintLockedDungeon(int num)
        {
            var currentCursor = Console.GetCursorPosition();
            var currentForgroundColor = Console.ForegroundColor;

            int left = 2 + (30 * (num));

            Console.SetCursorPosition(left + 10, 6);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"< 잠김 >");
            Console.ForegroundColor = currentForgroundColor;

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintItemsAtSmithy()
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(0, _itemsTopPostion);

            DataManager dm = GameManager.Instance.DataManager;

            dm.SortItems(dm.Inventory);

            for (int i = 0; i < dm.SortedItems.Count && i < 12; i++)
            {
                ClearLine();
                dm.SortedItems[i].PrintInfoAtSmithy(i + 1);
            }

            ClearLine();

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintDef()
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;

            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion);
            Console.Write("┌─────────────────────────┐");
            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion + 1);
            Console.Write($"│ 방어력│ {(dm.Player.Def + dm.GetDefBonus()).ToString().PadLeft(16, ' ')}│");
            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion + 2);
            Console.Write("┴─────────────────────────┤");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void MakeShelterBox()
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;

            int left = 0, top = 5, right = 92, bottom = 20;

            MakeUIContainer(left, top, 32, bottom);
            MakeUIContainer(30, top, 62, bottom);
            MakeUIContainer(60, top, right, bottom);

            Console.SetCursorPosition(_goldLeftPostion, top);
            Console.Write("┴");
            Console.SetCursorPosition(right - 2, top);
            Console.Write("┤");

            for (int i = 0; i < 3; i++)
            {
                PrintShelter(i);
            }

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintShelter(int num)
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;

            int left = 2 + (30 * (num));

            Console.SetCursorPosition(left + 11, 6);
            Console.Write($"< {num + 1} >");

            Console.SetCursorPosition(left + 9, 9);
            Console.Write($"{dm.Shelters[num].Name}");

            Console.SetCursorPosition(left+3, 14);
            Console.Write($"회복량        {dm.Shelters[num].Heal} H");

            Console.SetCursorPosition(left+3, 17);
            Console.Write($"비  용        {dm.Shelters[num].Cost} G");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintHp()
        {
            var currentCursor = Console.GetCursorPosition();
            var dm = GameManager.Instance.DataManager;

            Console.SetCursorPosition(0, _goldTopPostion);
            Console.Write("┌─────────────────────────┐");
            Console.SetCursorPosition(0, _goldTopPostion + 1);
            Console.Write($"│ 체  력│ {dm.Player.CurrentHp.ToString().PadLeft(6, ' ')}  / {(dm.Player.MaxHp + dm.GetHpBonus()).ToString().PadLeft(6, ' ')}│");
            Console.SetCursorPosition(0, _goldTopPostion + 2);
            Console.Write("├─────────────────────────┴");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void MakePlayerBox()
        {
            var currentCursor = Console.GetCursorPosition();

            int left = 92, top = 23, right = 120, bottom = 30;

            MakeUIContainer(left, top, right, bottom);

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

    }
}
