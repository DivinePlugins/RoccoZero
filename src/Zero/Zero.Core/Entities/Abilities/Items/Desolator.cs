using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_desolator)]
    public sealed class Desolator : PassiveItem
    {
        public Desolator(Item item)
            : base(item)
        {
        }
    }
}