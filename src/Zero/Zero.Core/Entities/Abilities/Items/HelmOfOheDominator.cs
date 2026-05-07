using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_helm_of_the_dominator)]
    public sealed class HelmOfOheDominator : RangedItem, IAuraAbility
    {
        public HelmOfOheDominator(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_helm_of_the_dominator_aura";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }
    }
}