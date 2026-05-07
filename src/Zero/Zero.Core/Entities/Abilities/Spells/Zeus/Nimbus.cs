using System.Linq;

using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Update;

namespace Divine.Core.Entities.Abilities.Spells.Zeus
{
    [Spell(AbilityId.zuus_cloud, HeroId.npc_dota_hero_zuus)]
    public sealed class Nimbus : RangedSpell, IAreaOfEffectAbility
    {
        public Nimbus(Ability ability)
            : base(ability)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return base.CanBeCasted && !IsHidden;
            }
        }

        public override float CastRange
        {
            get
            {
                return float.MaxValue;
            }
        }

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("cloud_radius");
            }
        }

        public override float GetDamage(params CUnit[] targets)
        {
            if (LightningBolt == null)
            {
                return 0;
            }

            var rawLightningDamage = LightningBolt.GetDamage();
            var reduction = 0.0f;

            if (targets.Any())
            {
                reduction = this.GetDamageReduction(targets.First(), DamageType);
            }

            // no spell amp on cloud
            return DamageHelpers.GetSpellDamage(rawLightningDamage, 0, reduction);
        }

        public override float GetDamage(CUnit target, float damageModifier, float targetHealth = float.MinValue)
        {
            if (LightningBolt == null)
            {
                return 0;
            }

            var rawLightningDamage = LightningBolt.GetDamage();
            var reduction = this.GetDamageReduction(target, DamageType);

            // no spell amp on cloud
            return DamageHelpers.GetSpellDamage(rawLightningDamage, 0, -reduction, damageModifier);
        }

        private LightningBolt LightningBolt;

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
                    LightningBolt = (LightningBolt)owner.GetAbilityById(AbilityId.zuus_lightning_bolt);
                });
            }
        }
    }
}