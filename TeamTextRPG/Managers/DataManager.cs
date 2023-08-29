/// <summary
/// 플레이어 데이터 및 아이템과 던전 등의 데이터를 관리하는 클래스
/// </summary>

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TeamTextRPG.Classes;
using static TeamTextRPG.Managers.SceneManager;

namespace TeamTextRPG.Managers
{
    internal class DataManager
    {
        public GameManager GameManager;

        public Character Player { get; private set; }
        public List<Item> Inventory { get; private set; }
        public List<Item> Shop { get; private set; }
        public List<Item> SortedItems { get; set; }
        public List<Shelter> Shelters { get; private set; }
        public List<int> DiscoveredItem { get; set; }


        private Item[] _items = new Item[60];
        private Dungeon[] _dungeons = new Dungeon[10];
        private List<Monster> _monsters = new List<Monster>();
        private string? _id; // 캐릭터 생성 시의 id
        public int MaxStage { get; set; }
        public int StagePage { get; set; }

        private string _savePath = @"../../../Save";
        private string _itemPath = @"../../../Data\\Items.json";
        private string _dungeonPath = @"../../../Data\\Dungeons.Json";
        private string _monsterPath = @"../../../Data\\Monsters.Json";

        public DataManager()
        {
            SortedItems = new List<Item>();
            Inventory = new List<Item>();
            Shop = new List<Item>();
            Shelters = new List<Shelter>();
            DiscoveredItem = new List<int>();
            MaxStage = 1;
            StagePage = 0;
        }


        public void SaveData()
        {
            if (Player == null) return;

            JObject configData = new JObject(
                new JProperty("Name", Player.Name),
                new JProperty("Job", (int)Player.Job),
                new JProperty("Level", Player.Level),
                new JProperty("Exp", Player.Exp),
                new JProperty("Atk", Player.Atk),
                new JProperty("Def", Player.Def),
                new JProperty("MaxHp", Player.MaxHp),
                new JProperty("CurrentHp", Player.CurrentHp),
                new JProperty("Gold", Player.Gold)
                );

            if(Inventory.Count > 0)
            {
                int[] inventoryIds = new int[Inventory.Count];
                int[] itemsLevel = new int[Inventory.Count];

                for (int i = 0; i < Inventory.Count; i++)
                {
                    inventoryIds[i] = Inventory[i].Id;
                    itemsLevel[i] = Inventory[i].Level;
                }
                configData.Add(new JProperty("Inventory", JArray.FromObject(inventoryIds)));
                configData.Add(new JProperty("ItemLevel", JArray.FromObject(itemsLevel)));
            }
            
            if(DiscoveredItem.Count > 0)
            {
                int[] discoveredIds = new int[DiscoveredItem.Count];

                for (int i = 0; i < DiscoveredItem.Count; i++)
                {
                    discoveredIds[i] = DiscoveredItem[i];
                }

                configData.Add(new JProperty("Discovered", JArray.FromObject(discoveredIds)));
            }


            if (Player.Equipments[(int)Item.Parts.WEAPON] != null)
                configData.Add(new JProperty("Weapon", Player.Equipments[(int)Item.Parts.WEAPON].Id));
            if (Player.Equipments[(int)Item.Parts.HELMET] != null)
                configData.Add(new JProperty("Helmet", Player.Equipments[(int)Item.Parts.HELMET].Id));
            if (Player.Equipments[(int)Item.Parts.CHESTPLATE] != null)
                configData.Add(new JProperty("ChestPlate", Player.Equipments[(int)Item.Parts.CHESTPLATE].Id));
            if (Player.Equipments[(int)Item.Parts.LEGGINGS] != null)
                configData.Add(new JProperty("Leggings", Player.Equipments[(int)Item.Parts.LEGGINGS].Id));
            if (Player.Equipments[(int)Item.Parts.BOOTS] != null)
                configData.Add(new JProperty("Boots", Player.Equipments[(int)Item.Parts.BOOTS].Id));


            configData.Add(new JProperty("MaxStage", MaxStage));
            using (FileStream fs = File.Open(_savePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(configData.ToString());
                }
            }
        }

