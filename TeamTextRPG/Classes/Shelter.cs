/// <summary
/// 던전 클래스처럼 회복량과 비용을 가진 피난처 클래스
/// </summary>

namespace TeamTextRPG.Classes
{
    internal class Shelter
    {
        public string Name { get; }
        public int Healing { get; }
        public int Refreshing { get; }
        public int Cost { get; }

        public Shelter(string name, int heal, int mana, int cost)
        {
            Name = name;
            Healing = heal;
            Refreshing = mana;
            Cost = cost;
        }
    }
}
