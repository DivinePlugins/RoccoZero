using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_solar_crest)]
    public sealed class SolarCrest : RangedItem, IHasTargetModifier
    {
        public SolarCrest(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_solar_crest_armor_reduction";
    }
}