﻿/// <summary
/// Console에 직접적으로 보여지는 부분을 관리하는 클래스
/// </summary>

using TeamTextRPG.Classes;
using TeamTextRPG.Common;
using System.Text;
using static TeamTextRPG.Managers.SceneManager;
using System.Drawing;

namespace TeamTextRPG.Managers
{
    internal class UIManager
    {
        public GameManager GameManager;
        public Parts? Category { get; private set; }

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
            var currentCursor = Console.GetCursorPosition();
            var currentColor = Console.ForegroundColor;

            Console.SetCursorPosition(0, 0);
            Console.Write("                    ");
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = color;
            Console.WriteLine($"{title}");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
            Console.ForegroundColor = currentColor;
        }

        public void PrintItemCategories(bool forRefresh = false)
        {
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(0, _categoryTopPostion);

            Console.WriteLine("┌───────┐───────┐───────┐───────┐───────┐───────┐───────┐");
            Console.WriteLine("│  전체 │  무기 │  투구 │  갑옷 │  바지 │  신발 │  소모 │");
            switch (Category)
            {
                case null:
                    Console.Write("┘       └─────────────────────────────────────────────");
                    break;
                case Parts.WEAPON:
                    Console.Write("────────┘       └─────────────────────────────────────");
                    break;
                case Parts.HELMET:
                    Console.Write("────────────────┘       └─────────────────────────────");
                    break;
                case Parts.CHESTPLATE:
                    Console.Write("────────────────────────┘       └─────────────────────");
                    break;
                case Parts.LEGGINGS:
                    Console.Write("────────────────────────────────┘       └─────────────");
                    break;
                case Parts.BOOTS:
                    Console.Write("────────────────────────────────────────┘       └─────");
                    break;
                case Parts.USEABlE:
                    Console.Write("────────────────────────────────────────────────┘       └─");
                    break;
            }
            Console.Write("".PadRight(Console.WindowWidth - 58, '─'));
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

            int fillExpBar = (int)(8 * (float)player.Exp / (player.Level * player.Level * 100));
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
            var currentCursor = Console.GetCursorPosition();

            Console.SetCursorPosition(0, 1);
            Console.Write("".PadLeft(90, ' '));
            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"{description}\n\n");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
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
                    dm.SortItems(dm.Player.Inventory);
                    showPrice = false;
                    printNum = false;
                    break;
                case Scenes.INVENTORY_EQUIP:
                    dm.SortItems(dm.Player.Inventory);
                    showPrice = false;
                    printNum = true;
                    break;
                case Scenes.INVENTORY_SORT:
                    dm.SortItems(dm.Player.Inventory);
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
                    dm.SortItems(dm.Player.Inventory);
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
                    Category = (Parts)(Enum.GetValues(typeof(Parts)).Length - 1);
                else if (Category == (Parts)0)
                    Category = null;
                else Category--;

