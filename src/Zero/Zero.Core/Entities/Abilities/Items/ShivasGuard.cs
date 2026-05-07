using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_shivas_guard)]
    public sealed class ShivasGuard : ActiveItem, IAreaOfEffectAbility, IAuraAbility, IHasTargetModifier
    {
        public ShivasGuard(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_shivas_guard_aura";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("blast_radius");
            }
        }

        public override float Speed
        {
            get
            {
                return GetAbilitySpecialData("blast_speed");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_shivas_guard_blast";

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialData("blast_damage");
            }
        }

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < Radius;
        }

        public override float GetDamage(params CUnit[] targets)
        {
            var totalDamage = 0.0f;

            var damage = RawDamage;
            var amplify = Owner.GetSpellAmplification();
            foreach (var target in targets)
            {
                var reduction = this.GetDamageReduction(target, DamageType);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }
    }
}