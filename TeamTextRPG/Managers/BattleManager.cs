using System.Threading;
using TeamTextRPG.Classes;

namespace TeamTextRPG.Managers
{
    internal class BattleManager
    {
        public enum AttackType
        {
            NORMAL,
            SKILL,
            ITEM
        }

        public List<Monster> Monsters = new List<Monster>();
        int _size;
        int _left;

        public bool EntryBattle(Dungeon dungeon)
        {
            // 클리어 여부를 bool로 반환 false면 실패

            var ui = GameManager.Instance.UIManager;
            Random rnd = new Random();
            _size = rnd.Next(1, 5);

            // 던전의 몬스터 객체화: _size에 따라 수행
            InstantiateMonster(dungeon);
            _left = Monsters.Count;

            ui.MakeBattleBox();

            BattleInput();

            return -_left == 0;
        }

        private void BattleInput()
        {
            var ui = GameManager.Instance.UIManager;

            List<string> option = new List<string>();

            option.Add("1. 일반 공격");
            option.Add("2. 특수 공격");
            option.Add("3. 인벤토리");
            option.Add("0. 도망가기");

            while (_left > 0)
            {
                PrintPlayerUI();
                ui.ShowMonsterCard(Monsters);

                ui.MakeOptionBox(option);
                ui.SetCursorPositionForOption();

                string input = Console.ReadLine();
                if (int.TryParse(input, out var ret) && ret >= 0 && ret < option.Count)
                {
                    switch (ret)
                    {
                        case 0:
                            return;
                        case 1:
                            PrintBattleOption();
                            break;
                        case 2:
                            PrintSkillOption();
                            break;
                        case 3:
                            PrintInventoryOption();
                            break;
                    }
                }
                else ui.AddLog("잘못된 입력입니다.");
            }
        }
        private void PrintPlayerUI()
        {
            var ui = GameManager.Instance.UIManager;

            ui.PrintHp();
            ui.PrintMp();
        }

        private void PrintBattleOption()
        {
            var ui = GameManager.Instance.UIManager;
            List<string> option = new List<string>();
            bool playerTurn = true;

            for (int i = 0; i < _size; i++)
            {
                option.Add($"{i + 1}. {Monsters[i].Name}");
            }
            option.Add("0. 취소");

            while (playerTurn)
            {
                ui.MakeOptionBox(option);
                ui.SetCursorPositionForOption();

                string input = Console.ReadLine();
                if (int.TryParse(input, out var ret) && ret >= 0 && ret <= Monsters.Count)
                {
                    if(ret == 0)
                    {
                        return;
                    }
                    else
                    {
                        Monster monster = Monsters[ret - 1];

                        if (monster.IsDead()) ui.AddLog("이미 사망한 적입니다.");
                        else
                        {
                            NormalHitMonster(monster);

                            if (monster.IsDead())
                            {
                                // 죽음 처리
                                // ui에 해당 몬스터의 인덱스를 전달해야 함
                                _left--;
                            }

                            playerTurn = false;
                        }
                    }
                }
                else ui.AddLog("잘못된 입력입니다.");
            }
        }

        private void PrintSkillOption()
        {

        }

        private void PrintInventoryOption()
        {

        }

        public void InstantiateMonster(Dungeon dungeon)
        {
            Random rnd = new Random();

            Monsters.Clear();

            // 던전 클래스 몬스터 id -> 몬스터 객체화
            for (int i = 0; i < _size; i++)
            {
                Monsters.Add(GameManager.Instance.DataManager.MakeNewMonster(dungeon.MonsterIds[rnd.Next(0, dungeon.MonsterIds.Count)]));
            }
        }

        public void NormalHitMonster(Monster monster)
        {
            Random rnd = new Random();
            Character player = GameManager.Instance.DataManager.Player;

            int damage = player.Atk + player.GetAtkBonus();

            if (rnd.NextSingle() * 100 <= player.CriticalChance)
                damage = (int)(damage * player.CriticalDamage);

            damage -= monster.Def;
            if (damage < 0) damage = 0;

            monster.ChangeHP(-damage);
        }
    }
}
