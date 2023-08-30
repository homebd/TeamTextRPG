/// <summary
/// 플레이어 데이터 및 아이템과 던전 등의 데이터를 관리하는 클래스
/// </summary>

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TeamTextRPG.Classes;
using TeamTextRPG.Common;

namespace TeamTextRPG.Managers
{
    internal class DataManager
    {
        public GameManager GameManager;

        public Player Player { get; private set; }
        public List<Item> Shop { get; private set; }
        public List<Item> SortedItems { get; set; }
        public List<Dungeon> Dungeons { get; private set; }
        public List<Shelter> Shelters { get; private set; }
        public List<int> DiscoveredItem { get; set; }


        private List<Item> _items = new List<Item>();
        private List<Monster> _monsters = new List<Monster>();
        private string? _id; // 캐릭터 생성 시의 id
        public int MaxStage { get; set; }
        public int StagePage { get; set; }

        private string _savePath = @"../../../Data/Save";
        private string _dataPath = @"../../../Data";

        public DataManager()
        {
            SortedItems = new List<Item>();
            Shop = new List<Item>();
            Dungeons = new List<Dungeon>();
            Shelters = new List<Shelter>();
            DiscoveredItem = new List<int>();
            MaxStage = 1;
            StagePage = 0;
        }


        public void SaveData()
        {
            if (Player == null) return;

            using (FileStream fs = File.Open(_savePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    string playerJson = JsonConvert.SerializeObject(Player);
                    writer.WriteLine("{" + $"\"Player\": {playerJson}");

                    string discoveredItemJson = JsonConvert.SerializeObject(DiscoveredItem);
                    writer.WriteLine($", \"DiscoveredItem\": {discoveredItemJson}");

                    writer.WriteLine($", \"MaxStage\": {MaxStage}" + "}");
                }
            }
        }