        public void LoadData()
        {
            #region 유저 데이터 로드
            string jsonContent;

            using (FileStream fs = File.Open(_savePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    jsonContent = reader.ReadToEnd();
                }
            }

            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            Player = new Character(
                data["Name"].ToString(),
                (JOP)data["Job"],
                (int)data["Level"],
                (int)data["Atk"],
                (int)data["Def"],
                (int)data["MaxHp"],
                (int)data["Gold"]
                );

            Player.Exp = (int)data["Exp"];

            if (data["Inventory"] != null)
            {
                for (int i = 0; i < data["Inventory"].Count; i++)
                {
                    int id = (int)data["Inventory"][i];
                    int level = (int)data["ItemLevel"][i];

                    Item newItem = MakeNewItem(id, level);

                    Inventory.Add(newItem);
                    Shop.Remove(Shop.Find(x => x.Name == newItem.Name));
                }
            }

            if (data["Discovered"] != null)
            {
                foreach (var id in data["Discovered"])
                {
                    Shop.Add(MakeNewItem((int)id));
                    DiscoveredItem.Add((int)id);
                }
            }

            if (data["Weapon"] != null)
            {
                Item item = Inventory.Find(x => x.Id == (int)data["Weapon"]);

                Player.Equipments[(int)Item.Parts.WEAPON] = item;
                item.IsEquipped = true;
            }
            if (data["Helmet"] != null)
            {
                Item item = Inventory.Find(x => x.Id == (int)data["Helmet"]);

                Player.Equipments[(int)Item.Parts.HELMET] = item;
                item.IsEquipped = true;
            }
            if (data["ChestPlate"] != null)
            {
                Item item = Inventory.Find(x => x.Id == (int)data["ChestPlate"]);

                Player.Equipments[(int)Item.Parts.CHESTPLATE] = item;
                item.IsEquipped = true;
            }
            if (data["Leggings"] != null)
            {
                Item item = Inventory.Find(x => x.Id == (int)data["Leggings"]);

                Player.Equipments[(int)Item.Parts.LEGGINGS] = item;
                item.IsEquipped = true;
            }
            if (data["Boots"] != null)
            {
                Item item = Inventory.Find(x => x.Id == (int)data["Boots"]);

                Player.Equipments[(int)Item.Parts.BOOTS] = item;
                item.IsEquipped = true;
            }

            Player.ChangeHP((int)data["CurrentHp"] - Player.MaxHp);

            MaxStage = (int)data["MaxStage"];
            #endregion
        }

        public void Wear(Item item)
        {
            Player.Equipments[(int)item.Part] = item;
            item.IsEquipped = true;

            if (item.Part == Item.Parts.HELMET || item.Part == Item.Parts.BOOTS)
            {
                Player.ChangeHP(item.Stat + item.BonusStat);
            }
        }

        public void Unwear(Item.Parts part)
        {
            if (part == Item.Parts.HELMET || part == Item.Parts.BOOTS)
            {
                int hp;
                if (Player.CurrentHp <= Player.Equipments[(int)part].Stat + Player.Equipments[(int)part].BonusStat)
                    hp = Player.CurrentHp - 1;
                else
                    hp = Player.Equipments[(int)part].Stat + Player.Equipments[(int)part].BonusStat;
                
                Player.ChangeHP(-hp);
            }
            Player.Equipments[(int)part].IsEquipped = false;
            Player.Equipments[(int)part] = null;

        }

        public void BuyItem(Item item)
        {
            Player.Gold -= item.Price;
            Inventory.Add(item);
            Shop.Remove(item);
            GameManager.Instance.UIManager.PrintGold();
            GameManager.Instance.UIManager.PrintItems();
        }

        public void SellItem(Item item)
        {
            Player.Gold += (int)(item.Price * 0.85f);

            item.Level = 0;
            Inventory.Remove(item);
            if (!Shop.Exists(x => x.Name == item.Name)) Shop.Add(item);
            Shop = Shop.OrderBy(item => item.Id).ToList();
            GameManager.Instance.UIManager.PrintGold();
            GameManager.Instance.UIManager.PrintItems();
        }

