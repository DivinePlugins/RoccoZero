using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_staff_of_wizardry)]
    public sealed class StaffOfWizardry : PassiveItem
    {
        public StaffOfWizardry(Item item)
            : base(item)
        {
        }
    }
}