using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bloodthorn)]
    public sealed class Bloodthorn : RangedItem, IHasTargetModifier, IHasDamageAmplifier
    {
        public Bloodthorn(Item item)
            : base(item)
        {
        }

        public DamageType AmplifierType { get; } = DamageType.Physical | DamageType.Magical | DamageType.Pure;

        public override UnitState AppliesUnitState { get; } = UnitState.Silenced | UnitState.EvadeDisabled;

        public float DamageAmplification
        {
            get
            {
                return GetAbilitySpecialData("silence_damage_percent") / 100f;
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public string TargetModifierName { get; } = "modifier_bloodthorn_debuff";
    }
}