        public void GameDataSetting()
        {

            #region 아이템 데이터 로드
            using (FileStream fs = File.Open(_itemPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    _items = JsonConvert.DeserializeObject<Item[]>(reader.ReadToEnd());
                }
            }
            #endregion

            #region 상점 세팅
            for (int i = 0; i < _items.Length; i++)
            {
                if (i % 10 > 3) i += 9 - i % 10;
                else
                {
                    Shop.Add(MakeNewItem(i));
                }
            }
            Shop = Shop.OrderBy(item => item.Id).ToList();
            #endregion

            #region 몬스터 세팅
            // 이름 , id, 레벨, 공격력, 방어력, HP, 골드, 경험치
            _monsters.Add(new Monster("박쥐", 0, 1, 1, 1, 10, 50, 5));
            _monsters.Add(new Monster("토끼", 1, 1, 1, 0, 15, 50, 5));
            _monsters.Add(new Monster("거미", 2, 1, 2, 1, 5, 60, 6));
            _monsters.Add(new Monster("쥐", 3, 1, 2, 1, 10, 70, 7));
            _monsters.Add(new Monster("뱀", 4, 1, 3, 1, 15, 90, 9));
            _monsters.Add(new Monster("고블린 정찰병", 5, 2, 3, 3, 20, 150, 15));
            _monsters.Add(new Monster("배고픈 멧돼지", 6, 2, 3, 3, 30, 200, 20));
            _monsters.Add(new Monster("날렵한 올빼미", 7, 2, 2, 4, 20, 170, 17));
            _monsters.Add(new Monster("불개미 무리", 8, 2, 4, 1, 15, 150, 15));
            _monsters.Add(new Monster("허약한 스켈레톤", 9, 3, 4, 4, 30, 300, 30));
            _monsters.Add(new Monster("광부 코볼트", 10, 3, 6, 3, 40, 450, 45));
            _monsters.Add(new Monster("허술한 도적", 11, 3, 5, 4, 35, 400, 40));
            _monsters.Add(new Monster("허술한 도적 궁수", 12, 3, 8, 2, 25, 500, 50));
            _monsters.Add(new Monster("거대 사마귀", 13, 4, 7, 5, 40, 700, 70));
            _monsters.Add(new Monster("거대 타란툴라", 14, 4, 12, 2, 30, 850, 85));
            _monsters.Add(new Monster("거대 장수말벌", 15, 4, 6, 8, 50, 1000, 100));
            _monsters.Add(new Monster("아라크네", 16, 4, 8, 8, 60, 1300, 130));
            _monsters.Add(new Monster("고블린 무리", 17, 5, 10, 10, 80, 1600, 160));
            _monsters.Add(new Monster("흉폭한 야생마 무리", 18, 5, 7, 12, 100, 1800, 180));
            _monsters.Add(new Monster("오크", 19, 5, 14, 10, 80, 2200, 220));
            _monsters.Add(new Monster("놀", 20, 5, 15, 12, 120, 2400, 240));
            _monsters.Add(new Monster("갈색 그리즐리 베어", 21, 6, 18, 18, 150, 2800, 280));
            _monsters.Add(new Monster("검정 그리즐리 베어", 22, 6, 22, 16, 180, 3200, 320));
            _monsters.Add(new Monster("숨죽인 재규어", 23, 6, 28, 12, 80, 3000, 300));
            _monsters.Add(new Monster("각성한 곰 우르순", 24, 6, 22, 24, 200, 3600, 360));
            _monsters.Add(new Monster("블랙 와이번", 25, 7, 20, 30, 200, 4000, 400));
            _monsters.Add(new Monster("레드 와이번", 26, 7, 30, 20, 180, 4200, 420));
            _monsters.Add(new Monster("변종 거대 독수리", 27, 7, 22, 22, 280, 3800, 380));
            _monsters.Add(new Monster("용의 해츨링", 27, 7, 15, 40, 300, 4600, 460));
            _monsters.Add(new Monster("", 27, 7, 15, 40, 300, 4600, 460));





            #endregion

            #region 던전 세팅
            using (FileStream fs = File.Open(_dungeonPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    _dungeons = JsonConvert.DeserializeObject<Dungeon[]>(reader.ReadToEnd());
                }
            }
            #endregion

            #region 휴식 세팅
            Shelters.Add(new Shelter("약초 처방", 100, 500));
            Shelters.Add(new Shelter("전문 진료", 500, 5000));
            Shelters.Add(new Shelter("입원 치료", 1000, 50000));
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
                    Inventory = Inventory.OrderBy(item => item.Name).ToList();
                    break;
                case 2:
                    Inventory = Inventory.OrderByDescending(item => item.Stat).ToList();
                    break;
                case 3:
                    Inventory = Inventory.OrderBy(item => item.Price).ToList();
                    break;
                case 4:
                    Inventory = Inventory.OrderByDescending(item => item.IsEquipped).ToList();
                    break;
                case 5:
                    Inventory = Inventory.OrderByDescending(item => item.Level).ToList();
                    break;
            }
        }

