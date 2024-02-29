using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicLoot
{
    [Serializable]
    public class LootFilter
    {
        public string Name;
        public bool Whitelist; 
        public bool DropMaterials;
        public List<string> Items = new List<string>();
        public List<ItemRarity> Rarities = new List<ItemRarity>();
        public List<ItemQuality> Quality = new List<ItemQuality>();
        public int Distance;
        // public int Day; to be implemented for full accordance with CreatureLevelControl world level settings, that is, distance or day based
    }

    [Serializable]
    public class LootFiltersConfig
    {
        public List<LootFilter> LootFilters = new List<LootFilter>();
    }

    public class LootFilterDefinitions {
        public static readonly List<LootFilter> BlackLists = new List<LootFilter>();
        public static readonly List<LootFilter> WhiteLists = new List<LootFilter>();

        public static void Initialize(LootFiltersConfig _config)
        {
            foreach(var item in _config?.LootFilters)
            {
                if(item.Whitelist)
                {
                    WhiteLists.Add(item);
                }
                else
                {
                    BlackLists.Add(item);
                }
            }
        }

        public static bool FilteredOut(string itemName, ItemRarity rarity, ItemQuality quality, int distance, out bool dropMaterials)
        {
            bool ItemMatches(LootFilter item, out bool dropMats)
            {
                dropMats = item.DropMaterials;
                return item.Items.Contains(itemName) && item.Rarities.Contains(rarity) && item.Quality.Contains(quality) && distance > item.Distance;
            }

            foreach (var item in WhiteLists)
            {
                if(ItemMatches(item, out dropMaterials))
                {
                    return false;
                }
            }

            foreach (var item in BlackLists)
            {
                if (ItemMatches(item, out dropMaterials))
                {
                    return true;
                }
            }

            dropMaterials = false;
            return false;
        }
    }
}
