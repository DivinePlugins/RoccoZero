using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_soul_booster)]
    public sealed class SoulBooster : PassiveItem
    {
        public SoulBooster(Item item)
            : base(item)
        {
        }
    }
}