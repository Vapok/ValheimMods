using System;
using HarmonyLib;

namespace EpicLoot.MagicItemEffects
{
	//public override bool BlockAttack(HitData hit, Character attacker)
	[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.BlockAttack))]
	public static class ModifyParry_Humanoid_BlockAttack_Patch
	{
		public static bool Override;
		public static float OriginalValue;

		public static bool Prefix(Humanoid __instance, HitData hit, Character attacker)
		{
			Override = false;
			OriginalValue = -1;

			var currentBlocker = __instance.GetCurrentBlocker();
			if (currentBlocker == null || !(__instance is Player player))
			{
				return true;
			}

			var totalParryBonusMod = 0f;
			ModifyWithLowHealth.Apply(player, MagicEffectType.ModifyParry, effect =>
			{
				if (player.HasActiveMagicEffect(effect))
				{
					if (!Override)
					{
						Override = true;
						OriginalValue = currentBlocker.m_shared.m_timedBlockBonus;
					}

					totalParryBonusMod += player.GetTotalActiveMagicEffectValue(effect, 0.01f);
				}
			});

			currentBlocker.m_shared.m_timedBlockBonus *= 1.0f + totalParryBonusMod;
			
			return true;
		}

		public static void Postfix(Humanoid __instance, HitData hit, Character attacker)
		{
			var currentBlocker = __instance.GetCurrentBlocker();
			if (currentBlocker != null && Override)
			{
				currentBlocker.m_shared.m_timedBlockBonus = OriginalValue;
			}

			Override = false;
			OriginalValue = -1;
		}
	}
}