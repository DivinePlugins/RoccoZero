using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_hand_of_midas)]
    public sealed class HandOfMidas : RangedItem
    {
        public HandOfMidas(Item item)
            : base(item)
        {
        }
    }
}