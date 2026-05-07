using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_assault)]
    public sealed class AssaultCuirass : AuraItem
    {
        public AssaultCuirass(Item item)
            : base(item)
        {
        }

        public override string AuraModifierName { get; } = "modifier_item_assault_negative_armor";
    }
}