using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_basher)]
    public sealed class SkullBasher : PassiveItem
    {
        public SkullBasher(Item item)
            : base(item)
        {
        }
    }
}