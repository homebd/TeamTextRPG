﻿using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
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
        public Stack<Skill> SkillStack = new Stack<Skill>();
        private Dungeon currentDgn;
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
            currentDgn = dungeon;

            var ui = GameManager.Instance.UIManager;
            SkillStack.Clear();
            Monsters.Clear();
            ui.PrintTitle($"[{currentDgn.Name}]", ConsoleColor.Green);
            ui.PrintDescription(currentDgn.Description);
            Random rnd = new Random();
            _size = rnd.Next(1, 5);

            // 던전의 몬스터 객체화: _size에 따라 수행
            InstantiateMonster(currentDgn);
            _left = Monsters.Count;

            ui.MakeBattleBox();

            BattleInput();

            return _left == 0;
        }

        private void BattleInput()
        {
            var ui = GameManager.Instance.UIManager;

            List<string> option = new List<string>
            {
                "1. 일반 공격",
                "2. 특수 공격",
                "3. 소모품",
                "0. 도망가기"
            };

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
                    Player player = GameManager.Instance.DataManager.Player;
                    switch (ret)
                    {
                        case 0:
                            Random rnd = new Random();
                            
                            // 100% HP -> 40% RUN / 20% HP -> 80% RUN
                            if(rnd.Next(0, 100) < (90 - 100f * player.CurrentHp / player.GetStatValue(Stats.MAXHP) / 2))
                            {
                                ui.AddLog("성공적으로 도망쳤습니다!");
                                return;
                            }
                            else
                            {
                                ui.AddLog("붙잡혔습니다!");
                                Battle(null, null);
                                break;
                            }
                        case 1:
                            targetNum = PrintBattleOption(BattleType.NORMAL);
                            if(targetNum != 0)
                            {
                                Battle(Monsters[targetNum - 1], null);
                            }
                            break;
                        case 2:
                            #region 스킬창 열고 입력 받고 닫고 다 해무라
                            ui.MakeTab();
                            ui.PrintSkills();

                            int skillNum;
                            Skill? selectedSkill = null;
                            while (true)
                            {
                                ui.SetCursorPositionForOption();

                                if (int.TryParse(Console.ReadLine(), out skillNum) && skillNum >= 0 && skillNum <= player.Skills.Count)
                                {
                                    if (skillNum == 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        selectedSkill = player.Skills[skillNum - 1];
                                        if (selectedSkill.ManaCost > player.CurrentMp) ui.AddLog("내공이 부족합니다.");
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else ui.AddLog("잘못된 입력입니다.");
                            }

                            LoadBattle();

                            #endregion
                            if (selectedSkill == null) continue;
                            targetNum = PrintBattleOption(BattleType.SKILL);
                            if (targetNum != 0)
                            {
                                if (targetNum == _size + 1)
                                {
                                    Battle(GameManager.Instance.DataManager.Player, selectedSkill);
                                }
                                else {
                                    Battle(Monsters[targetNum - 1], selectedSkill);
                                }
                            }
                            break;
                        case 3:
                            PrintUseableOption();
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

        private void PrintUseableOption()
        {
            var ui = GameManager.Instance.UIManager;
            var dm = GameManager.Instance.DataManager;
            var player = GameManager.Instance.DataManager.Player;
            ui.MakeTab();
            ui.PrintUseables();
            int input;


            while (true)
            {
                ui.SetCursorPositionForOption();

                if (int.TryParse(Console.ReadLine(), out input) && input >= 0 && input <= dm.SortedItems.Count)
                {
                    if (input == 0)
                    {
                        LoadBattle();
                        break;
                    }
                    else
                    {
                        int ItemId = dm.SortedItems[input - 1].Id;

                        switch (ItemId)
                        {
                            //스테로이드
                            case 92:
                                Skill atkBuff = new Skill("공격력 상승", "", 0, SkillType.BUFF, Stats.ATK, 5, 1, false);
                                ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                                Battle(player, atkBuff);
                                break;
                            //철분제
                            case 93:
                                Skill defBuff = new Skill("방어력 상승", "", 0, SkillType.BUFF, Stats.DEF, 5, 1, false);
                                ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                                Battle(player, defBuff);
                                break;
                            // 수류탄
                            case 94:
                                int targetNum = PrintBattleOption(BattleType.SKILL);
                                Skill grade = new Skill("수류탄", "", 0, SkillType.DAMAGE, -100, 1, true);
                                ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                                Battle(Monsters[targetNum - 1], grade);
                                break;
                            //연막탄, 회피율 증가
                            case 95:
                                Skill SmokeShell = new Skill("연막탄", "", 0, SkillType.BUFF, Stats.DODGECHANCE, 10, 1, true);
                                ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                                Battle(player, SmokeShell);
                                break;
                            //독안개
                            case 96:
                                targetNum = PrintBattleOption(BattleType.SKILL);
                                Skill poisonMist = new Skill("독안개", "", 0, SkillType.BUFF, Stats.ATK, -5, 1, true);
                                ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                                Battle(Monsters[targetNum - 1], poisonMist);
                                break;
                            default:
                                ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                                break;
                        }

                        dm.Player.Wear(dm.SortedItems[input - 1]);
                       // ui.AddLog($"{dm.SortedItems[input - 1].Name}을 사용했습니다.");
                        //ui.PrintUseables();
                    }
                }
                else { ui.AddLog("잘못된 입력입니다."); }
                LoadBattle();
                break;
             
            }
        }

        public void Battle(Character? target, Skill? skill)
        {
            var ui = GameManager.Instance.UIManager;
            var player = GameManager.Instance.DataManager.Player;

            // 도망 실패로 배틀에 끌려왔을 시 플레이어 턴 무시
            if(target != null)
            {
                //플레이어 턴
                if (skill == null)
                {
                    skill = new Skill("공격", "", 0, SkillType.DAMAGE, -player.GetStatValue(Stats.ATK), 1, false);
                }

                SkillStack.Push(skill.UseSkill(player, target));

                if (skill.IsAoE && target != player)
                {
                    int index = Monsters.IndexOf((Monster)target);

                    if (Monsters.Count > 1)
                    {
                        if (index == 0)
                        {
                            SkillStack.Push(skill.UseSkill(player, Monsters[index + 1]));
                        }
                        else if (index == Monsters.Count - 1)
                        {
                            SkillStack.Push(skill.UseSkill(player, Monsters[index - 1]));
                        }
                        else
                        {
                            SkillStack.Push(skill.UseSkill(player, Monsters[index - 1]));
                            SkillStack.Push(skill.UseSkill(player, Monsters[index + 1]));
                        }
                    }
                }

                ManageSkillList();
                Console.CursorVisible = false;
                Thread.Sleep(100);
                Console.CursorVisible = true;
            }
            
            if(_left == 0)
            {
                return;
            }

            //몬스터 턴
            foreach (var livingMonster in Monsters.Where(x => !x.IsDead()))
            {
                var monsterSkill = new Skill("공격", "", 0, SkillType.DAMAGE, -livingMonster.GetStatValue(Stats.ATK), 1, false);
                SkillStack.Push(monsterSkill.UseSkill(livingMonster, player));
            }

            ManageSkillList();

            Console.CursorVisible = false;
            Thread.Sleep(100);
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

        public void ManageSkillList()
        {
            Stack<Skill> newSkillStack = new Stack<Skill>();
            var ui = GameManager.Instance.UIManager;

            // 이거때문에 오류 났는데 어떤 용도로 있는것인지?
            //GameManager.Instance.DataManager.Player.BuffStat = new int[] { 0, };

            // 일단 버프를 정렬
            SkillStack = new Stack<Skill>(SkillStack.OrderByDescending(x => x.SkillType).ToList());

            while(SkillStack.Count > 0)
            {
                Skill token = SkillStack.Pop();
                if (token.Target.IsDead()) continue;

                int value = token.DoSkill();
             
                switch (token.SkillType)
                {
                    case SkillType.DAMAGE:
                        if(value < 0)
                        {
                            Damage(value, token);
                            
                        }
                        else if(value > 0)
                        {
                            token.Target.ChangeHP(value);
                        }
                        
                        if (token.Target.IsDead())
                        {
                            KillMonster();
                        }
                        break;
                    case SkillType.BUFF:
                        token.Target.ChangeStat(token.Stat, value);
                        break;
                }
                newSkillStack.Push(token);
            }

            while(newSkillStack.Count > 0)
            {
                Skill token = newSkillStack.Pop();
                if (token.Duration > 0)
                {
                    SkillStack.Push(token);
                }
            }

            GameManager.Instance.UIManager.ShowMonsterCard(Monsters);
        }

        public void KillMonster()
        {
            _left--;
        }
    
        public void Damage(int damage, Skill skill)
        {
            if (damage < 0)
            {

                Random rnd = new Random();
                bool critical = false;

                #region 회피 공식
                if (rnd.Next(0, 100) <= skill.Target.GetStatValue(Stats.DODGECHANCE))
                {
                    GameManager.Instance.UIManager.AddLog($"{skill.Caster.Name}의 공격은 빗나갔다!");
                    return;
                }
                #endregion

                #region 데미지 공식
                int targetDef = skill.Target.GetStatValue(Stats.DEF);
                int characterATK = skill.Caster.GetStatValue(Stats.ATK);
                float input = 0f;
                float formulaResult = 0f;
                float randomDamageRange = (float)(rnd.NextDouble() * 0.4f) + 0.8f;

                if (targetDef > characterATK)
                {
                    // 해당 공식대로면 상대방의 방어력이 내 공격력보다 55% 가량 높으면 최소데미지인 40%에 가까운 데미지가 들어감.
                    input = ((float)-characterATK / targetDef) + 1f;
                    formulaResult = 1 - (float)((Math.Exp(input * 4) / (Math.Exp(input * 4) + 1)) - 0.5) * 2;
                    // 최소 데미지는 공격력의 40%
                    if (formulaResult < 0.4f)
                        formulaResult = 0.4f;
                    float proportion = (characterATK * formulaResult * randomDamageRange) / characterATK;
                    damage = (int)(damage * proportion);
                    // 최소데미지 1 보장
                    if (damage == 0)
                        damage--;
                }
                else
                {
                    // 해당 공식대로면 내 공격력이 상대 방어력보다 33% 가량 높으면 약 150%에 해당하는 데미지가 들어감. 65%높으면 175%가 들어감.
                    input = 1f - ((float)characterATK / targetDef);
                    formulaResult = 1 + (-(float)((Math.Exp(input * 3.3) / (Math.Exp(input * 3.3) + 1)) - 0.5) * 2);
                    float proportion = (characterATK * formulaResult * randomDamageRange) / characterATK;
                    damage = (int)(damage * proportion);
                    // 최소데미지 1 보장
                    if (damage == 0)
                        damage--;
                }
                #endregion


                #region 치명타 공식

                if (rnd.Next(0, 100) <= skill.Caster.CriticalChance)
                {
                    damage = (int)(damage * skill.Caster.CriticalDamage / 100f);
                    critical = true;
                }
                #endregion

                if (skill.Caster == GameManager.Instance.DataManager.Player)
                {
                    GameManager.Instance.UIManager.AddLog($"[{skill.Name}]{skill.Target.Name}에게 {-damage} {(critical ? "치명타 " : "")}피해!");
                }
                else
                {
                    GameManager.Instance.UIManager.AddLog($"{skill.Caster.Name}의 {skill.Name}! {-damage} {(critical ? "치명타 " : "")}피해!");
                }
            }
            else
                GameManager.Instance.UIManager.AddLog($"{skill.Caster.Name}의 {skill.Name}! {damage} 회복!");
            skill.Target.ChangeHP(damage);
        }

        public void LoadBattle()
        {
            var ui = GameManager.Instance.UIManager;

            Console.Clear();
            ui.PrintTitle($"[{currentDgn.Name}]", ConsoleColor.Green);
            ui.PrintDescription(currentDgn.Description);
            ui.MakeBattleBox();
            ui.MakeLogBox();
            PrintPlayerUI();
            ui.ShowMonsterCard(Monsters);
        }
    }
}
