using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_guardian_greaves)]
    public sealed class GuardianGreaves : ActiveItem, IAreaOfEffectAbility, IHasModifier, IAuraAbility, IHasHealthRestore, IHasManaRestore
    {
        public GuardianGreaves(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_guardian_greaves_aura";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }

        public string ModifierName { get; } = "modifier_item_mekansm_noheal";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("replenish_radius");
            }
        }

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("replenish_health");
            }
        }

        public float TotalManaRestore
        {
            get
            {
                return GetAbilitySpecialData("replenish_mana");
            }
        }
    }
}