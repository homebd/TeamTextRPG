namespace TeamTextRPG.Common
{
    public enum Parts
    {
        WEAPON,
        HELMET,
        CHESTPLATE,
        LEGGINGS,
        BOOTS
    }
    public enum JOB
    {
        WARRIOR,
        WIZARD,
        ARCHER
    }
    public enum SkillType
    {
        DAMAGE,
        BUFF
    }
    public enum ValueTypeEnum
    {
        PROPOTIONAL,
        FIXED // NOT TRUE(PURE) VALUE
    }
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

    public enum Stats
    {
        MAXHP,
        MAXMP,
        ATK,
        DEF,
        CRITICALCHANCE,
        CRITICALDAMAGE,
        DODGECHANCE
    }
}
