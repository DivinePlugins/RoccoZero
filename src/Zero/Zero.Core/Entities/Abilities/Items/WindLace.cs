using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_wind_lace)]
    public sealed class WindLace : PassiveItem
    {
        public WindLace(Item item)
            : base(item)
        {
        }
    }
}