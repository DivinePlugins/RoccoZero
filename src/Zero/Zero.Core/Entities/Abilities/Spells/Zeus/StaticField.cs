using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

namespace Divine.Core.Entities.Abilities.Spells.Zeus
{
    [Spell(AbilityId.zuus_static_field, HeroId.npc_dota_hero_zuus)]
    public sealed class StaticField : PassiveSpell
    {
        public StaticField(Ability ability)
            : base(ability)
        {
        }

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialDataWithTalent("damage_health_pct") / 100f;
            }
        }

        public override float GetDamage(params CUnit[] targets)
        {
            var damagePercent = RawDamage;
            var amplify = Owner.GetSpellAmplification();

            var totalDamage = 0.0f;
            foreach (var target in targets)
            {
                var reduction = this.GetDamageReduction(target, DamageType);
                var damage = damagePercent * target.Health;
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }

        public override float GetDamage(CUnit target, float damageModifier, float targetHealth = float.MinValue)
        {
            if (targetHealth == float.MinValue)
            {
                targetHealth = target.Health;
            }

            var damagePercent = RawDamage;
            var damage = damagePercent * targetHealth;
            var amplify = Owner.GetSpellAmplification();
            var reduction = this.GetDamageReduction(target, DamageType);

            return DamageHelpers.GetSpellDamage(damage, amplify, -reduction, damageModifier);
        }
    }
}