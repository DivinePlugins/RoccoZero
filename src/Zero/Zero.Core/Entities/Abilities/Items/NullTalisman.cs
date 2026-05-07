using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_null_talisman)]
    public sealed class NullTalisman : PassiveItem //TODO DamageAmplify
    {
        public NullTalisman(Item item)
            : base(item)
        {
        }
    }
}