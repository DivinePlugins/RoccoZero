using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_cyclone)]
    public sealed class EulsScepterOfDivinity : RangedItem, IHasTargetModifier
    {
        public EulsScepterOfDivinity(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Invulnerable;

        public override DamageType DamageType { get; } = DamageType.Magical;

        public string TargetModifierName { get; } = "modifier_eul_cyclone";
    }
}