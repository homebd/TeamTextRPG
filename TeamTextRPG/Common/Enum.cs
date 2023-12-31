﻿namespace TeamTextRPG.Common
{
    public enum Parts
    {
        WEAPON,
        HELMET,
        CHESTPLATE,
        LEGGINGS,
        BOOTS,
        USEABLE
    }
    public enum UsableItemTypes
    {
        HEAL_HP,
        HEAL_MP,
        DAMAGE,
        ATTACK_BUFF,
        DEFENCE_BUFF,
        CRITICAL_CHANCE_BUFF,
        CRITICAL_DAMAGE_BUFF,
        DODGE_CHANCE_BUFF,

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
