using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bracer)]
    public sealed class Bracer : PassiveItem
    {
        public Bracer(Item item)
            : base(item)
        {
        }
    }
}