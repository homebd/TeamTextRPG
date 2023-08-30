/// <summary
/// 플레이어 데이터 및 아이템과 던전 등의 데이터를 관리하는 클래스
/// </summary>

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TeamTextRPG.Classes;
using TeamTextRPG.Common;
using static TeamTextRPG.Managers.SceneManager;

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
            #region 아이템 정보 세팅
            // 이름 id 파츠 레벨 스탯 가격 설명
            // 무기 = 공격력, 헬멧과 부츠 = 체력, 갑옷과 바지 = 방어력
            // 0~49 번은 던전 보상으로 주어질 예정.
            _items.Add(new Item("목검", 0, Parts.WEAPON, 0, 1, 800, "금방이라도 부러질 듯한 목검입니다."));
            _items.Add(new Item("낡은 검", 1, Parts.WEAPON, 0, 3, 3000, "너무 무뎌져 종이조차 자르지 못하는 검입니다."));
            _items.Add(new Item("모험자의 도끼", 2, Parts.WEAPON, 0, 7, 8000, "대륙 끝까지 도달했던 모험자의 도끼입니다."));
            _items.Add(new Item("거미 독 창", 3, Parts.WEAPON, 0, 11, 20000, "거미의 맹독이 발린 창입니다."));
            _items.Add(new Item("사자발톱 검 ", 4, Parts.WEAPON, 0, 16, 38000, "초왕 \'라이언\'의 발톱으로 만든 검입니다."));
            _items.Add(new Item("곰 장갑", 5, Parts.WEAPON, 0, 23, 55000, "폭군 \'비틀즈\'의 발톱이 달린 장갑입니다."));
            _items.Add(new Item("지룡의 단검", 6, Parts.WEAPON, 0, 31, 90000, "지룡 \'메테오\'의 어금니를 벼려 만든 단검입니다."));
            _items.Add(new Item("수룡장", 7, Parts.WEAPON, 0, 40, 150000, "수룡 \'네시\'의 두개골로 만든 지팡이입니다."));
            _items.Add(new Item("백월", 8, Parts.WEAPON, 0, 50, 240000, "무명의 밤, 이 차크람이 세상을 비췄습니다."));
            _items.Add(new Item("초월한 창", 9, Parts.WEAPON, 0, 63, 440000, "이 창을 쥐면 용맹함이 샘솟는 것 같습니다."));
            
            _items.Add(new Item("가벼운 천 모자", 10, Parts.HELMET, 0, 5, 500, "머리에 꼭 맞는 모자입니다."));
            _items.Add(new Item("수련자 두건", 11, Parts.HELMET, 0, 20, 2000, "수련에 도움을 주는 두건입니다."));
            _items.Add(new Item("모험자의 투구", 12, Parts.HELMET, 0, 40, 5000, "대륙 끝까지 도달했던 모험자의 투구입니다."));
            _items.Add(new Item("실크 모자", 13, Parts.HELMET, 0, 80, 13000, "마력이 깃든 거미줄로 만든 모자입니다."));
            _items.Add(new Item("사슴뿔 투구", 14, Parts.HELMET, 0, 125, 22000, "성록 \'데이지\'의 뿔로 장식한 투구입니다."));
            _items.Add(new Item("공작깃 투구", 15, Parts.HELMET, 0, 180, 35000, "명조 \'무지개\'의 깃털로 장식한 투구입니다."));
            _items.Add(new Item("지룡의 투구", 16, Parts.HELMET, 0, 245, 55000, "지룡 \'메테오\'의 비늘로 덮은 투구입니다."));
            _items.Add(new Item("수룡의 투구", 17, Parts.HELMET, 0, 320, 90000, "수룡 \'네시\'의 지느러미로 덮은 투구입니다"));
            _items.Add(new Item("월관", 18, Parts.HELMET, 0, 405, 150000, "흑백청적 네 빛의 보석이 찬란하게 빛납니다."));
            _items.Add(new Item("초월한 투구 ", 19, Parts.HELMET, 0, 500, 270000, "이 투구를 쓰면 강인함 샘솟는 것 같습니다."));
            
            _items.Add(new Item("얇은 천 상의", 20, Parts.CHESTPLATE, 0, 1, 500, "얇은 천으로 만들어진 상의입니다."));
            _items.Add(new Item("수련자 상의", 21, Parts.CHESTPLATE, 0, 2, 2000, "수련에 도움을 주는 옷입니다."));
            _items.Add(new Item("모험자의 갑옷", 22, Parts.CHESTPLATE, 0, 4, 5000, "대륙 끝까지 도달했던 모험자의 갑옷입니다."));
            _items.Add(new Item("실크 로브", 23, Parts.CHESTPLATE, 0, 6, 13000, "마력이 깃든 거미줄로 만든 로브입니다."));
            _items.Add(new Item("악어 갑옷", 24, Parts.CHESTPLATE, 0, 8, 22000, "늪의 군주 \'샤로\'의 가죽으로 만든 갑옷입니다."));
            _items.Add(new Item("곰가죽 갑옷", 25, Parts.CHESTPLATE, 0, 10, 35000, "폭군 \'비틀즈\'의 가죽을 받친 갑옷입니다."));
            _items.Add(new Item("지룡의 갑주", 26, Parts.CHESTPLATE, 0, 13, 55000, "지룡 \'메테오\'의 날갯가죽으로 만든 갑옷입니다."));
            _items.Add(new Item("수룡 갑주", 27, Parts.CHESTPLATE, 0, 16, 90000, "수룡 \'네시\'의 비늘로 덮은 갑옷입니다."));
            _items.Add(new Item("적월", 28, Parts.CHESTPLATE, 0, 19, 150000, "붉은 땅, 달만이 외로이 피어 몽우리졌습니다."));
            _items.Add(new Item("빨간 망토", 29, Parts.CHESTPLATE, 0, 23, 270000, "이 망토만 있다면 갑옷은 불필요합니다."));
            
            _items.Add(new Item("얇은 천 바지", 30, Parts.LEGGINGS, 0, 1, 500, "군데군데 헤진 바지입니다."));
            _items.Add(new Item("수련자 하의", 31, Parts.LEGGINGS, 0, 2, 2000, "수련에 도움을 주는 바지입니다."));
            _items.Add(new Item("모험자의 바지", 32, Parts.LEGGINGS, 0, 3, 5000, "대륙 끝까지 도달했던 모험자의 바지입니다."));
            _items.Add(new Item("실크 바지", 33, Parts.LEGGINGS, 0, 5, 13000, "마력이 깃든 거미줄로 만든 바지입니다."));
            _items.Add(new Item("뱀가죽 바지", 34, Parts.LEGGINGS, 0, 6, 22000, "교사 \'스니키\'의 가죽으로 만든 레깅스입니다."));
            _items.Add(new Item("곰가죽 바지", 35, Parts.LEGGINGS, 0, 8, 35000, "폭군 \'비틀즈\'의 가죽으로 만든 바지입니다."));
            _items.Add(new Item("지룡의 바지", 36, Parts.LEGGINGS, 0, 10, 55000, "지룡 \'메테오\'의 지느러미로 덮은 바지입니다."));
            _items.Add(new Item("수룡의 문양", 37, Parts.LEGGINGS, 0, 12, 90000, "수룡 \'네시\'의 표식이 박힌 바지입니다."));
            _items.Add(new Item("흑월", 38, Parts.LEGGINGS, 0, 14, 150000, "달이 없던 그날 밤, 그 그림자에 물들었습니다."));
            _items.Add(new Item("빨간 반바지", 39, Parts.LEGGINGS, 0, 17, 270000, "반바지에서조차 그의 예절과 겸손이 느껴집니다."));
            
            _items.Add(new Item("발싸개", 40, Parts.BOOTS, 0, 2, 500, "신발이라고 부르기 민망한 천 쪼가리입니다."));
            _items.Add(new Item("수련자 단화", 41, Parts.BOOTS, 0, 4, 2000, "수련에 도움을 주는 단화입니다."));
            _items.Add(new Item("모험자의 부츠", 42, Parts.BOOTS, 0, 6, 5000, "대륙 끝까지 도달했던 모험자의 부츠입니다."));
            _items.Add(new Item("실크 부츠", 43, Parts.BOOTS, 0, 8, 13000, "마력이 깃든 거미줄로 만든 부츠입니다."));
            _items.Add(new Item("늑대 부츠", 44, Parts.BOOTS, 0, 10, 22000, "걸랑 \'울\'의 발바닥을 밑창에 붙인 부츠입니다."));
            _items.Add(new Item("곰가죽 장화", 45, Parts.BOOTS, 0, 12, 35000, "폭군 \'비틀즈\'의 가죽으로 만든 장화입니다."));
            _items.Add(new Item("지룡의 각반", 46, Parts.BOOTS, 0, 14, 55000, "지룡 \'메테오\'의 뿔을 감아 만든 각반입니다."));
            _items.Add(new Item("수룡각", 47, Parts.BOOTS, 0, 16, 90000, "수룡 \'네시\'의 보주로 만든 각반입니다."));
            _items.Add(new Item("청월", 48, Parts.BOOTS, 0, 18, 150000, "달은 물에 잠겨서도 은은한 빛을 만들었습니다."));
            _items.Add(new Item("살구색 양말", 49, Parts.BOOTS, 0, 20, 270000, "맨발은 위험합니다."));

            // id가 50 이상은 몬스터에게 드랍되는 아이템
            _items.Add(new Item("거미 눈 헬멧", 50, Parts.HELMET, 0, 10, 1000, "거미 눈을 달아놓은 헬멧입니다."));
            _items.Add(new Item("뱀 독 단검", 51, Parts.WEAPON, 0, 2, 1200, "뱀의 독을 발라놓은 단검입니다."));
            _items.Add(new Item("고블린의 누더기 갑옷", 52, Parts.CHESTPLATE, 0, 2, 1500, "고블린이 입던 더러운 가죽 갑옷입니다."));
            _items.Add(new Item("멧돼지 가죽 부츠", 53, Parts.BOOTS, 0, 3, 1000, "단단한 멧돼지의 가죽으로 만들어졌습니다."));
            _items.Add(new Item("코볼트 곡괭이", 54, Parts.WEAPON, 0, 5, 5000, "코볼트가 쓰던 곡괭이로 굉장히 무겁습니다."));
            _items.Add(new Item("궁수의 신발", 55, Parts.BOOTS, 0, 5, 3500, "도적 궁수가 신던 신발로 가볍고 튼튼합니다."));
            _items.Add(new Item("도적의 바지", 56, Parts.LEGGINGS, 0, 3, 3500, "도적이 입던 바지로 손때가 탄 것 말고는 괜찮습니다."));
            _items.Add(new Item("사마귀 갑각", 57, Parts.CHESTPLATE, 0, 5, 9000, "사마귀의 갑각을 다듬어서 만든 갑옷입니다."));
            _items.Add(new Item("말벌 투구", 58, Parts.HELMET, 0, 60, 9000, "말벌의 겹눈을 가공해서 덧댄 투구입니다."));
            _items.Add(new Item("오크 글레이브", 59, Parts.WEAPON, 0, 14, 30000, "오크가 쓰던 글레이브입니다."));

            _items.Add(new Item("놀 가죽 바지", 60, Parts.LEGGINGS, 0, 5, 17000, "놀의 가죽과 털로 만들어진 바지입니다."));
            _items.Add(new Item("우르순의 가죽 갑옷", 61, Parts.CHESTPLATE, 0, 9, 30000, "각성한 곰의 가죽으로 만든 갑옷입니다."));
            _items.Add(new Item("숨 죽인 신발", 62, Parts.BOOTS, 0, 11, 30000, "재규어의 발가죽으로 만든 소음이 적은 신발입니다."));
            _items.Add(new Item("와이번 이빨 창", 63, Parts.WEAPON, 0, 27, 75000, "와이번의 이빨을 가공해서 만든 창입니다"));
            _items.Add(new Item("독수리 깃털 투구", 64, Parts.HELMET, 0, 220, 40000, "변종 독수리의 깃털로 장식된 투구입니다."));
            _items.Add(new Item("인어족의 비늘각반", 65, Parts.LEGGINGS, 0, 11, 75000, "인어족의 비늘로 이뤄진 각반입니다."));
            _items.Add(new Item("아귀 머리", 66, Parts.HELMET, 0, 280, 75000, "미끌미끌한 아귀의 머리입니다."));
            _items.Add(new Item("그림자 검", 67, Parts.WEAPON, 0, 45, 200000, "칠흑의 검신을 가진 검입니다."));
            _items.Add(new Item("차크람의 갑옷", 68, Parts.CHESTPLATE, 0, 17, 120000, "차크람이 한 때 입었던 갑옷입니다."));
            _items.Add(new Item("드래곤 비늘 신발", 69, Parts.BOOTS, 0, 19, 220000, "드래곤의 비늘을 덧댄 신발로 아주 튼튼합니다."));
            #endregion

            #region 상점 세팅
            for (int i = 0; i < _items.Count; i++)
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
            _monsters.Add(new Monster("거미", 2, 1, 2, 1, 5, 60, 6, 50));
            _monsters.Add(new Monster("쥐", 3, 1, 2, 1, 10, 70, 7));
            _monsters.Add(new Monster("뱀", 4, 1, 3, 1, 15, 90, 9, 51));
            _monsters.Add(new Monster("고블린 정찰병", 5, 2, 3, 3, 20, 150, 15, 52));
            _monsters.Add(new Monster("배고픈 멧돼지", 6, 2, 3, 3, 30, 200, 20, 53));
            _monsters.Add(new Monster("날렵한 올빼미", 7, 2, 2, 4, 20, 170, 17));
            _monsters.Add(new Monster("불개미 무리", 8, 2, 4, 1, 15, 150, 15));
            _monsters.Add(new Monster("허약한 스켈레톤", 9, 2, 3, 3, 30, 240, 24));
            _monsters.Add(new Monster("광부 코볼트", 10, 4, 8, 4, 40, 700, 70, 54));
            _monsters.Add(new Monster("허술한 도적", 11, 3, 5, 4, 35, 400, 40, 56));
            _monsters.Add(new Monster("허술한 도적 궁수", 12, 3, 8, 2, 25, 500, 50, 55));
            _monsters.Add(new Monster("거대 사마귀", 13, 4, 7, 5, 40, 700, 70, 57));
            _monsters.Add(new Monster("거대 타란툴라", 14, 4, 12, 2, 30, 850, 85));
            _monsters.Add(new Monster("거대 장수말벌", 15, 3, 4, 6, 30, 500, 50, 58));
            _monsters.Add(new Monster("아라크네", 16, 5, 12, 8, 60, 1400, 140));
            _monsters.Add(new Monster("악어 무리", 17, 4, 8, 8, 70, 1200, 120));
            _monsters.Add(new Monster("흉폭한 늑대 무리", 18, 5, 12, 8, 100, 1600, 160));
            _monsters.Add(new Monster("오크", 19, 6, 16, 14, 120, 2200, 220, 59));
            _monsters.Add(new Monster("놀", 20, 5, 13, 10, 100, 1800, 180, 60));
            _monsters.Add(new Monster("갈색 그리즐리 베어", 21, 6, 16, 16, 150, 2600, 260));
            _monsters.Add(new Monster("검정 그리즐리 베어", 22, 6, 20, 16, 180, 3000, 300));
            _monsters.Add(new Monster("숨죽인 재규어", 23, 5, 24, 4, 80, 2400, 240, 62));
            _monsters.Add(new Monster("각성한 곰 우르순", 24, 7, 22, 24, 200, 3800, 380, 61));
            _monsters.Add(new Monster("블랙 와이번", 25, 7, 20, 30, 200, 4000, 400));
            _monsters.Add(new Monster("레드 와이번", 26, 7, 30, 20, 180, 4200, 420, 63));
            _monsters.Add(new Monster("변종 거대 독수리", 27, 6, 13, 17, 140, 2400, 240, 64));
            _monsters.Add(new Monster("지룡의 해츨링", 28, 8, 20, 40, 300, 5500, 550));
            _monsters.Add(new Monster("심해 해파리", 29, 7, 15, 25, 160, 3400, 340));
            _monsters.Add(new Monster("아귀", 30, 8, 30, 20, 250, 4600, 460, 66));
            _monsters.Add(new Monster("인어족 전사", 31, 8, 30, 30, 300, 5000, 500, 65));
            _monsters.Add(new Monster("수룡의 사념", 32, 9, 45, 35, 450, 7500, 750));
            _monsters.Add(new Monster("그림자 사냥개", 33, 8, 45, 20, 300, 6000, 600));
            _monsters.Add(new Monster("그림자 망령", 34, 9, 30, 50, 550, 8500, 850));
            _monsters.Add(new Monster("그림자 암살자", 35, 9, 60, 25, 350, 9500, 950, 67));
            _monsters.Add(new Monster("차크람의 후예", 36, 10, 60, 40, 600, 13000, 1200, 68));
            _monsters.Add(new Monster("괴수 히드라", 37, 9, 45, 45, 500, 11000, 1100));
            _monsters.Add(new Monster("마계의 사천왕", 38, 10, 50, 60, 700, 16000, 1600));
            _monsters.Add(new Monster("골드 드래곤", 39, 10, 55, 65, 800, 20000, 2000, 69));
            _monsters.Add(new Monster("마왕 바알", 40, 11, 75, 75, 1400, 30000, 3000));





            #endregion

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
                    Player.Inventory = Player.Inventory.OrderBy(item => item.Name).ToList();
                    break;
                case 2:
                    Player.Inventory = Player.Inventory.OrderByDescending(item => item.Stat).ToList();
                    break;
                case 3:
                    Player.Inventory = Player.Inventory.OrderBy(item => item.Price).ToList();
                    break;
                case 4:
                    Player.Inventory = Player.Inventory.OrderByDescending(item => GameManager.Instance.DataManager.Player.Equipments[(int)item.Part] == item).ToList();
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

                foreach (var reward in m.Inventory)
                {
                    rewardItems.Add(MakeNewItem(reward.Id));
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
            if (dungeon.Reward.Count > 1 && rnd.Next(0, 100) < 40)
            {
                int rewardItemId = dungeon.Reward[rnd.Next(1, dungeon.Reward.Count)];

                Player.Inventory.Add(MakeNewItem(rewardItemId));
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
