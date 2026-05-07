using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_hurricane_pike)]
    public sealed class HurricanePike : RangedItem, IHasTargetModifier, IHasModifier
    {
        public HurricanePike(Item item)
            : base(item)
        {
        }

        public float EnemyCastRange
        {
            get
            {
                var bonusRange = CastRange - BaseCastRange;
                return GetAbilitySpecialData("cast_range_enemy") + bonusRange;
            }
        }

        public string ModifierName { get; } = "modifier_item_hurricane_pike_range";

        public float PushLength
        {
            get
            {
                return GetAbilitySpecialData("push_length");
            }
        }

        public float PushSpeed { get; } = 1500.0f;

        public string TargetModifierName { get; } = "modifier_item_hurricane_pike_active";

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < (target.IsAlly(UnitManager.Owner) ? CastRange : EnemyCastRange);
        }
    }
}