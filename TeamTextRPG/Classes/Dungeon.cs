﻿
using System.Text.Json.Serialization;
/// <summary
/// 보상이나 조건 등을 가진 던전 클래스
/// </summary>
namespace TeamTextRPG.Classes
{
    internal class Dungeon
    {
        public string Name { get; }
        public string Description { get; }
        public int Condition { get; }
        public List<int> Reward { get; } // Reward[0]은 골드, Reward[1]부터는 아이템 id
        public List<int> MonsterIds { get; }

        // NOTE : 아래와 같은 Monster 리스트가 있어야 ExploreDungeon에서 올바른 결과 출력이 가능합니다.

        public Dungeon(string name, string description, int condition)
        {
            Reward = new List<int>();
            MonsterIds = new List<int>();

            Name = name;
            Description = description;
            Condition = condition;
        }

        public void AddReward(int id)
        {
            Reward.Add(id);
        }
        public void AddMonster(int id)
        {
            MonsterIds.Add(id);
        }
    }
}
