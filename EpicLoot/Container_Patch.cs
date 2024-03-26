using System.Linq;
using HarmonyLib;

namespace EpicLoot
{
    //public void AddDefaultItems()
    [HarmonyPatch(typeof(Container), nameof(Container.AddDefaultItems))]
    public static class Container_AddDefaultItems_Patch
    {
        // already created (on Awake call) default items should be removed first 
        public static void Prefix(Container __instance)
        {
            if (__instance == null || __instance.m_piece == null)
            {
                return;
            }

            __instance.m_inventory.RemoveAll();
        }

        public static void Postfix(Container __instance)
        {
            if (__instance == null || __instance.m_piece == null)
            {
                return;
            }

            var containerName = __instance.m_piece.name.Replace("(Clone)", "").Trim();
            var lootTables = LootRoller.GetLootTable(containerName);
            if (lootTables != null && lootTables.Count > 0)
            {
                var items = LootRoller.RollLootTable(lootTables, 1, __instance.m_piece.name, __instance.transform.position);
                EpicLoot.Log($"Rolling on loot table: {containerName}, spawned {items.Count} items at drop point({__instance.transform.position.ToString("0")}).");
                foreach (var item in items)
                {
                    __instance.m_inventory.AddItem(item);
                    EpicLoot.Log($"  - {item.m_shared.m_name}" + (item.IsMagic() ? $": {string.Join(", ", item.GetMagicItem().Effects.Select(x => x.EffectType.ToString()))}" : ""));
                }
            }
        }
    }

    // Looks like there is no need to change this since all containers are not empty initially
    // [HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]

    [HarmonyPatch(typeof(Container), nameof(Container.RPC_RequestOpen))]
    public static class Container_RPC_RequestOpen_Patch
    {
        public static void Prefix(Container __instance, long uid, long playerID)
        {
            if (__instance == null || __instance.m_piece == null)
            {
                return;
            }

            if (__instance.m_nview.IsOwner() && !__instance.m_nview.GetZDO().GetBool("EL_container_items_rolled".GetStableHashCode()))
            {
                var containerName = __instance.m_piece.name.Replace("(Clone)", "").Trim();
                var lootTables = LootRoller.GetLootTable(containerName);
                if (lootTables != null && lootTables.Count > 0)
                {
                    __instance.AddDefaultItems();
                }
                __instance.m_nview.GetZDO().Set("EL_container_items_rolled".GetStableHashCode(), value: true);
            }
        }
    }

    [HarmonyPatch(typeof(Container), nameof(Container.OnDestroyed))]
    public static class Container_OnDestroyed_Patch
    {
        public static void Prefix(Container __instance)
        {
            if (__instance == null || __instance.m_piece == null)
            {
                return;
            }

            if (__instance.m_nview.IsOwner() && !__instance.m_nview.GetZDO().GetBool("EL_container_items_rolled".GetStableHashCode()))
            {
                var containerName = __instance.m_piece.name.Replace("(Clone)", "").Trim();
                var lootTables = LootRoller.GetLootTable(containerName);
                if (lootTables != null && lootTables.Count > 0)
                {
                    __instance.AddDefaultItems();
                }
                __instance.m_nview.GetZDO().Set("EL_container_items_rolled".GetStableHashCode(), value: true);
            }
        }
    }
}
