using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace EpicLoot.MagicItemEffects
{
	[HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
	public class AvoidDamageTaken_Character_RPC_Damage_Patch
	{
		[UsedImplicitly]
		private static bool Prefix(Character __instance, HitData hit)
		{
			var attacker = hit.GetAttacker();
			if (__instance is Player player && attacker != null && attacker != __instance)
			{
				var avoidancePercent = player.GetTotalActiveMagicEffectValue(MagicEffectType.AvoidDamageTaken, 1.0f);
                var avoidancePercentLowHealth = ModifyWithLowHealth.PlayerHasLowHealth(player) ? player.GetTotalActiveMagicEffectValue(MagicEffectType.AvoidDamageTakenLowHealth, 1.0f) : 0.0f;

				float min, max;
				if(avoidancePercent > avoidancePercentLowHealth)
				{
					min = avoidancePercentLowHealth;
					max = avoidancePercent;
				}
				else
				{
                    min = avoidancePercent;
                    max = avoidancePercentLowHealth;
                }

				float avoidanceChance = (max + (100.0f - max) * min * 0.01f) * 0.01f;

                return !(Random.Range(0f, 1f) < avoidanceChance);
			}

			return true;
		}
	}
}