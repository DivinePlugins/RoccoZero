using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_flask)]
    public sealed class HealingSalve : RangedItem, IHasTargetModifier, IHasHealthRestore
    {
        public HealingSalve(Item item)
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

        public string TargetModifierName { get; } = "modifier_flask_healing";

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("health_regen") * this.Duration;
            }
        }
    }
}