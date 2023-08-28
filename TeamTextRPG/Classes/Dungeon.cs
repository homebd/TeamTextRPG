/// <summary
/// 보상이나 조건 등을 가진 던전 클래스
/// </summary>

namespace TeamTextRPG.Classes
{
    internal class Dungeon
    {
        public string Name { get; }
        public int Condition { get; }
        public List<int> Reward { get; } // Reward[0]은 골드, Reward[1]부터는 아이템 id
        public List<int> MonsterIds { get; }
        public List<Monster> Monsters { get; }


        public Dungeon(Character player, string name, int condition, int gold)
        {
            Reward = new List<int>();
            MonsterIds = new List<int>();
            Monsters = new List<Monster>();

            Name = name;
            Condition = condition;
            Reward.Add(gold);
        }

        public void AddReward(int id)
        {
            Reward.Add(id);
        }
        public void AddMonster(int id)
        {
            MonsterIds.Add(id);
        }

        public void InstantiateMonster(Monster monster)
        {
            Monsters.Add(monster);
        }
    }
}
