using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_urn_of_shadows)]
    public sealed class UrnOfShadows : RangedItem, IHasDot, IHasHealthRestore
    {
        public UrnOfShadows(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return CurrentCharges > 0 && base.CanBeCasted;
            }
        }

        public float DamageDuration
        {
            get
            {
                return Duration;
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public override float Duration
        {
            get
            {
                return GetAbilitySpecialData("soul_damage_duration");
            }
        }

        public bool HasInitialDamage { get; } = false;

        public float RawTickDamage
        {
            get
            {
                return GetTotalDamage() / DamageDuration;
            }
        }

        // modifier_item_urn_heal
        public string TargetModifierName { get; } = "modifier_item_urn_damage";

        public float TickRate { get; } = 1.0f;

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("soul_heal_amount") * Duration;
            }
        }

        public float GetTickDamage(params CUnit[] targets)
        {
            return RawTickDamage;
        }

        public float GetTotalDamage(params CUnit[] targets)
        {
            return GetAbilitySpecialData("soul_damage_amount");
        }
    }
}