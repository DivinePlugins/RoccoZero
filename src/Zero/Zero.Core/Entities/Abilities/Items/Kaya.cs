using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_kaya)]
    public sealed class Kaya : PassiveItem // TODO DamageAmplify
    {
        public Kaya(Item item)
            : base(item)
        {
        }
    }
}