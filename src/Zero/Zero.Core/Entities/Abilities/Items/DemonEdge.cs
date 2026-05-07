using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_demon_edge)]
    public sealed class DemonEdge : PassiveItem
    {
        public DemonEdge(Item item)
            : base(item)
        {
        }
    }
}