using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_energy_booster)]
    public sealed class EnergyBooster : PassiveItem
    {
        public EnergyBooster(Item item)
            : base(item)
        {
        }
    }
}