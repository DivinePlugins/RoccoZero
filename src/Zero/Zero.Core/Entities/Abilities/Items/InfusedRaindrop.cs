using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_infused_raindrop)]
    public sealed class InfusedRaindrop : PassiveItem //TODO DamageBlock
    {
        public InfusedRaindrop(Item item)
            : base(item)
        {
        }
    }
}