using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ultimate_scepter_2)]
    public sealed class AghanimsBlessing : PassiveItem
    {
        public AghanimsBlessing(Item item)
            : base(item)
        {
        }
    }
}