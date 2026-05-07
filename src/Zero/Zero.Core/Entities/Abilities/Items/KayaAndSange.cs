using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_kaya_and_sange)]
    public sealed class KayaAndSange : PassiveItem // TODO DamageAmplify
    {
        public KayaAndSange(Item item)
            : base(item)
        {
        }
    }
}