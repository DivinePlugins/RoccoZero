using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_meteor_hammer)]
    public sealed class MeteorHammer : RangedItem, IAreaOfEffectAbility, IChannableAbility, IHasDot
    {
        public MeteorHammer(Item item)
            : base(item)
        {
        }

        public override float ActivationDelay
        {
            get
            {
                return GetAbilitySpecialData("land_time");
            }
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public float ChannelDuration
        {
            get
            {
                return GetAbilitySpecialData("max_duration");
            }
        }

        public float DamageDuration
        {
            get
            {
                return GetAbilitySpecialData("burn_duration");
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public bool HasInitialDamage { get; } = true;

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("impact_radius");
            }
        }

        public float RawTickDamage
        {
            get
            {
                // burn_dps_buildings for buildings
                return GetAbilitySpecialData("burn_dps_units");
            }
        }

        public float RemainingDuration
        {
            get
            {
                if (!IsChanneling)
                {
                    return 0;
                }

                return ChannelDuration - ChannelTime;
            }
        }

        public string TargetModifierName { get; } = "modifier_meteor_hammer_burn";

        public float TickRate
        {
            get
            {
                return GetAbilitySpecialData("burn_interval");
            }
        }

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialData("impact_damage_units");
            }
        }

        public float GetTickDamage(params CUnit[] targets)
        {
            var damage = RawTickDamage;
            var amplify = this.SpellAmplification();
            var reduction = 0.0f;
            if (targets.Any())
            {
                reduction = this.GetDamageReduction(targets.First(), DamageType);
            }

            return DamageHelpers.GetSpellDamage(damage, amplify, reduction);
        }

        public float GetTotalDamage(params CUnit[] targets)
        {
            return GetDamage(targets) + (GetTickDamage(targets) * (DamageDuration / TickRate));
        }
    }
}