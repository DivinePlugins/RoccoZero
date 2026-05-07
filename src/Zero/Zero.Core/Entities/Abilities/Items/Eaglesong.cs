using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_eagle)]
    public sealed class Eaglesong : PassiveItem
    {
        public Eaglesong(Item item)
            : base(item)
        {
        }
    }
}