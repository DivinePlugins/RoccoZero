using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_mystic_staff)]
    public sealed class MysticStaff : PassiveItem
    {
        public MysticStaff(Item item)
            : base(item)
        {
        }
    }
}