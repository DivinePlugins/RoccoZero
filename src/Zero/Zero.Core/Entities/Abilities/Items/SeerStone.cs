using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_seer_stone)]
    public sealed class SeerStone : PassiveItem
    {
        public SeerStone(Item item)
            : base(item)
        {
        }
    }
}