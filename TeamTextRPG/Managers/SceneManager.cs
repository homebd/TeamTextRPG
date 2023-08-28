/// <summary
/// Scene을 관리하고 각 Scene이 수행할 기능까지도 구현된 클래스
/// </summary>

using Newtonsoft.Json;

namespace TeamTextRPG.Managers
{
    internal class SceneManager
    {
        public GameManager GameManager;

        private string _path = @"../../../Scenes";
        public enum Scenes
        {
            GAME_INTRO,
            GAME_OUTRO,
            TOWN,
            STATUS,
            INVENTORY_MAIN,
            INVENTORY_EQUIP,
            INVENTORY_SORT,
            SHOP_MAIN,
            SHOP_BUY,
            SHOP_SELL,
            DUNGEON,
            SHELTER,
            SMITHY
        }

        public Scenes Scene;

        public SceneManager()
        {
            Scene = Scenes.GAME_INTRO;
        }

        public void LoadScene()
        {
            string path = MakePath();
            if (path == string.Empty || !File.Exists(path)) return;

            string jsonContent;
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    jsonContent = reader.ReadToEnd();
                }
            }

            dynamic sceneData = JsonConvert.DeserializeObject(jsonContent);

            UIManager ui = GameManager.Instance.UIManager;

            ui.PrintTitle(sceneData["Title"].ToString());
            ui.PrintDescription(sceneData["Description"].ToString());

            if (Scene == Scenes.TOWN) GameManager.Instance.DataManager.SaveData();
            else if (Scene == Scenes.STATUS)
            {
                ui.PrintGold();
                ui.MakeStatusBox();
                ui.PrintLevel();
            }
            else if (Scene >= Scenes.INVENTORY_MAIN && Scene <= Scenes.SHOP_SELL)
            {
                ui.PrintItemCategories();

                if (Scene >= Scenes.SHOP_MAIN && Scene <= Scenes.SHOP_SELL)
                {
                    ui.PrintGold();
                    if (Scene == Scenes.SHOP_MAIN) ui.PrintItems();
                    else if (Scene == Scenes.SHOP_BUY) ui.PrintItems();
                    else ui.PrintItems();
                }
                else
                {
                    if (Scene == Scenes.INVENTORY_EQUIP) ui.PrintItems();
                    else ui.PrintItems();
                }
            }
            else if (Scene == Scenes.DUNGEON)
            {
                ui.MakeDungeonBox();
                ui.PrintDef();
                ui.PrintLevel();
            }
            else if (Scene == Scenes.SHELTER)
            {
                ui.PrintGold();
                ui.MakeShelterBox();
                ui.PrintHp();
            }
            else if (Scene == Scenes.SMITHY)
            {
                ui.PrintItemCategories();
                ui.PrintGold();
                ui.PrintItemsAtSmithy();
            }

            int minOption = (int)sceneData["OptionMin"];
            int maxOption;
            List<string>? option;

            if (Scene == Scenes.INVENTORY_EQUIP || Scene == Scenes.SHOP_BUY
                || Scene == Scenes.SHOP_SELL || Scene == Scenes.GAME_OUTRO)
            {
                option = sceneData["Option2"].ToObject(typeof(List<string>));
                maxOption = GameManager.Instance.DataManager.SortedItems.Count;
            }
            else if (Scene == Scenes.INVENTORY_SORT)
            {
                option = sceneData["Option3"].ToObject(typeof(List<string>));
                maxOption = option.Count - 1;
            }
            else if (Scene == Scenes.DUNGEON)
            {
                option = sceneData["Option"].ToObject(typeof(List<string>));
                maxOption = GameManager.Instance.DataManager.MaxStage - GameManager.Instance.DataManager.StagePage;
                if (maxOption < 0) maxOption = 0;
            }
            else if (Scene == Scenes.SHELTER)
            {
                option = sceneData["Option"].ToObject(typeof(List<string>));
                maxOption = 3;
            }
            else if (Scene == Scenes.SMITHY)
            {
                option = sceneData["Option"].ToObject(typeof(List<string>));
                maxOption = GameManager.Instance.DataManager.SortedItems.Count;
            }
            else
            {
                option = sceneData["Option"].ToObject(typeof(List<string>));
                maxOption = option.Count - ((minOption == 0) ? 1 : 0);
            }

            ui.MakeOptionBox(option);
            ui.MakeLogBox();

