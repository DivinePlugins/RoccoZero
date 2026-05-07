using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_pers)]
    public sealed class Perseverance : PassiveItem
    {
        public Perseverance(Item item)
            : base(item)
        {
        }
    }
}