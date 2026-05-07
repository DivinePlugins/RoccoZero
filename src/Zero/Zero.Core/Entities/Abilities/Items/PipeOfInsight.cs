using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_pipe)]
    public sealed class PipeOfInsight : ActiveItem, IAreaOfEffectAbility, IHasModifier, IAuraAbility
    {
        public PipeOfInsight(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_pipe_aura";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }

        public string ModifierName { get; } = "modifier_item_pipe_barrier";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("barrier_radius");
            }
        }
    }
}