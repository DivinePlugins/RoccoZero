using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_helm_of_iron_will)]
    public sealed class HelmOfIronWill : PassiveItem
    {
        public HelmOfIronWill(Item item)
            : base(item)
        {
        }
    }
}