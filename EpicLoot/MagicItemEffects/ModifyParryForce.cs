using System;
using HarmonyLib;

namespace EpicLoot.MagicItemEffects
{
	//public float GetDeflectionForce(int quality)
	[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetDeflectionForce), typeof(int))]
	public static class ModifyParryForce_ItemData_GetDeflectionForce_Patch
	{
		public static void Postfix(ItemDrop.ItemData __instance, ref float __result)
        {
            var player = PlayerExtensions.GetPlayerWithEquippedItem(__instance);
            var totalParryForceMod = 0f;
			ModifyWithLowHealth.Apply(player, MagicEffectType.ModifyParryForce, effect =>
            {
                totalParryForceMod += MagicEffectsHelper.GetTotalActiveMagicEffectValueForWeapon(player, __instance, effect, 0.01f);
            });

			__result *= 1.0f + totalParryForceMod;
            if (player != null && player.m_leftItem == null && MagicEffectsHelper.HasActiveMagicEffect(player, __instance, MagicEffectType.Duelist))
			{
				__result += __instance.GetDamage().GetTotalDamage() / 2 * MagicEffectsHelper.GetTotalActiveMagicEffectValueForWeapon(player, __instance, MagicEffectType.Duelist, 0.01f);
			}

			__result = (float) Math.Round(__result, 1);
		}
	}
}