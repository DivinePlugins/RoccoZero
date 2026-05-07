using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_talisman_of_evasion)]
    public sealed class TalismanOfEvasion : PassiveItem
    {
        public TalismanOfEvasion(Item item)
            : base(item)
        {
        }
    }
}