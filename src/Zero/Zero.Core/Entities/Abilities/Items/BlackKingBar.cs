using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_black_king_bar)]
    public sealed class BlackKingBar : ActiveItem, IHasModifier
    {
        public BlackKingBar(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.MagicImmune;

        public string ModifierName { get; } = "modifier_black_king_bar_immune";
    }
}