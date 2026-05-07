using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_boots)]
    public sealed class BootsOfSpeed : PassiveItem
    {
        public BootsOfSpeed(Item item)
            : base(item)
        {
        }
    }
}