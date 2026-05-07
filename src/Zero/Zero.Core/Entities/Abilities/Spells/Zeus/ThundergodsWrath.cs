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
    [Spell(AbilityId.zuus_thundergods_wrath, HeroId.npc_dota_hero_zuus)]
    public sealed class ThundergodsWrath : ActiveSpell, IAreaOfEffectAbility
    {
        public ThundergodsWrath(Ability ability)
            : base(ability)
        {
        }

        public float Radius { get; } = float.MaxValue;

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialData("damage");
            }
        }

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < CastRange + Radius;
        }

        public override float GetDamage(params CUnit[] targets)
        {
            var damage = RawDamage;
            var amplify = Owner.GetSpellAmplification();

            var totalDamage = 0.0f;
            foreach (var target in targets)
            {
                if (target.IsInvisible())
                {
                    continue;
                }

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