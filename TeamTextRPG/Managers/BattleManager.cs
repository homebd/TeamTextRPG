using System.Numerics;
using System.Threading;
using TeamTextRPG.Classes;
using TeamTextRPG.Common;
using static System.Net.Mime.MediaTypeNames;

namespace TeamTextRPG.Managers
{
    internal class BattleManager
    {
        public List<Monster> Monsters = new List<Monster>();
        public Stack<Skill> SkillList = new Stack<Skill>();
        int _size;
        int _left;

        private enum BattleType
        {
            NORMAL,
            SKILL
        }

        public bool EntryBattle(Dungeon dungeon)
        {
            // 클리어 여부를 bool로 반환 false면 실패

            var ui = GameManager.Instance.UIManager;
            SkillList.Clear();
            Random rnd = new Random();
            _size = rnd.Next(1, 5);

            // 던전의 몬스터 객체화: _size에 따라 수행
            InstantiateMonster(dungeon);
            _left = Monsters.Count;

            ui.MakeBattleBox();

            BattleInput();

            return _left == 0;
        }

        private void BattleInput()
        {
            GameManager.Instance.DataManager.Player.Skills.Add(new Skill("맹독성 공격", "도트뎀", 3, SkillType.DAMAGE, Stats.ATK, -80, 10));
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
                    int targetNum;
                    switch (ret)
                    {
                        case 0:
                            return;
                        case 1:
                            targetNum = PrintBattleOption(BattleType.NORMAL);
                            if(targetNum != 0)
                            {
                                Battle(Monsters[targetNum - 1], null);
                            }
                            break;
                        case 2:
                            Skill selectedSkill = PrintSkillOption();
                            if (selectedSkill == null) continue;
                            targetNum = PrintBattleOption(BattleType.SKILL);
                            if (targetNum != 0)
                            {
                                if (targetNum == _size + 1)
                                {
                                    SkillList.Push(selectedSkill.UseSkill(
                                        GameManager.Instance.DataManager.Player,
                                        GameManager.Instance.DataManager.Player));
                                } //버프 임시로
                                else {
                                    Battle(Monsters[targetNum - 1], selectedSkill);
                                }
                            }
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

        private int PrintBattleOption(BattleType battleType)
        {
            var ui = GameManager.Instance.UIManager;
            var dm = GameManager.Instance.DataManager;
            List<string> option = new List<string>();

            for (int i = 0; i < _size; i++)
            {
                option.Add($"{i + 1}. {Monsters[i].Name}");
            }
            if (battleType == BattleType.SKILL) option.Add($"{_size+1}. {dm.Player.Name}");
            option.Add("0. 취소");

            while (true)
            {
                ui.MakeOptionBox(option);
                ui.SetCursorPositionForOption();

                string input = Console.ReadLine();
                if (int.TryParse(input, out var ret) && ret >= 0 && ret <= _size+1)
                {
                    if(ret == _size + 1)
                    {
                        if(battleType == BattleType.NORMAL)
                        {
                            ui.AddLog("잘못된 입력입니다.");
                            continue;
                        }
                    }
                    else if(ret != 0 && Monsters[ret - 1].IsDead())
                    {
                        ui.AddLog("이미 사망한 적입니다.");
                        continue;
                    }

                    return ret;
                }
                else ui.AddLog("잘못된 입력입니다.");
            }
        }

        private Skill PrintSkillOption()
        {
            var ui = GameManager.Instance.UIManager;
            var dm = GameManager.Instance.DataManager;
            List<string> option = new List<string>();
            bool playerTurn = true;

            for (int i = 0; i < dm.Player.Skills.Count ; i++)
            {
                option.Add($"{i + 1}. {dm.Player.Skills[i].Name}");
            }
            option.Add("0. 취소");

            while (true)
            {
                ui.MakeOptionBox(option);
                ui.SetCursorPositionForOption();

                string input = Console.ReadLine();
                if (int.TryParse(input, out var ret) && ret >= 0 && ret <= dm.Player.Skills.Count)
                {
                    if (ret == 0)
                    {
                        return null;
                    }
                    else
                    {
                        Skill selectedSkill = dm.Player.Skills[ret - 1];
                        if (selectedSkill.ManaCost > dm.Player.CurrentMp) ui.AddLog("내공이 부족합니다.");
                        else
                        {
                            return selectedSkill;
                        }
                    }
                }
                else ui.AddLog("잘못된 입력입니다.");
            }
        }

        private void PrintInventoryOption()
        {

        }

        public void Battle(Monster monster, Skill? skill)
        {

            var ui = GameManager.Instance.UIManager;
            var player = GameManager.Instance.DataManager.Player;

            // 플레이어 턴
            if (skill == null)
            {
                skill = new Skill("attack", "", 0, SkillType.DAMAGE, -player.GetStatValue(Stats.ATK), 1);
            }
            
            SkillList.Push(skill.UseSkill(player, monster));

            ManageSkillList();
            Console.CursorVisible = false;
            Thread.Sleep(500);
            Console.CursorVisible = true;

            //몬스터 턴
            foreach (var livingMonster in Monsters.Where(x => !x.IsDead()))
            {
                var monsterSkill = new Skill("attack", "", 0, SkillType.DAMAGE, -livingMonster.GetStatValue(Stats.ATK), 1);
                SkillList.Push(monsterSkill.UseSkill(monster, player));
            }

            ManageSkillList();
            Console.CursorVisible = false;
            Thread.Sleep(500);
            Console.CursorVisible = true;

            if (player.CurrentHp == 0)
            {
                _left = -1;
            }
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

        public void NormalHitPlayer(Monster monster)
        {
            Random rnd = new Random();
            Player player = GameManager.Instance.DataManager.Player;
            bool critical = false;

            int damage;
            // 회피 계산
            if(rnd.Next(0, 100) <= player.DodgeChance)
            {
                damage = 0;
            }
            else
            {
                damage = (int)(rnd.Next(90, 110) * 0.01f * monster.Atk);

                if (rnd.Next(0, 100) <= monster.CriticalChance)
                {
                    damage = (int)(damage * monster.CriticalDamage / 100f);
                    critical = true;
                }

                damage -= player.Def;
            }

            if (damage > 0)
            {
                player.ChangeHP(-damage);
                GameManager.Instance.UIManager.AddLog($"{monster.Name}의 {(critical ? "치명타" : "")} 공격! [데미지: {damage}]");
            }
            else
            {
                GameManager.Instance.UIManager.AddLog($"{monster.Name}의 공격은 빗나갔다!");
            }
        }

        public void ManageSkillList()
        {

            Stack<Skill> newSkillList = new Stack<Skill>();

            while(SkillList.Count > 0)
            {
                Skill token = SkillList.Pop();
                if (token.Target.IsDead()) continue;

                int value = token.DoSkill();

                switch (token.SkillType)
                {
                    case SkillType.DAMAGE:
                        if(value < 0)
                        {
                            // 스킬 데미지 적용 방식
                            value = value + token.Target.Def;
                            if (value > 0) value = 0;
                        }
                        
                        // 치명타, 회피 계산 X
                        token.Target.ChangeHP(value);
                        if (token.Target.IsDead()) KillMonster();
                        break;
                    case SkillType.BUFF:
                        token.Target.ChangeStat(token.Stat, value);
                        break;
                }
                newSkillList.Push(token);
            }

            while(newSkillList.Count > 0)
            {
                Skill token = newSkillList.Pop();
                if (token.Duration > 0)
                {
                    SkillList.Push(token);
                }
            }

            GameManager.Instance.UIManager.ShowMonsterCard(Monsters);
        }

        public void KillMonster()
        {
            _left--;
        }

        public void Damage(int damage, Character target)
        {
            Random rnd = new Random();
            Player player = GameManager.Instance.DataManager.Player;
            bool critical = false;

            #region 회피 공식
            if (rnd.Next(0, 100) <= target.GetStatValue(Stats.DODGECHANCE))
            {
                GameManager.Instance.UIManager.AddLog("공격이 빗나갔다!");
                return;
            }
            #endregion

            #region 데미지 공식
            float control = 1f - MathF.Pow(((float)target.GetStatValue(Stats.DEF) / -damage), 2);
            if (control < 0.2f) control = 0.2f;

            damage = (int)(damage * control);
            #endregion

            #region 치명타 공식
            if (rnd.Next(0, 100) <= player.CriticalChance)
            {
                damage = (int)(damage * player.CriticalDamage / 100f);
                critical = true;
            }
            #endregion

            target.ChangeHP(damage);

            GameManager.Instance.UIManager.AddLog($"{target.Name}에게 {-damage}의 {(critical ? "치명타" : "")} 데미지!");
        }
    }
}
