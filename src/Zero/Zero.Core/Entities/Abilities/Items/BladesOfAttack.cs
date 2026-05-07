using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_blades_of_attack)]
    public sealed class BladesOfAttack : PassiveItem
    {
        public BladesOfAttack(Item item)
            : base(item)
        {
        }
    }
}