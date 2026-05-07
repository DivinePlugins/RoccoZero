using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_sobi_mask)]
    public sealed class SagesMask : PassiveItem
    {
        public SagesMask(Item item)
            : base(item)
        {
        }
    }
}