        // 몬스터를 저장합니다. 여기서 데이터 만드는거 아니면 딱히 쓸 일은 없습니다.
        public void SaveMonster()
        {
            using (FileStream fs = File.Open(_dataPath + @"/Monsters.json", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    JArray monster = new JArray();
                    foreach (Monster m in _monsters)
                    {

                        int rewardItemId;
                        if (m.Inventory.Count > 0)
                            rewardItemId = m.Inventory.First().Id;
                        else
                            rewardItemId = -1;

                         monster.Add(new JObject(
                            new JProperty("Name", m.Name),
                            new JProperty("Id", m.Id),
                            new JProperty("Level", m.Level),
                            new JProperty("Atk", m.Atk),
                            new JProperty("Def", m.Def),
                            new JProperty("MaxHp", m.MaxHp),
                            new JProperty("Gold", m.Gold),
                            new JProperty("Exp", m.Exp),
                            new JProperty("RewardItemId", rewardItemId),
                            new JProperty("CriticalChance", m.CriticalChance),
                            new JProperty("CriticalDamage", m.CriticalDamage),
                            new JProperty("DodgeChance", m.DodgeChance)));
                    }
                    writer.WriteLine(monster);
                }
            }
        }
        public void LoadData()
        {
            string jsonContent;

            using (FileStream fs = File.Open(_savePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    jsonContent = reader.ReadToEnd();
                }
            }

            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            Player = data.Player.ToObject<Player>();
            DiscoveredItem = data.DiscoveredItem.ToObject<List<int>>();
            foreach (var id in DiscoveredItem)
            {
                Shop.Add(MakeNewItem((int)id));
            }
            MaxStage = data.MaxStage.ToObject<int>();
        }
        // 몬스터를 Json 파일에서 불러옵니다.
        public void LoadMonsters()
        {
            using (StreamReader sr = File.OpenText(_dataPath + @"/Monsters.json"))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(sr))
                {
                    JArray monsters = (JArray)JToken.ReadFrom(jsonReader);

                    for (int i = 0; i < monsters.Count; i++)
                    {
                        JObject monster = (JObject)monsters[i];
                        _monsters.Add(new Monster((string)monster["Name"], (int)monster["Id"], (int)monster["Level"], (int)monster["Atk"], (int)monster["Def"], (int)monster["MaxHp"], (int)monster["Gold"], (int)monster["Exp"], (int)monster["RewardItemId"], (int)monster["CriticalChance"], (int)monster["CriticalDamage"], (int)monster["DodgeChance"]));
                    }
                }
            }
        }
        // 아이템을 Json 파일에서 불러옵니다.
        public void LoadItems()
        {
            List<Item>? LoadedItems = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(_dataPath + @"/Items.json"));
            if (LoadedItems == null)
                return;
            _items.AddRange(LoadedItems);
        }

        public void BuyItem(Item item)
        {
            Player.Gold -= item.Price;
            Player.Inventory.Add(item);
            Shop.Remove(item);
            GameManager.Instance.UIManager.PrintGold();
            GameManager.Instance.UIManager.PrintItems();
        }

        public void SellItem(Item item)
        {
            Player.Gold += (int)(item.Price * 0.85f);

            item.Level = 0;
            Player.Inventory.Remove(item);
            if (!Shop.Exists(x => x.Name == item.Name)) Shop.Add(item);
            Shop = Shop.OrderBy(item => item.Id).ToList();
            GameManager.Instance.UIManager.PrintGold();
            GameManager.Instance.UIManager.PrintItems();
        }

        public void GameDataSetting()
        {
            LoadItems();
            LoadMonsters();

            #region 던전 세팅
            Dungeons.Add(new Dungeon(Player, "마을 동굴", 3, 300));
            Dungeons[0].AddMonster(0);
            Dungeons[0].AddMonster(2);
            Dungeons[0].AddMonster(3);
            Dungeons[0].AddMonster(4);
            Dungeons[0].AddMonster(6);
            Dungeons[0].AddReward(0);
            Dungeons[0].AddReward(10);
            Dungeons[0].AddReward(20);
            Dungeons[0].AddReward(30);
            Dungeons[0].AddReward(40);

            Dungeons.Add(new Dungeon(Player, "옆 마을", 5, 600));
            Dungeons[1].AddMonster(1);
            Dungeons[1].AddMonster(5);
            Dungeons[1].AddMonster(6);
            Dungeons[1].AddMonster(7);
            Dungeons[1].AddMonster(8);
            Dungeons[1].AddReward(1);
            Dungeons[1].AddReward(11);
            Dungeons[1].AddReward(21);
            Dungeons[1].AddReward(31);
            Dungeons[1].AddReward(41);

            Dungeons.Add(new Dungeon(Player, "대륙끝의 던전", 7, 1000));
            Dungeons[2].AddMonster(5);
            Dungeons[2].AddMonster(9);
            Dungeons[2].AddMonster(10);
            Dungeons[2].AddMonster(11);
            Dungeons[2].AddMonster(12);
            Dungeons[2].AddReward(2);
            Dungeons[2].AddReward(12);
            Dungeons[2].AddReward(22);
            Dungeons[2].AddReward(32);
            Dungeons[2].AddReward(42);

            Dungeons.Add(new Dungeon(Player, "대형 거미줄", 10, 1500));

            Dungeons[3].AddMonster(13);
            Dungeons[3].AddMonster(14);
            Dungeons[3].AddMonster(15);
            Dungeons[3].AddMonster(16);
            Dungeons[3].AddReward(3);
            Dungeons[3].AddReward(13);
            Dungeons[3].AddReward(23);
            Dungeons[3].AddReward(33);
            Dungeons[3].AddReward(43);

            Dungeons.Add(new Dungeon(Player, "초원 지대", 14, 2500));
            Dungeons[4].AddMonster(17);
            Dungeons[4].AddMonster(18);
            Dungeons[4].AddMonster(19);
            Dungeons[4].AddMonster(20);
            Dungeons[4].AddReward(4);
            Dungeons[4].AddReward(14);
            Dungeons[4].AddReward(24);
            Dungeons[4].AddReward(34);
            Dungeons[4].AddReward(44);
            Dungeons.Add(new Dungeon(Player, "곰의 절벽", 20, 4000));
            Dungeons[5].AddMonster(21);
            Dungeons[5].AddMonster(22);
            Dungeons[5].AddMonster(23);
            Dungeons[5].AddMonster(24);
            Dungeons[5].AddReward(5);
            Dungeons[5].AddReward(15);
            Dungeons[5].AddReward(25);
            Dungeons[5].AddReward(35);
            Dungeons[5].AddReward(45);
            Dungeons.Add(new Dungeon(Player, "지룡의 둥지", 26, 6000));
            Dungeons[6].AddMonster(25);
            Dungeons[6].AddMonster(26);
            Dungeons[6].AddMonster(27);
            Dungeons[6].AddMonster(28);
            Dungeons[6].AddReward(6);
            Dungeons[6].AddReward(16);
            Dungeons[6].AddReward(26);
            Dungeons[6].AddReward(36);
            Dungeons[6].AddReward(46);
            Dungeons.Add(new Dungeon(Player, "심연의 해구", 35, 9000));
            Dungeons[7].AddMonster(29);
            Dungeons[7].AddMonster(30);
            Dungeons[7].AddMonster(31);
            Dungeons[7].AddMonster(32);
            Dungeons[7].AddReward(7);
            Dungeons[7].AddReward(17);
            Dungeons[7].AddReward(27);
            Dungeons[7].AddReward(37);
            Dungeons[7].AddReward(47);
            Dungeons.Add(new Dungeon(Player, "달의 안개", 45, 13000));
            Dungeons[8].AddMonster(33);
            Dungeons[8].AddMonster(34);
            Dungeons[8].AddMonster(35);
            Dungeons[8].AddMonster(36);
            Dungeons[8].AddReward(8);
            Dungeons[8].AddReward(18);
            Dungeons[8].AddReward(28);
            Dungeons[8].AddReward(38);
            Dungeons[8].AddReward(48);
            Dungeons.Add(new Dungeon(Player, "격전지", 60, 25000));
            Dungeons[9].AddMonster(37);
            Dungeons[9].AddMonster(38);
            Dungeons[9].AddMonster(39);
            Dungeons[9].AddMonster(40);
            Dungeons[9].AddReward(9);
            Dungeons[9].AddReward(19);
            Dungeons[9].AddReward(29);
            Dungeons[9].AddReward(39);
            Dungeons[9].AddReward(49);
            #endregion

            #region 상점 세팅
            // 70 번부터 상점 아이템입니다.
            for (int i = 70; i < _items.Count; i++)
            {
                Shop.Add(MakeNewItem(i));
            }
            Shop = Shop.OrderBy(item => item.Id).ToList();
           
            #endregion

            #region 휴식 세팅
            Shelters.Add(new Shelter("약초 처방", 100, 300, 1500));
            Shelters.Add(new Shelter("전문 진료", 500, 1500, 7500));
            Shelters.Add(new Shelter("입원 치료", 1000, 3000, 15000));
            #endregion
        }

        public void SortItems(List<Item> itemList)
        {
            SortedItems.Clear();

            foreach (Item item in itemList)
            {
                if (item.Part == GameManager.Instance.UIManager.Category
                    || GameManager.Instance.UIManager.Category == null)
                {
                    SortedItems.Add(item);
                }
            }
        }

        public void SortInventory(int num)
        {
            switch (num)
            {
                case 1:
                    Player.Inventory = Player.Inventory.OrderBy(item => item.Name).ToList();
                    break;
                case 2:
                    Player.Inventory = Player.Inventory.OrderByDescending(item => item.Stat).ToList();
                    break;
                case 3:
                    Player.Inventory = Player.Inventory.OrderBy(item => item.Price).ToList();
                    break;
                case 4:
                    Player.Inventory = Player.Inventory.OrderByDescending(item => item.IsEquipped).ToList();
                    break;
                case 5:
                    Player.Inventory = Player.Inventory.OrderByDescending(item => item.Level).ToList();
                    break;
            }
        }

        public void ExploreDungeon(int num)
        {
            UIManager ui = GameManager.Instance.UIManager;
            int stage = num + StagePage;
            Dungeon dungeon = Dungeons[stage - 1];
            Random rnd = new Random();

            // 배틀 진입 후 결과 반환
            bool clear = GameManager.Instance.BattleManager.EntryBattle(dungeon);
            // ++++ 결과 정산도 battleManager에서 처리해도 될 것 같습니다. ++++ //

            // 배틀 진행 끝났다면 결과 정산합니다.

            int rewardGold = 0;
            int rewardExp = 0;
            // 죽은 몬스터에 따라 골드와 아이템, 경험치를 보상에 추가합니다.
            List<Item> rewardItems = new List<Item>();
            foreach (Monster m in GameManager.Instance.BattleManager.Monsters)
            {
                if (!m.IsDead())
                    continue;

                rewardGold += m.Gold;
                rewardExp += m.Exp;

                foreach (Item rewardItem in m.Inventory)
                {
                    if (rnd.NextDouble() < 0.1f)
                    {
                        rewardItems.Add(rewardItem);
                    }
                }
            }

            // 클리어를 하지 못했다면 잡은 몬스터의 보상을 주고 끝냅니다.
            if (!clear)
            {
                Player.Gold += rewardGold;
                Player.Exp += rewardExp;
                Player.Inventory.AddRange(rewardItems);
                // UI 로그에 출력합니다.
                PrintDungeonExploreResult(dungeon, clear, rewardGold, rewardExp, rewardItems);
                return;
            }
                
            // 던전 클리어 골드, 경험치, 아이템 보상을 추가합니다.
            if (stage == MaxStage) MaxStage++;
            rewardGold += (int)(dungeon.Reward[0]
                * (rnd.NextDouble() * 0.4 + 0.8));
            rewardExp += stage * stage * 10;
            for (int i = 1; i < dungeon.Reward.Count; i++)
            {
                if (rnd.NextDouble() < 0.1f)
                {
                    Item rewardItem = MakeNewItem(dungeon.Reward[i]);
                    Player.Inventory.Add(rewardItem);
                    rewardItems.Add(rewardItem);
                    if (!DiscoveredItem.Exists(x => x == rewardItem.Id))
                        DiscoveredItem.Add(rewardItem.Id);
                }
            }

            Player.Gold += rewardGold;
            Player.Exp += rewardExp;

            // UI 로그에 출력합니다.
            PrintDungeonExploreResult(dungeon, clear, rewardGold, rewardExp, rewardItems);
        }
        // 던전, 클리어여부, 보상 골드, 경험치, 아이템을 받아 UI 로그에 출력합니다.
        public void PrintDungeonExploreResult(Dungeon dungeon, bool clear, int rewardGold, int rewardExp, List<Item>rewardItems)
        {
            UIManager ui = GameManager.Instance.UIManager;
            if (clear)
                ui.AddLog($"{dungeon.Name} 클리어");
            else
                ui.AddLog($"{dungeon.Name} 탐험 실패");
            ui.AddLog($"골드  + {rewardGold} G");
            ui.AddLog($"경험치  + {rewardExp}");

            int levelThresholdExp = Player.Level * Player.Level * 100;
            if (Player.Exp >= levelThresholdExp)
            {
                Player.Exp -= levelThresholdExp;
                Player.Level++;
                Player.Atk++;
                Player.Def++;

                ui.AddLog("레벨이 올랐습니다.");
            }
            if (rewardItems.Count <= 0)
                return;

            ui.AddLog("");
            ui.AddLog("획득한 전리품 목록입니다.");
            foreach (Item item in rewardItems)
            {
                ui.AddLog(item.Name);
            }
        }
        
        public void RestPlayer(int num)
        {
            Shelter st = Shelters[num - 1];
            UIManager ui = GameManager.Instance.UIManager;

            if (Player.Gold >= st.Cost)
            {
                ui.AddLog(".");
                Thread.Sleep(500);
                ui.AddLog(".");
                Thread.Sleep(500);
                ui.AddLog(".");
                Thread.Sleep(500);

                var iHp = Player.CurrentHp;
                var iGold = Player.Gold;

                Player.Gold -= st.Cost;
                Player.ChangeHP(st.Healing);
                Player.ChangeMP(st.Refreshing);

                ui.AddLog($"{st.Name}(을)를 완료했습니다.");
                ui.AddLog($"체력 {iHp} -> {Player.CurrentHp}");
                if (Player.Gold / 1000000 > 0)
                {
                    ui.AddLog($"소지금 {iGold} ");
                    ui.AddLog($"-> {Player.Gold}");
                }
                else ui.AddLog($"소지금 {iGold} -> {Player.Gold}");
            }
            else
            {
                ui.AddLog("소지금이 부족합니다.");
            }

        } 

        public void CreateId()
        {
            UIManager ui = GameManager.Instance.UIManager;

            // 직업정보 읽기---------------
            string jobPath = @"../../../Data";
            jobPath = Path.Combine(jobPath, $"JobInfo.json");
            string jsonContent;

            using (FileStream fs = File.Open(jobPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    jsonContent = reader.ReadToEnd();
                }
            }

            dynamic data = JsonConvert.DeserializeObject(jsonContent);
            // ---------------------------
            while (true)
            {
                ui.SetCursorPositionForOption();
                _id = Console.ReadLine();
                if (_id != null)
                {
                    string savePath = Path.Combine(_savePath, $"{_id}.json");

                    if (File.Exists(savePath))
                    {
                        ui.AddLog("존재하는 ID입니다.");
                        ui.AddLog("ID 생성에 실패했습니다.");
                        return;
                    }
                    else
                    {
                        _savePath = savePath;
                        ui.AddLog("ID 생성에 성공했습니다.");
                        ui.AddLog("닉네임을 입력하세요.");
                        string? name;
                        while (true)
                        {
                            ui.SetCursorPositionForOption();
                            name = Console.ReadLine();
                            if (name != null)
                            {
                                break;
                            }
                            ui.AddLog("잘못된 입력입니다.");
                        }

                        // -------- Json으로 직업에 대한 정보 관리 및 접근성 확보 필요 ---------
                        ui.AddLog("닉네임 생성에 성공했습니다.");
                        ui.AddLog(" ");
                        ui.AddLog("직업을 선택해 주세요.");

                        List<string> option = new List<string>();
                        option.Add("1. 전사");
                        option.Add("2. 마법사");
                        option.Add("3. 궁수");
                        ui.MakeOptionBox(option);
                        while (true)
                        {
                            ui.SetCursorPositionForOption();
                            int job;
                            if(int.TryParse(Console.ReadLine(), out job))
                            {
                                if (job >= 1 && job <= (int)JOB.ARCHER + 1)
                                {
                                    // 인덱스로 교체
                                    job--;
                                    // 초기 스텟 설정
                                    Player = new Player(name, (JOB)(job), 1, (int)data["Atk"][(job)], (int)data["Def"][(job)], (int)data["MaxHp"][(job)], (int)data["MaxMp"][(job)], 1500);
                                    // 닷지 찬스 설정 (Player의 생성자 문제)
                                    Player.DodgeChance = (int)data["DodgeChance"][job];
                                    // 레벨 당 스탯 증가 수치 설정
                                    Player.SetStatsPerLevel((int)data["AddAtk"][job], (int)data["AddDef"][job], (int)data["AddMaxHp"][job], (int)data["AddMaxMp"][job], (int)data["AddCriticalChance"][job], (int)data["AddDodgeChance"][job]);
                                    GetBasicItem();
                                    SaveData();
                                    GameManager.Instance.SceneManager.Scene = Scenes.TOWN;
                                    ui.AddLog("게임을 시작합니다.");
                                    return;
                                }
                            }
                            ui.AddLog("잘못된 입력입니다.");
                        }
                        // -------------------------------------------------------------
                    }
                }
                ui.AddLog("잘못된 입력입니다.");
            }
        }

        public void LoginId()
        {
            UIManager ui = GameManager.Instance.UIManager;

            while (true)
            {
                ui.SetCursorPositionForOption();
                _id = Console.ReadLine();
                if (_id != null)
                {
                    string savePath = Path.Combine(_savePath, $"{_id}.json");

                    if (File.Exists(savePath))
                    {
                        _savePath = savePath;
                        GameManager.Instance.SceneManager.Scene = Scenes.TOWN;
                        LoadData();
                        ui.AddLog("로그인에 성공했습니다");
                        return;
                    }
                    else
                    {
                        ui.AddLog("존재하지 않는 ID입니다.");
                        ui.AddLog("로그인에 실패했습니다");
                        return;
                    }
                }
                ui.AddLog("잘못된 입력입니다.");
            }
        }

        private void GetBasicItem()
        {
            Player.Inventory.Add(MakeNewItem(0)); Shop.Remove(Shop.Find(x => x.Name == _items[0].Name));
            Player.Inventory.Add(MakeNewItem(10)); Shop.Remove(Shop.Find(x => x.Name == _items[10].Name));
            Player.Inventory.Add(MakeNewItem(20)); Shop.Remove(Shop.Find(x => x.Name == _items[20].Name));
            Player.Inventory.Add(MakeNewItem(30)); Shop.Remove(Shop.Find(x => x.Name == _items[30].Name));
            Player.Inventory.Add(MakeNewItem(40)); Shop.Remove(Shop.Find(x => x.Name == _items[40].Name));
        }

        public bool ShiftStagePage(string input)
        {
            var ui = GameManager.Instance.UIManager;

            if (input == "[")
            {
                if (StagePage == 0) {
                    Console.Beep();
                    ui.AddLog("가장 첫 페이지입니다.");
                }
                else
                {
                    StagePage--;
                }

                return true;
            }
            else if (input == "]")
            {
                if (StagePage == Dungeons.Count - 3)
                {
                    Console.Beep();
                    ui.AddLog("마지막 페이지입니다.");
                }
                else
                {
                    StagePage++;
                }

                return true;
            }

            return false;
        }

        public void StrengthenItem(Item item)
        {
            var ui = GameManager.Instance.UIManager;
            if (Player.Gold >= item.Price * (6 << item.Level) / 100)
            {
                Player.Gold -= item.Price * (6 << item.Level) / 100;

                Random rnd = new Random();
                if (rnd.Next(0, 100) < (100 >> item.Level) + (100 >> item.Level + 1))
                {
                    ui.AddLog("강화에 성공했습니다!");
                    ui.AddLog($"{item.Level++} -> {item.Level}");
                }
                else if (rnd.Next(0, 100) < 50)
                {
                    ui.AddLog("강화에 실패했습니다!");
                    ui.AddLog("강화 레벨은 유지됩니다.");
                }
                else if (rnd.Next(0, 100) < 80)
                {
                    var iLevel = item.Level;

                    if (item.Level != 0)
                        item.Level--;

                    ui.AddLog("강화에 실패했습니다!");
                    ui.AddLog($"{iLevel} -> {item.Level}");
                }
                else
                {
                    if (Player.Equipments[(int)item.Part] == item)
                    {
                        Player.Unwear(item.Part);
                    }

                    item.Level = 0;
                    Player.Inventory.Remove(item);
                    if(!Shop.Exists(x => x.Name == item.Name)) Shop.Add(item);

                    ui.AddLog($"강화에 실패하여 {item.Name}(이)가 파괴되었습니다");
                }

            }
            else
            {
                ui.AddLog("소지금이 부족합니다.");
            }
        }

        public Item MakeNewItem(int id)
        {
            Item item = _items[id];
            Item newItem = new Item(item.Name, item.Id, item.Part, item.Level, item.Stat, item.Price, item.Description);

            return newItem;
        }

        public Item MakeNewItem(int id, int level)
        {
            Item item = _items[id];
            Item newItem = new Item(item.Name, item.Id, item.Part, level, item.Stat, item.Price, item.Description);

            return newItem;
        }

        public Monster MakeNewMonster(int id)
        {
            Monster monster = _monsters[id];
            Monster newMonster = new Monster(monster.Name, monster.Id, monster.Level, monster.Atk, monster.Def, monster.MaxHp, monster.Gold, monster.Exp);

            foreach(var reward in monster.Inventory)
            {
                newMonster.Inventory.Add(reward);
            }
            return newMonster;
        }
    }
}
