using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_tarrasque)]
    public sealed class RingOfTarrasque : PassiveItem
    {
        public RingOfTarrasque(Item item)
            : base(item)
        {
        }
    }
}