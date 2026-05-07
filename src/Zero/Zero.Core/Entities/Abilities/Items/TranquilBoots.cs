using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_tranquil_boots)]
    public sealed class TranquilBoots : PassiveItem
    {
        public TranquilBoots(Item item)
            : base(item)
        {
        }
    }
}