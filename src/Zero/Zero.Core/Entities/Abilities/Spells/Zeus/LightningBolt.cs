using System.Linq;

using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Update;

namespace Divine.Core.Entities.Abilities.Spells.Zeus
{
    [Spell(AbilityId.zuus_lightning_bolt, HeroId.npc_dota_hero_zuus)]
    public sealed class LightningBolt : RangedSpell, IAreaOfEffectAbility
    {
        public LightningBolt(Ability ability)
            : base(ability)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned | UnitState.ProvidesVision;

        public override AbilityBehavior AbilityBehavior
        {
            get
            {
                var behavior = base.AbilityBehavior;
                var talent = Owner.GetAbilityById(AbilityId.special_bonus_unique_zeus_5);

                if (talent?.Level > 0)
                {
                    behavior = (behavior & ~AbilityBehavior.UnitTarget) | AbilityBehavior.Point;
                }

                return behavior;
            }
        }

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("spread_aoe");
            }
        }

        public override float GetDamage(params CUnit[] targets)
        {
            if (!targets.Any())
            {
                return RawDamage;
            }

            return base.GetDamage(targets) + (StaticField?.GetDamage(targets) ?? 0);
        }

        private StaticField StaticField;

        private CUnit owner;

        public override CUnit Owner
        {
            get
            {
                return owner;
            }

            internal set
            {
                owner = value;

                UpdateManager.BeginInvoke(100, () =>
                {
                    StaticField = (StaticField)owner.GetAbilityById(AbilityId.zuus_static_field);
                });
            }
        }
    }
}