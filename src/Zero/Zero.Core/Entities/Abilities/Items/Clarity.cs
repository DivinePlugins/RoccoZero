using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_clarity)]
    public sealed class Clarity : RangedItem, IHasTargetModifier, IHasManaRestore
    {
        public Clarity(Item item)
            : base(item)
        {
        }

        public override float Duration
        {
            get
            {
                return GetAbilitySpecialData("buff_duration");
            }
        }

        public string TargetModifierName { get; } = "modifier_clarity_potion";

        public float TotalManaRestore
        {
            get
            {
                return GetAbilitySpecialData("mana_regen") * Duration;
            }
        }
    }
}