using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ancient_janggo)]
    public sealed class DrumOfEndurance : ActiveItem, IAreaOfEffectAbility, IHasModifier, IAuraAbility
    {
        public DrumOfEndurance(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_ancient_janggo_aura_effect";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }

        public override bool CanBeCasted
        {
            get
            {
                return CurrentCharges > 0 && base.CanBeCasted;
            }
        }

        public string ModifierName { get; } = "modifier_item_ancient_janggo_active";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }
    }
}