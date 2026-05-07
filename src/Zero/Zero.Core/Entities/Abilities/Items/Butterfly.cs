using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_butterfly)]
    public sealed class Butterfly : PassiveItem
    {
        public Butterfly(Item item)
            : base(item)
        {
        }
    }
}