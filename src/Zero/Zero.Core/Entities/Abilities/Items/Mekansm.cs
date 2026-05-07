using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_mekansm)]
    public sealed class Mekansm : ActiveItem, IAreaOfEffectAbility, IAuraAbility, IHasModifier, IHasHealthRestore
    {
        public Mekansm(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_mekansm_aura";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }

        public string ModifierName { get; } = "modifier_item_mekansm_spell";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("heal_radius");
            }
        }

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("heal_amount");
            }
        }
    }
}