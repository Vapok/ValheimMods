using HarmonyLib;
using JetBrains.Annotations;

namespace EpicLoot.MagicItemEffects
{
    [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
    public static class ModifyAttackSpeed_ApplyAnimationHandler_Patch
    {
        public static double ModifyAttackSpeed(Character character, double speed)
        {
            if (character is not Player player || !player.InAttack())
            {
                return speed;
            }

            var currentAttack = player.m_currentAttack;
            if (currentAttack == null)
            {
                return speed;
            }

            double bonus = 0.0;
            
            ModifyWithLowHealth.Apply(player, MagicEffectType.ModifyAttackSpeed, effect =>
            {
                bonus += player.GetTotalActiveMagicEffectValue(effect, 0.01f);
            });

            speed *= (1.0 + bonus);

            return speed;
        }
        [UsedImplicitly]
        private static void Postfix(Game __instance)
        {
            AnimationSpeedManager.Add((character, speed) => ModifyAttackSpeed(character,speed));
        }
    }
}
