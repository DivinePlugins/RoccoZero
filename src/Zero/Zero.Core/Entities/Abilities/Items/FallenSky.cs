using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_abyssal_blade)]
    public sealed class FallenSky : RangedItem
    {
        public FallenSky(Item item)
            : base(item)
        {
        }
    }
}