using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_orchid)]
    public sealed class OrchidMalevolence : RangedItem, IHasTargetModifier, IHasDamageAmplifier
    {
        public OrchidMalevolence(Item item)
            : base(item)
        {
        }

        public DamageType AmplifierType { get; } = DamageType.All;

        public override UnitState AppliesUnitState { get; } = UnitState.Silenced;

        public float DamageAmplification
        {
            get
            {
                return GetAbilitySpecialData("silence_damage_percent") / 100f;
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public string TargetModifierName { get; } = "modifier_orchid_malevolence_debuff";
    }
}