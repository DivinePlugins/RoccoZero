using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_belt_of_strength)]
    public sealed class BeltOfStrength : PassiveItem
    {
        public BeltOfStrength(Item item)
            : base(item)
        {
        }
    }
}