                return true;
            }
            else if (input == "]")
            {
                if (Category == null)
                    Category = (Parts)0;
                else if (Category == (Parts)(Enum.GetValues(typeof(Parts)).Length - 1))
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
                    if (tempLeft >= right - 5)
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

            int left = 92, top = 0, right = 120, bottom = 30;
            _logLeft = left + 2; _logTop = top + 2;

            MakeUIContainer(left, top, right, bottom);

            foreach (string log in Logs)
            {
                PrintLog(log);
            }

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);

        }

        public void AddLog(string log)
        {
            if (_logTop > 20)
            {
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

            // 줄바꿈
            int len = Encoding.Default.GetBytes(log).Length;
            if (len > 33)
            {
                for (int i = 1; i <= 25; i++)
                {
                    string str = log.Substring(log.Length - i, 1);

                    len -= Encoding.Default.GetBytes(str).Length;

                    if (len <= 33)
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

            if (equipments[(int)Parts.WEAPON] == null) name = "------- 없음 -------";
            else name = equipments[(int)Parts.WEAPON].Name;
            Console.SetCursorPosition(2, 9);
            Console.Write($"무기  {name}");

            if (equipments[(int)Parts.HELMET] == null) name = "------- 없음 -------";
            else name = equipments[(int)Parts.HELMET].Name;
            Console.SetCursorPosition(2, 11);
            Console.Write($"투구  {name}");

            if (equipments[(int)Parts.CHESTPLATE] == null) name = "------- 없음 -------";
            else name = equipments[(int)Parts.CHESTPLATE].Name;
            Console.SetCursorPosition(2, 13);
            Console.Write($"갑옷  {name}");

            if (equipments[(int)Parts.LEGGINGS] == null) name = "------- 없음 -------";
            else name = equipments[(int)Parts.LEGGINGS].Name;
            Console.SetCursorPosition(2, 15);
            Console.Write($"바지  {name}");

            if (equipments[(int)Parts.BOOTS] == null) name = "------- 없음 -------";
            else name = equipments[(int)Parts.BOOTS].Name;
            Console.SetCursorPosition(2, 17);
            Console.Write($"신발  {name}");


            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintStat()
        {
            var currentCursor = Console.GetCursorPosition();
            var player = GameManager.Instance.DataManager.Player;

            // 직업 표시
            string jop = "";
            switch (player.Job)
            {
                case JOB.WARRIOR:
                    jop = "전사";
                    break;
                case JOB.WIZARD:
                    jop = "마법사";
                    break;
                case JOB.ARCHER:
                    jop = "궁수";
                    break;

            }

            Console.SetCursorPosition(38, 6);
            Console.Write("< 현재 능력치 >");

            Console.SetCursorPosition(31, 9);
            Console.Write($"이  름  {player.Name}");

            Console.SetCursorPosition(31, 11);
            Console.Write($"직  업  {jop}");

            Console.SetCursorPosition(31, 13);
            Console.Write($"체  력  {player.CurrentHp} / {player.MaxHp}(+{player.GetEquipmentStatBonus(Stats.MAXHP)})");

            Console.SetCursorPosition(31, 15);
            Console.Write($"공격력  {player.Atk}(+{player.GetEquipmentStatBonus(Stats.ATK)})");

            Console.SetCursorPosition(31, 17);
            Console.Write($"방어력  {player.Def}(+{player.GetEquipmentStatBonus(Stats.DEF)})");

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

            dm.SortItems(dm.Player.Inventory);

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
            var player = GameManager.Instance.DataManager.Player;

            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion);
            Console.Write("┌─────────────────────────┐");
            Console.SetCursorPosition(_goldLeftPostion, _goldTopPostion + 1);
            Console.Write($"│ 방어력│ {(player.Def + player.GetEquipmentStatBonus(Stats.DEF)).ToString().PadLeft(16, ' ')}│");
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

            Console.SetCursorPosition(left + 3, 13);
            Console.Write($"회복량        {dm.Shelters[num].Healing} H");

            Console.SetCursorPosition(left + 3, 15);
            Console.Write($"수련량        {dm.Shelters[num].Refreshing} M");

            Console.SetCursorPosition(left + 3, 17);
            Console.Write($"비  용        {dm.Shelters[num].Cost} G");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintHp()
        {
            var currentCursor = Console.GetCursorPosition();
            var player = GameManager.Instance.DataManager.Player;

            int rate = 20;
            int fillExpBar = (int)(rate * (float)player.CurrentHp / (player.MaxHp + player.GetEquipmentStatBonus(Stats.MAXHP)) + 0.5f);
            if (fillExpBar >= rate) fillExpBar = rate;

            Console.SetCursorPosition(0, _goldTopPostion);
            Console.Write("┌──────┬──────────────────────────────────────────┐");
            Console.SetCursorPosition(0, _goldTopPostion + 1);
            Console.Write($"│ 체 력│  ");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("".PadRight(fillExpBar, '　'));
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("".PadRight(rate - fillExpBar, '　'));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("│");
            Console.SetCursorPosition(0, _goldTopPostion + 2);
            Console.Write("├──────┴──────────────────────────────────────────┴");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void PrintMp()
        {
            var currentCursor = Console.GetCursorPosition();
            var player = GameManager.Instance.DataManager.Player;

            int rate = 15;
            int fillExpBar = (int)(rate * (float)player.CurrentMp / player.MaxMp + player.GetEquipmentStatBonus(Stats.MAXMP) + 0.5f);
            if (fillExpBar >= rate) fillExpBar = rate;

            Console.SetCursorPosition(50, _goldTopPostion);
            Console.Write("┬──────┬────────────────────────────────┐");
            Console.SetCursorPosition(50, _goldTopPostion + 1);
            Console.Write($"│ 내 공│  ");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("".PadRight(fillExpBar, '　'));
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write("".PadRight(rate - fillExpBar, '　'));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("│");
            Console.SetCursorPosition(50, _goldTopPostion + 2);
            Console.Write("┴──────┴────────────────────────────────┤");

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void MakeBattleBox()
        {
            var currentCursor = Console.GetCursorPosition();

            int left = 0, top = 5, right = 92, bottom = 20;

            MakeUIContainer(left, top, right, bottom);

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);
        }

        public void ShowMonsterCard(List<Monster> monsters)
        {
            var currentCursor = Console.GetCursorPosition();
            int size = monsters.Count;
            int top = 6, bottom = 19;
            int width = 22;

            List<int> leftPosition = new List<int>();

            switch (size)
            {
                case 1:
                    leftPosition.Add(35);
                    break;
                case 2:
                    leftPosition.Add(12);
                    leftPosition.Add(59);
                    break;
                case 3:
                    leftPosition.Add(5);
                    leftPosition.Add(35);
                    leftPosition.Add(65);
                    break;
                case 4:
                    leftPosition.Add(1);
                    leftPosition.Add(24);
                    leftPosition.Add(47);
                    leftPosition.Add(70);
                    break;
            }

            foreach (int left in leftPosition)
            {
                // 몬스터 카드 틀 생성
                MakeUIContainer(left, top, left + width - 1, bottom);
            }

            for (int i = 0; i < size; i++)
            {
                // 몬스터 Info 출력
                Monster monster = monsters[i];

                Console.SetCursorPosition(leftPosition[i] + 9, top + 1);
                Console.Write($"Lv{monster.Level}");
                Console.SetCursorPosition(leftPosition[i] + 1, top + 2);
                int paddingSize = (19 - monster.Name.Length * 2) / 2;
                if (monster.Name.IndexOf(' ') > 0)
                    paddingSize++;
                Console.Write("".PadLeft(paddingSize,' ') + monster.Name + "".PadRight(paddingSize - 1, ' '));

                int fillHpBar = (int)(7 * (float)monster.CurrentHp / monster.MaxHp + 0.9f);
                if (fillHpBar >= 7) fillHpBar = 7;

                Console.SetCursorPosition(leftPosition[i] + 2, top + 11);
                Console.Write("HP ");
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("".PadRight(fillHpBar, '　'));
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("".PadRight(7 - fillHpBar, '　'));
                Console.BackgroundColor = ConsoleColor.Black;
            }

            Console.SetCursorPosition(currentCursor.Left, currentCursor.Top);

        }
    }
}
