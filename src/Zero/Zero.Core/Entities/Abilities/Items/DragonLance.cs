using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_dragon_lance)]
    public sealed class DragonLance : PassiveItem //TODO RangeIncrease
    {
        public DragonLance(Item item)
            : base(item)
        {
        }
    }
}