        public void ExploreDungeon(int num)
        {
            UIManager ui = GameManager.Instance.UIManager;
            int stage = num + StagePage;
            Dungeon dungeon = _dungeons[stage - 1];
            Random rnd = new Random();

            // 배틀 진입 후 결과 반환
            bool clear = GameManager.Instance.BattleManager.EntryBattle(dungeon);

            // 배틀 진행 끝났다면 결과 정산합니다.

            int rewardGold = 0;
            int rewardExp = 0;
            // 죽은 몬스터에 따라 골드와 아이템, 경험치를 보상에 추가합니다.
            List<Item> rewardItems = new List<Item>();
            foreach (Monster m in GameManager.Instance.BattleManager.Monsters)
            {
                if (!m.IsDead())
                    continue;
                rewardGold += m.Reward[0];

                rewardExp += m.RewardExp;
                for (int i = 1; i < m.Reward.Count; i++)
                {
                    int id = m.Reward[i];
                    rewardItems.Add(MakeNewItem(id));
                }
            }

            // 클리어를 하지 못했다면 잡은 몬스터의 보상을 주고 끝냅니다.
            if (!clear)
            {
                Player.Gold += rewardGold;
                Player.Exp += rewardExp;
                Inventory.AddRange(rewardItems);
                // UI 로그에 출력합니다.
                PrintDungeonExploreResult(dungeon, clear, rewardGold, rewardExp, rewardItems);
                return;
            }
                
            // 던전 클리어 골드, 경험치, 아이템 보상을 추가합니다.
            if (stage == MaxStage) MaxStage++;
            rewardGold = (int)(dungeon.Reward[0]
                * (rnd.NextDouble() * 0.4 + 0.8));
            rewardExp += stage * stage * 10;
            if (dungeon.Reward.Count > 1 && rnd.Next(0, 100) < 40)
            {
                int rewardItemId = dungeon.Reward[rnd.Next(1, dungeon.Reward.Count)];

                Inventory.Add(MakeNewItem(rewardItemId));
                // 던전 보상으로 얻은 아이템을 상점에서도 나오게 합니다.
                if (!DiscoveredItem.Exists(x => x == rewardItemId)) DiscoveredItem.Add(rewardItemId);
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
                Player.ChangeHP(st.Heal);

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

            while(true)
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
                            int jop;
                            if(int.TryParse(Console.ReadLine(), out jop))
                            {
                                if (jop >= 1 && jop <= (int)JOP.ARCHER + 1)
                                {
                                    Player = new Character(name, (JOP)(jop - 1), 1, 10, 5, 100, 1500);
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
            Inventory.Add(MakeNewItem(0)); Shop.Remove(Shop.Find(x => x.Name == _items[0].Name));
            Inventory.Add(MakeNewItem(10)); Shop.Remove(Shop.Find(x => x.Name == _items[10].Name));
            Inventory.Add(MakeNewItem(20)); Shop.Remove(Shop.Find(x => x.Name == _items[20].Name));
            Inventory.Add(MakeNewItem(30)); Shop.Remove(Shop.Find(x => x.Name == _items[30].Name));
            Inventory.Add(MakeNewItem(40)); Shop.Remove(Shop.Find(x => x.Name == _items[40].Name));
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
                if (StagePage == _dungeons.Length - 3)
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
                    if (item.IsEquipped)
                    {
                        Unwear(item.Part);
                    }

                    item.Level = 0;
                    Inventory.Remove(item);
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
            Monster newMonster = new Monster(monster.Name, monster.Id, monster.Level, monster.Atk, monster.Def, monster.MaxHp, 0, monster.RewardExp);

            newMonster.Reward.Clear();
            foreach(var reward in monster.Reward)
            {
                // TODO : 던전 레벨별로 몬스터도 맞춰서 들어갈 수 있도록 보정하는 로직

                newMonster.Reward.Add(reward);
            }
            return newMonster;
        }

        public Dungeon GetDungeon(int num)
        {
            return _dungeons[num + StagePage];
        }
    }
}
