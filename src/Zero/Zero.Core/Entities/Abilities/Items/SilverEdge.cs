using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_silver_edge)]
    public sealed class SilverEdge : ActiveItem, IHasModifier, IHasTargetModifier
    {
        public SilverEdge(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Invisible;

        public override DamageType DamageType { get; } = DamageType.Physical;

        public string ModifierName { get; } = "modifier_item_silver_edge_windwalk";

        public string TargetModifierName { get; } = "modifier_silver_edge_debuff";
    }
}