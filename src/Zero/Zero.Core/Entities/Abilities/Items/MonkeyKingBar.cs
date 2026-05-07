using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_monkey_king_bar)]
    public sealed class MonkeyKingBar : PassiveItem
    {
        public MonkeyKingBar(Item item)
            : base(item)
        {
        }
    }
}