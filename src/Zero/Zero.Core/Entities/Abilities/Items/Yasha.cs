using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_yasha)]
    public sealed class Yasha : PassiveItem
    {
        public Yasha(Item item)
            : base(item)
        {
        }
    }
}