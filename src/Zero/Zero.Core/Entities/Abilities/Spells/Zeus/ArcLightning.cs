using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Update;

namespace Divine.Core.Entities.Abilities.Spells.Zeus
{
    [Spell(AbilityId.zuus_arc_lightning, HeroId.npc_dota_hero_zuus)]
    public sealed class ArcLightning : RangedSpell, IAreaOfEffectAbility
    {
        public ArcLightning(Ability ability)
            : base(ability)
        {
        }

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialDataWithTalent("arc_damage");
            }
        }

        public override float GetDamage(params CUnit[] targets)
        {
            var damage = RawDamage;
            var amplify = Owner.GetSpellAmplification();

            var totalDamage = 0.0f;
            foreach (var target in targets)
            {
                var reduction = this.GetDamageReduction(target, DamageType);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage + (StaticField?.GetDamage(targets) ?? 0);
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