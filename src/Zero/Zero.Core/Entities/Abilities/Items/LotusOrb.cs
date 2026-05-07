using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_lotus_orb)]
    public sealed class LotusOrb : RangedItem, IHasTargetModifier
    {
        public LotusOrb(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_lotus_orb_active";
    }
}