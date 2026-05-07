using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_spirit_vessel)]
    public sealed class SpiritVessel : RangedItem, IHasDot, IHasHealthRestore
    {
        public SpiritVessel(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return CurrentCharges > 0 && base.CanBeCasted;
            }
        }

        public float DamageDuration
        {
            get
            {
                return Duration;
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public override float Duration
        {
            get
            {
                return GetAbilitySpecialData("duration");
            }
        }

        public bool HasInitialDamage { get; } = false;

        public float RawTickDamage
        {
            get
            {
                return GetAbilitySpecialData("soul_damage_amount");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_spirit_vessel_damage";

        public float TickRate { get; } = 1.0f;

        public float TotalHealthRestore
        {
            get
            {
                var regen = GetAbilitySpecialData("soul_heal_amount");
                return regen * Duration;
            }
        }

        public float GetTickDamage(params CUnit[] targets)
        {
            var damage = RawTickDamage;
            var amplify = this.SpellAmplification();
            var drain = GetAbilitySpecialData("enemy_hp_drain");
            var reduction = 0.0f;
            if (targets.Any())
            {
                var drainAmount = (targets.First().Health * drain) / 100;
                damage += drainAmount;
                reduction = this.GetDamageReduction(targets.First(), DamageType);
            }

            return DamageHelpers.GetSpellDamage(damage, amplify, reduction);
        }

        public float GetTotalDamage(params CUnit[] targets)
        {
            return GetTickDamage(targets) * (DamageDuration / TickRate);
        }
    }
}