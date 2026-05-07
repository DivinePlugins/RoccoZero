using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_yasha_and_kaya)]
    public sealed class YashaAndKaya : PassiveItem // TODO DamageAmplify
    {
        public YashaAndKaya(Item item)
            : base(item)
        {
        }
    }
}