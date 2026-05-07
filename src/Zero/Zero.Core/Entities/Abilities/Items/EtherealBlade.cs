using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ethereal_blade)]
    public sealed class EtherealBlade : RangedItem, IHasTargetModifier, IHasDamageAmplifier
    {
        public EtherealBlade(Item item)
            : base(item)
        {
        }

        public DamageType AmplifierType { get; } = DamageType.Magical;

        public override UnitState AppliesUnitState { get; } = UnitState.Disarmed;

        public float DamageAmplification
        {
            get
            {
                return GetAbilitySpecialData("ethereal_damage_bonus") / -100.0f;
            }
        }

        public override DamageType DamageType { get; } = DamageType.Magical;

        public override float Speed
        {
            get
            {
                return GetAbilitySpecialData("projectile_speed");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_ethereal_blade_ethereal";

        protected override float RawDamage
        {
            get
            {
                var damage = GetAbilitySpecialData("blast_damage_base");

                var hero = Owner as CHero;
                if (hero != null)
                {
                    var multiplier = GetAbilitySpecialData("blast_agility_multiplier"); // 2.0
                    switch (hero.PrimaryAttribute)
                    {
                        case Attribute.Strength:
                            damage += multiplier * hero.TotalStrength;
                            break;
                        case Attribute.Agility:
                            damage += multiplier * hero.TotalAgility;
                            break;
                        case Attribute.Intelligence:
                            damage += multiplier * hero.TotalIntelligence;
                            break;
                    }
                }

                return damage;
            }
        }

        public override float GetDamage(params CUnit[] targets)
        {
            var damage = RawDamage;

            var amplify = Owner.GetSpellAmplification();
            if (!targets.Any())
            {
                return DamageHelpers.GetSpellDamage(damage, amplify);
            }

            var damageBonus = GetAbilitySpecialData("ethereal_damage_bonus") / -100.0f; // -40 => 0.4
            var resist = this.GetDamageReduction(targets.First(), DamageType);

            return DamageHelpers.GetSpellDamage(damage, amplify, -resist, damageBonus);
        }
    }
}