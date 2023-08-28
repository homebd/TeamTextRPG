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
        public List<Dungeon> Dungeons { get; private set; }
        public List<Shelter> Shelters { get; private set; }
        public List<int> DiscoveredItem { get; set; }


        private Item[] _items = new Item[50];
        private string? _id;
        public int MaxStage { get; set; }
        public int StagePage { get; set; }

        private string _savePath = @"../../../Save";

        public DataManager()
        {
            SortedItems = new List<Item>();
            Inventory = new List<Item>();
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

            JObject configData = new JObject(
                new JProperty("Name", Player.Name),
                new JProperty("Job", Player.Job),
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
                data["Job"].ToString(),
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
            #region 아이템 정보 세팅
            _items[0] = new Item("나뭇가지", 0, Item.Parts.WEAPON, 0, 1, 300, "금방이라도 부러질 듯한 나뭇가지입니다.");
            _items[1] = new Item("낡은 검", 1, Item.Parts.WEAPON, 0, 4, 1000, "너무 무뎌져 종이조차 자르지 못하는 검입니다.");
            _items[2] = new Item("청동 도끼", 2, Item.Parts.WEAPON, 0, 9, 2200, "청동으로 만들어진 무거운 도끼입니다.");
            _items[3] = new Item("은빛 창", 3, Item.Parts.WEAPON, 0, 16, 4400, "은을 매우 뾰족하게 깎아만든 창입니다.");
            _items[4] = new Item("주궁", 4, Item.Parts.WEAPON, 0, 25, 8800, "여왕 거미 \'마더\'의 거미줄로 만든 활입니다.");
            _items[5] = new Item("곰 장갑", 5, Item.Parts.WEAPON, 0, 36, 17600, "폭군 \'비틀즈\'의 발톱이 달린 장갑입니다.");
            _items[6] = new Item("지룡의 단검", 6, Item.Parts.WEAPON, 0, 49, 35200, "지룡 \'메테오\'의 어금니를 벼려 만든 단검입니다.");
            _items[7] = new Item("수룡장", 7, Item.Parts.WEAPON, 0, 64, 70400, "수룡 \'네시\'의 두개골로 만든 지팡이입니다.");
            _items[8] = new Item("백월", 8, Item.Parts.WEAPON, 0, 81, 140800, "무명의 밤, 이 차크람이 세상을 비췄습니다.");
            _items[9] = new Item("초월한 창", 9, Item.Parts.WEAPON, 0, 100, 281600, "이 창을 쥐면 용맹함이 샘솟는 것 같습니다.");

            _items[10] = new Item("야구 모자", 10, Item.Parts.HELMET, 0, 3, 500, "머리에 꼭 맞는 모자입니다.");
            _items[11] = new Item("수련자 두건", 11, Item.Parts.HELMET, 0, 9, 1800, "수련에 도움을 주는 두건입니다.");
            _items[12] = new Item("청동 투구", 12, Item.Parts.HELMET, 0, 21, 3900, "청동으로 만든 투구입니다.");
            _items[13] = new Item("강철 투구", 13, Item.Parts.HELMET, 0, 35, 8300, "강철로 만든 투구입니다.");
            _items[14] = new Item("사슴뿔 투구", 14, Item.Parts.HELMET, 0, 50, 16500, "성록 \'데이지\'의 뿔로 장식한 투구입니다.");
            _items[15] = new Item("공작깃 투구", 15, Item.Parts.HELMET, 0, 69, 35200, "명조 \'무지개\'의 깃털로 장식한 투구입니다.");
            _items[16] = new Item("지룡의 투구", 16, Item.Parts.HELMET, 0, 96, 74700, "지룡 \'메테오\'의 비늘로 덮은 투구입니다.");
            _items[17] = new Item("수룡의 투구", 17, Item.Parts.HELMET, 0, 137, 166000, "수룡 \'네시\'의 지느러미로 덮은 투구입니다");
            _items[18] = new Item("월관", 18, Item.Parts.HELMET, 0, 184, 350500, "흑백청적 네 빛의 보석이 찬란하게 빛납니다.");
            _items[19] = new Item("초월한 투구 ", 19, Item.Parts.HELMET, 0, 250, 723600, "이 투구를 쓰면 강인함 샘솟는 것 같습니다.");

            _items[20] = new Item("티셔츠", 20, Item.Parts.CHESTPLATE, 0, 1, 500, "얇은 면 티셔츠입니다.");
            _items[21] = new Item("수련자 상의", 21, Item.Parts.CHESTPLATE, 0, 3, 1000, "수련에 도움을 주는 옷입니다.");
            _items[22] = new Item("청동 갑옷", 22, Item.Parts.CHESTPLATE, 0, 7, 3500, "청동으로 만든 갑옷입니다.");
            _items[23] = new Item("강철 갑옷", 23, Item.Parts.CHESTPLATE, 0, 15, 14000, "강철로 만든 갑옷입니다.");
            _items[24] = new Item("악어 갑옷", 24, Item.Parts.CHESTPLATE, 0, 22, 40500, "늪의 군주 \'샤로\'의 가죽으로 만든 갑옷입니다.");
            _items[25] = new Item("곰가죽 갑옷", 25, Item.Parts.CHESTPLATE, 0, 31, 107000, "폭군 \'비틀즈\'의 가죽을 받친 갑옷입니다.");
            _items[26] = new Item("지룡의 갑주", 26, Item.Parts.CHESTPLATE, 0, 40, 361000, "지룡 \'메테오\'의 날갯가죽으로 만든 갑옷입니다.");
            _items[27] = new Item("수룡 갑주", 27, Item.Parts.CHESTPLATE, 0, 52, 902000, "수룡 \'네시\'의 비늘로 덮은 갑옷입니다.");
            _items[28] = new Item("적월", 28, Item.Parts.CHESTPLATE, 0, 68, 2136000, "붉은 땅, 달만이 외로이 피어 몽우리졌습니다.");
            _items[29] = new Item("빨간 망토", 29, Item.Parts.CHESTPLATE, 0, 80, 7320000, "이 망토만 있다면 갑옷은 불필요합니다.");

            _items[30] = new Item("청바지", 30, Item.Parts.LEGGINGS, 0, 3, 1000, "멋을 위해 군데군데 찢긴 청바지입니다.");
            _items[31] = new Item("수련자 하의", 31, Item.Parts.LEGGINGS, 0, 9, 2200, "수련에 도움을 주는 바지입니다.");
            _items[32] = new Item("청동 바지", 32, Item.Parts.LEGGINGS, 0, 15, 7000, "청동으로 만든 바지입니다.");
            _items[33] = new Item("강철 바지", 33, Item.Parts.LEGGINGS, 0, 23, 28000, "강철로 만든 바지입니다.");
            _items[34] = new Item("뱀가죽 바지", 34, Item.Parts.LEGGINGS, 0, 32, 91000, "교사 \'스니키\'의 가죽으로 만든 레깅스입니다.");
            _items[35] = new Item("곰가죽 바지", 35, Item.Parts.LEGGINGS, 0, 42, 214000, "폭군 \'비틀즈\'의 가죽으로 만든 바지입니다.");
            _items[36] = new Item("지룡의 바지", 36, Item.Parts.LEGGINGS, 0, 55, 722000, "지룡 \'메테오\'의 지느러미로 덮은 바지입니다.");
            _items[37] = new Item("수룡의 문양", 37, Item.Parts.LEGGINGS, 0, 68, 1804000, "수룡 \'네시\'의 표식이 박힌 바지입니다.");
            _items[38] = new Item("흑월", 38, Item.Parts.LEGGINGS, 0, 83, 4272000, "달이 없던 그날 밤, 그 그림자에 물들었습니다.");
            _items[39] = new Item("빨간 반바지", 39, Item.Parts.LEGGINGS, 0, 100, 14640000, "반바지에서조차 그의 예절과 겸손이 느껴집니다.");

            _items[40] = new Item("운동화", 40, Item.Parts.BOOTS, 0, 5, 1000, "밑창이 다 닳아버린 운동화입니다.");
            _items[41] = new Item("수련자 단화", 41, Item.Parts.BOOTS, 0, 12, 3600, "수련에 도움을 주는 단화입니다.");
            _items[42] = new Item("청동 부츠", 42, Item.Parts.BOOTS, 0, 36, 7800, "청동으로 만든 부츠입니다.");
            _items[43] = new Item("강철 부츠", 43, Item.Parts.BOOTS, 0, 68, 16600, "강철로 만든 부츠입니다.");
            _items[44] = new Item("늑대 부츠", 44, Item.Parts.BOOTS, 0, 112, 33000, "걸랑 \'울\'의 발바닥을 밑창에 붙인 부츠입니다.");
            _items[45] = new Item("곰가죽 장화", 45, Item.Parts.BOOTS, 0, 172, 70400, "폭군 \'비틀즈\'의 가죽으로 만든 장화입니다.");
            _items[46] = new Item("지룡의 각반", 46, Item.Parts.BOOTS, 0, 255, 149400, "지룡 \'메테오\'의 뿔을 감아 만든 각반입니다.");
            _items[47] = new Item("수룡각", 47, Item.Parts.BOOTS, 0, 360, 332000, "수룡 \'네시\'의 보주로 만든 각반입니다.");
            _items[48] = new Item("청월", 48, Item.Parts.BOOTS, 0, 496, 701000, "달은 물에 잠겨서도 은은한 빛을 만들었습니다.");
            _items[49] = new Item("살구색 양말", 49, Item.Parts.BOOTS, 0, 650, 1447200, "맨발은 위험합니다.");
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

            #region 던전 세팅
            Dungeons.Add(new Dungeon(Player, "마을 동굴", 5, 1000));
            Dungeons.Add(new Dungeon(Player, "옆 마을", 17, 2500));
            Dungeons.Add(new Dungeon(Player, "대륙끝의 던전", 28, 6000));
            Dungeons.Add(new Dungeon(Player, "대형 거미줄", 42, 11000));
            Dungeons[3].AddReward(4);
            Dungeons.Add(new Dungeon(Player, "초원 지대", 61, 24000));
            Dungeons[4].AddReward(14);
            Dungeons[4].AddReward(15);
            Dungeons[4].AddReward(24);
            Dungeons[4].AddReward(34);
            Dungeons[4].AddReward(44);
            Dungeons.Add(new Dungeon(Player, "곰의 절벽", 85, 38000));
            Dungeons[5].AddReward(5);
            Dungeons[5].AddReward(25);
            Dungeons[5].AddReward(35);
            Dungeons[5].AddReward(45);
            Dungeons.Add(new Dungeon(Player, "지룡의 둥지", 120, 62000));
            Dungeons[6].AddReward(6);
            Dungeons[6].AddReward(16);
            Dungeons[6].AddReward(26);
            Dungeons[6].AddReward(36);
            Dungeons[6].AddReward(46);
            Dungeons.Add(new Dungeon(Player, "심연의 해구", 170, 80000));
            Dungeons[7].AddReward(7);
            Dungeons[7].AddReward(17);
            Dungeons[7].AddReward(27);
            Dungeons[7].AddReward(37);
            Dungeons[7].AddReward(47);
            Dungeons.Add(new Dungeon(Player, "달의 안개", 230, 110000));
            Dungeons[8].AddReward(8);
            Dungeons[8].AddReward(18);
            Dungeons[8].AddReward(28);
            Dungeons[8].AddReward(38);
            Dungeons[8].AddReward(48);
            Dungeons.Add(new Dungeon(Player, "격전지", 300, 150000));
            Dungeons[9].AddReward(9);
            Dungeons[9].AddReward(19);
            Dungeons[9].AddReward(29);
            Dungeons[9].AddReward(39);
            Dungeons[9].AddReward(49);
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

        public int GetAtkBonus(bool print = true)
        {
            int atkBonus = 0;

            if (Player.Equipments[(int)Item.Parts.WEAPON] != null)
            {
                atkBonus = Player.Equipments[(int)Item.Parts.WEAPON].Stat
                    + Player.Equipments[(int)Item.Parts.WEAPON].BonusStat;
            }

            return atkBonus;
        }

        public int GetDefBonus(bool print = true)
        {
            int defBonus = 0;

            if (Player.Equipments[(int)Item.Parts.CHESTPLATE] != null)
                defBonus += Player.Equipments[(int)Item.Parts.CHESTPLATE].Stat
                    + Player.Equipments[(int)Item.Parts.CHESTPLATE].BonusStat;

            if (Player.Equipments[(int)Item.Parts.LEGGINGS] != null)
                defBonus += Player.Equipments[(int)Item.Parts.LEGGINGS].Stat
                    + Player.Equipments[(int)Item.Parts.LEGGINGS].BonusStat;

            return defBonus;
        }

        public int GetHpBonus(bool print = true)
        {
            int hpBonus = 0;

            if (Player.Equipments[(int)Item.Parts.HELMET] != null)
                hpBonus += Player.Equipments[(int)Item.Parts.HELMET].Stat
                    + Player.Equipments[(int)Item.Parts.HELMET].BonusStat;

            if (Player.Equipments[(int)Item.Parts.BOOTS] != null)
                hpBonus += Player.Equipments[(int)Item.Parts.BOOTS].Stat
                    +Player.Equipments[(int)Item.Parts.BOOTS].BonusStat;

            return hpBonus;
        }

        public void ExploreDungeon(int num)
        {
            UIManager ui = GameManager.Instance.UIManager;
            int stage = num + StagePage;
            Dungeon dungeon = Dungeons[stage - 1];
            bool clear = false;

            Random rnd = new Random();

            ui.ClearLog();

            // TODO : 배틀 진행하기!

            // 배틀 진행 끝났다면 결과 정산합니다.

            int rewardGold = 0;
            int rewardExp = 0;
            // 죽은 몬스터에 따라 골드와 아이템, 경험치를 보상에 추가합니다.
            List<Item> rewardItems = new List<Item>();
            foreach (Monster m in dungeon.Monsters)
            {
                if (!m.isDead())
                    continue;
                rewardGold += m.rewardItemIds[0];
                m.rewardItemIds.RemoveAt(0);
                rewardExp += m.rewardExp;
                foreach (int id in m.rewardItemIds)
                {
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

                        while (true)
                        {
                            ui.SetCursorPositionForOption();
                            string? name = Console.ReadLine();
                            if (name != null)
                            {
                                Player = new Character(name, "전사", 1, 10, 5, 100, 1500);
                                GetBasicItem();
                                SaveData();
                                GameManager.Instance.SceneManager.Scene = Scenes.TOWN;
                                return;
                            }
                            ui.AddLog("잘못된 입력입니다.");
                        }
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

        private Item MakeNewItem(int id)
        {
            Item item = _items[id];
            Item newItem = new Item(item.Name, item.Id, item.Part, item.Level, item.Stat, item.Price, item.Description);

            return newItem;
        }

        private Item MakeNewItem(int id, int level)
        {
            Item item = _items[id];
            Item newItem = new Item(item.Name, item.Id, item.Part, level, item.Stat, item.Price, item.Description);

            return newItem;
        }
    }
}
