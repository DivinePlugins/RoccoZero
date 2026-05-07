using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_regen)]
    public sealed class RingOfRegen : PassiveItem
    {
        public RingOfRegen(Item item)
            : base(item)
        {
        }
    }
}