using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_shadow_amulet)]
    public sealed class ShadowAmulet : RangedItem, IHasTargetModifier
    {
        public ShadowAmulet(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Invisible;

        public string TargetModifierName { get; } = "modifier_item_shadow_amulet_fade";
    }
}