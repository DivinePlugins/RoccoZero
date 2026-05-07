using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_radiance)]
    public sealed class Radiance : ToggleItem, IAuraAbility
    {
        public Radiance(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_radiance_debuff";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }
    }
}