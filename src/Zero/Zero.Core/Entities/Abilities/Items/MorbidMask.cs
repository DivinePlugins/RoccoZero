using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_lifesteal)]
    public sealed class MorbidMask : PassiveItem
    {
        public MorbidMask(Item item)
            : base(item)
        {
        }
    }
}