using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_health)]
    public sealed class RingOfHealth : PassiveItem
    {
        public RingOfHealth(Item item)
            : base(item)
        {
        }
    }
}