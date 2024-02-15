using System;
using System.Collections.Generic;
using HarmonyLib;

namespace EpicLoot.MagicItemEffects
{
    //public override void ApplyArmorDamageMods(ref HitData.DamageModifiers mods)
    [HarmonyPatch(typeof(Player), nameof(Player.ApplyArmorDamageMods))]
    public static class ModifyResistance_Player_ApplyArmorDamageMods_Patch
    {
        public static void Postfix(Player __instance, ref HitData.DamageModifiers mods)
        {
            var damageMods = new List<HitData.DamageModPair>();

            if (__instance.HasActiveMagicEffect(MagicEffectType.AddFireResistance))
            {
                damageMods.Add(new HitData.DamageModPair() { m_type = HitData.DamageType.Fire, m_modifier = HitData.DamageModifier.Resistant});
            }
            if (__instance.HasActiveMagicEffect(MagicEffectType.AddFrostResistance))
            {
                damageMods.Add(new HitData.DamageModPair() { m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.Resistant });
            }
            if (__instance.HasActiveMagicEffect(MagicEffectType.AddLightningResistance))
            {
                damageMods.Add(new HitData.DamageModPair() { m_type = HitData.DamageType.Lightning, m_modifier = HitData.DamageModifier.Resistant });
            }
            if (__instance.HasActiveMagicEffect(MagicEffectType.AddPoisonResistance))
            {
                damageMods.Add(new HitData.DamageModPair() { m_type = HitData.DamageType.Poison, m_modifier = HitData.DamageModifier.Resistant });
            }
            if (__instance.HasActiveMagicEffect(MagicEffectType.AddSpiritResistance))
            {
                damageMods.Add(new HitData.DamageModPair() { m_type = HitData.DamageType.Spirit, m_modifier = HitData.DamageModifier.Resistant });
            }

            mods.Apply(damageMods);
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    public static class ModifyResistance_Character_RPC_Damage_Patch
    {
	    public static void Prefix(Character __instance, HitData hit)
	    {
		    if (!(__instance is Player player))
		    {
			    return;
		    }

            float resultingDamageCoeff(string eff1, string eff2)
            {
                float totalSum = PlayerExtensions.GetEffectDiminishingReturnsTotalValue(new List<float>()
                {
                    player.GetTotalActiveMagicEffectValue(eff1, 0.01f),
                    player.GetTotalActiveMagicEffectValue(eff2, 0.01f)
                });

                return 1.0f - totalSum;
            }

            // elemental resistances
            hit.m_damage.m_fire *= resultingDamageCoeff(MagicEffectType.AddFireResistancePercentage, MagicEffectType.AddElementalResistancePercentage);
		    hit.m_damage.m_frost *= resultingDamageCoeff(MagicEffectType.AddFrostResistancePercentage, MagicEffectType.AddElementalResistancePercentage);
		    hit.m_damage.m_lightning *= resultingDamageCoeff(MagicEffectType.AddLightningResistancePercentage, MagicEffectType.AddElementalResistancePercentage);
		    hit.m_damage.m_poison *= resultingDamageCoeff(MagicEffectType.AddPoisonResistancePercentage, MagicEffectType.AddElementalResistancePercentage);
		    hit.m_damage.m_spirit *= resultingDamageCoeff(MagicEffectType.AddSpiritResistancePercentage, MagicEffectType.AddElementalResistancePercentage);
		    
		    // physical resistances
		    hit.m_damage.m_blunt *= resultingDamageCoeff(MagicEffectType.AddBluntResistancePercentage, MagicEffectType.AddPhysicalResistancePercentage);
		    hit.m_damage.m_slash *= resultingDamageCoeff(MagicEffectType.AddSlashingResistancePercentage, MagicEffectType.AddPhysicalResistancePercentage);
		    hit.m_damage.m_pierce *= resultingDamageCoeff(MagicEffectType.AddPiercingResistancePercentage, MagicEffectType.AddPhysicalResistancePercentage);
		    hit.m_damage.m_chop *= resultingDamageCoeff(MagicEffectType.AddChoppingResistancePercentage, MagicEffectType.AddPhysicalResistancePercentage);
	    }
    }
}
