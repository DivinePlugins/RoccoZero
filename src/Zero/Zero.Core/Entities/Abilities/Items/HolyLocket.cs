using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_holy_locket)]
    public sealed class HolyLocket : PassiveItem
    {
        public HolyLocket(Item item)
            : base(item)
        {
        }
    }
}