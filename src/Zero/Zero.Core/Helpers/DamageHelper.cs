using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Components;

namespace Divine.Core.Helpers
{
    public static class DamageHelpers
    {
        public static float GetDamageReduction(this CAbility ability, CUnit target)
        {
            return GetDamageReduction(ability, target, ability.DamageType);
        }

        public static float GetDamageReduction(this CAbility ability, CUnit target, DamageType damageType)
        {
            // TODO: modifiers
            if ((damageType & DamageType.HealthRemoval) == DamageType.HealthRemoval)
            {
                return 0.0f;
            }

            var reduction = 0.0f;
            switch (damageType)
            {
                case DamageType.Magical:
                    reduction = target.MagicDamageResist;
                    break;
                case DamageType.Physical:
                    reduction = target.DamageResist;
                    break;
            }

            return reduction;
        }

        public static float GetSpellDamage(float damage, float amplify = 0, float reduction = 0)
        {
            return damage * (1.0f + amplify) * (1.0f - reduction);
        }

        public static float GetSpellDamage(float damage, params float[] modifiers)
        {
            var amplify = 1.0f;
            foreach (var modifier in modifiers)
            {
                amplify *= 1.0f + modifier;
            }

            return damage * amplify;
        }

        public static float SpellAmplification(this CAbility ability)
        {
            var owner = ability.Owner;
            if (owner == null)
            {
                return 0.0f;
            }

            return owner.GetSpellAmplification();
        }
    }
}
