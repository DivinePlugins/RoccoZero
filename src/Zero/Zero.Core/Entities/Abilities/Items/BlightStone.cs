using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_blight_stone)]
    public sealed class BlightStone : PassiveItem
    {
        public BlightStone(Item item)
            : base(item)
        {
        }
    }
}