            Input(minOption, maxOption);
        }

        private string MakePath()
        {
            switch (Scene)
            {
                case Scenes.GAME_INTRO:
                case Scenes.GAME_OUTRO:
                    return Path.Combine(_path, "GameMenu.json");
                case Scenes.TOWN:
                    return Path.Combine(_path, "Town.json");
                case Scenes.STATUS:
                    return Path.Combine(_path, "Status.json");
                case Scenes.INVENTORY_MAIN:
                case Scenes.INVENTORY_EQUIP:
                case Scenes.INVENTORY_SORT:
                    return Path.Combine(_path, "Inventory.json");
                case Scenes.SHOP_MAIN:
                case Scenes.SHOP_BUY:
                case Scenes.SHOP_SELL:
                    return Path.Combine(_path, "Shop.json");
                case Scenes.DUNGEON:
                    return Path.Combine(_path, "Dungeon.json");
                case Scenes.SHELTER:
                    return Path.Combine(_path, "Shelter.json");
                case Scenes.SMITHY:
                    return Path.Combine(_path, "Smithy.json");
            }

            return string.Empty;
        }

        private void Input(int min, int max)
        {
            DataManager dm = GameManager.Instance.DataManager;
            UIManager ui = GameManager.Instance.UIManager;

            while (true)
            {
                ui.SetCursorPositionForOption();

                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                    {
                        switch(Scene)
                        {
                            case Scenes.GAME_INTRO:
                                switch(ret)
                                {
                                    case 1: // 회원가입
                                        ui.AddLog("ID를 입력하세요.(영어로)");
                                        dm.CreateId();
                                        return;
                                    case 2: // 로그인
                                        ui.AddLog("ID를 입력하세요.(영어로)");
                                        dm.LoginId();
                                        return;
                                    case 0:
                                        Environment.Exit(0);
                                        return;
                                }
                                break;
                            case Scenes.GAME_OUTRO:
                                if(ret == 0)
                                    Environment.Exit(0);
                                break;
                            case Scenes.TOWN:
                                switch (ret)
                                {
                                    case 1:
                                        Scene = Scenes.STATUS;
                                        return;
                                    case 2:
                                        Scene = Scenes.INVENTORY_MAIN;
                                        return;
                                    case 3:
                                        Scene = Scenes.SHOP_MAIN;
                                        return;
                                    case 4:
                                        Scene = Scenes.DUNGEON;
                                        return;
                                    case 5:
                                        Scene = Scenes.SHELTER;
                                        return;
                                    case 6:
                                        Scene = Scenes.SMITHY;
                                        return;
                                    case 7:
                                        Environment.Exit(0);
                                        return;
                                }
                                break;
                            case Scenes.STATUS:
                                switch (ret)
                                {
                                    case 0:
                                        Scene = Scenes.TOWN;
                                        return;
                                }
                                break;
                            case Scenes.INVENTORY_MAIN:
                                switch (ret)
                                {
                                    case 0:
                                        Scene = Scenes.TOWN;
                                        return;
                                    case 1:
                                        Scene = Scenes.INVENTORY_EQUIP;
                                        return;
                                    case 2:
                                        Scene = Scenes.INVENTORY_SORT;
                                        return;
                                }
                                break;
                            case Scenes.INVENTORY_EQUIP:
                                if (ret == 0)
                                {
                                    Scene = Scenes.INVENTORY_MAIN;
                                    return;
                                }
                                else
                                {
                                    var selectedItem = dm.SortedItems[ret - 1];

                                    if (selectedItem.IsEquipped)
                                    {
                                        dm.Unwear(selectedItem.Part);
                                    }
                                    else
                                    {
                                        if (dm.Player.Equipments[(int)selectedItem.Part] != null)
                                            dm.Unwear(selectedItem.Part);

                                        dm.Wear(selectedItem);
                                    }

                                    return;
                                }
                            case Scenes.INVENTORY_SORT:
                                if (ret == 0)
                                {
                                    Scene = Scenes.INVENTORY_MAIN;
                                    return;
                                }
                                else
                                {
                                    dm.SortInventory(ret);
                                    return;
                                }
                            case Scenes.SHOP_MAIN:
                                switch (ret)
                                {
                                    case 0:
                                        Scene = Scenes.TOWN;
                                        return;
                                    case 1:
                                        Scene = Scenes.SHOP_BUY;
                                        return;
                                    case 2:
                                        Scene = Scenes.SHOP_SELL;
                                        return;
                                }
                                break;
                            case Scenes.SHOP_BUY:
                                if (ret == 0)
                                {
                                    Scene = Scenes.SHOP_MAIN;
                                    return;
                                }
                                else
                                {
                                    var selectedItem = dm.SortedItems[ret - 1];

                                    if (dm.Player.Gold >= selectedItem.Price)
                                    {
                                        ui.AddLog("구매를 완료했습니다.");
                                        dm.BuyItem(selectedItem);
                                    }
                                    else
                                        ui.AddLog("소지금이 부족합니다.");

                                    return;
                                }
                            case Scenes.SHOP_SELL:
                                if (ret == 0)
                                {
                                    Scene = Scenes.SHOP_MAIN;
                                    return;
                                }
                                else
                                {
                                    var selectedItem = dm.SortedItems[ret - 1];

                                    if (selectedItem.IsEquipped)
                                    {
                                        ui.AddLog("장착 중인 장비는 판매할 수 없습니다.");
                                    }
                                    else
                                    {
                                        ui.AddLog("판매를 완료했습니다.");
                                        dm.SellItem(selectedItem);
                                    }

                                    return;
                                }
                            case Scenes.DUNGEON:
                                if (ret == 0)
                                {
                                    Scene = Scenes.TOWN;
                                    return;
                                }
                                else
                                {
                                    dm.ExploreDungeon(ret);
                                    if (dm.Player.CurrentHp == 0)
                                    {
                                        Scene = Scenes.GAME_OUTRO;
                                        ui.AddLog("사망했습니다.");
                                    }
                                    return;
                                }
                            case Scenes.SHELTER:
                                if (ret == 0)
                                {
                                    Scene = Scenes.TOWN;
                                    return;
                                }
                                else
                                {
                                    dm.RestPlayer(ret);
                                    return;
                                }
                            case Scenes.SMITHY:
                                if (ret == 0)
                                {
                                    Scene = Scenes.TOWN;
                                    return;
                                }
                                else
                                {
                                    var selectedItem = dm.SortedItems[ret - 1];

                                    dm.StrengthenItem(selectedItem);

                                    return;
                                }
                        }
                    }
                }

                if (Scene >= Scenes.INVENTORY_MAIN && Scene <= Scenes.SHOP_SELL || Scene == Scenes.SMITHY)
                {
                    if(ui.ShiftCategory(input)) return;
                }
                else if (Scene >= Scenes.DUNGEON)
                {
                    if (dm.ShiftStagePage(input)) return;
                }
                else if (Scene == Scenes.TOWN)
                {
                    if (input == "x" || input == "X")
                        Environment.Exit(0);
                }

                ui.AddLog("잘못된 입력입니다.");
            }
        }
    }
}
