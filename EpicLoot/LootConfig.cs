﻿using System;
using System.Collections.Generic;

namespace EpicLoot
{
    [Serializable]
    public class LootDrop
    {
        public string Item;
        public float Weight = 1;
        public float[] Rarity;
        public float[] Quality;
    }

    [Serializable]
    public class LeveledLootDef
    {
        public int Level;
        public float[][] Drops;
        public LootDrop[] Loot;
    }

    [Serializable]
    public class LootTable
    {
        public string Object;
        public string RefObject;
        public float[][] Drops;
        public float[][] Drops2;
        public float[][] Drops3;
        public LootDrop[] Loot;
        public LootDrop[] Loot2;
        public LootDrop[] Loot3;
        public List<LeveledLootDef> LeveledLoot = new List<LeveledLootDef>();
    }

    [Serializable]
    public class LootItemSet
    {
        public string Name;
        public LootDrop[] Loot;
    }

    [Serializable]
    public class MagicEffectsCountConfig
    {
        public float[][] Magic;
        public float[][] Rare;
        public float[][] Epic;
        public float[][] Legendary;
        public float[][] Mythic;
    }

    [Serializable]
    public class LootConfig
    {
        public MagicEffectsCountConfig MagicEffectsCount;
        public MagicEffectsCountConfig MagicEffectsCountExceptional;
        public MagicEffectsCountConfig MagicEffectsCountElite;
        public LootItemSet[] ItemSets;
        public LootTable[] LootTables;
        public List<string> RestrictedItems = new List<string>();
    }
}
