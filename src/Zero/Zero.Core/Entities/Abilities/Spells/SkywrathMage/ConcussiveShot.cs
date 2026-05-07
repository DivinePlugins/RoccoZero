using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

namespace Divine.Core.Entities.Abilities.Spells.SkywrathMage
{
    [Spell(AbilityId.skywrath_mage_concussive_shot, HeroId.npc_dota_hero_skywrath_mage)]
    public sealed class ConcussiveShot : ActiveSpell, IAreaOfEffectAbility, IHasTargetModifier
    {
        public ConcussiveShot(Ability ability)
            : base(ability)
        {
        }

        public float Radius
        {
            get
            {
                if (Owner.GetAbilityById(AbilityId.special_bonus_unique_skywrath_4)?.Level > 0)
                {
                    return float.MaxValue;
                }

                return GetAbilitySpecialData("launch_radius");
            }
        }

        public override float Speed
        {
            get
            {
                return GetAbilitySpecialData("speed");
            }
        }

        public CHero TargetHit
        {
            get
            {
                return UnitManager<CHero, Enemy, NoIllusion>.Units.Where(x => x.IsVisible && x.IsAlive && CanHit(x))
                                                                  .OrderBy(x => x.Distance2D(Owner))
                                                                  .FirstOrDefault();
            }
        }

        public string TargetModifierName { get; } = "modifier_skywrath_mage_concussive_shot_slow";

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialData("damage");
            }
        }

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < Radius - Owner.HullRadius;
        }

        public override float GetDamage(params CUnit[] targets)
        {
            var targetHit = TargetHit;
            if (!targets.Any(x => x == targetHit))
            {
                return 0;
            }

            var damage = RawDamage;
            var amplify = Owner.GetSpellAmplification();
            var reduction = this.GetDamageReduction(targetHit, DamageType);

            return DamageHelpers.GetSpellDamage(damage, amplify, reduction);
        }

        public override float GetDamage(CUnit target, float damageModifier, float targetHealth = float.MinValue)
        {
            var targetHit = TargetHit;
            if (targetHit != target)
            {
                return 0;
            }

            var damage = RawDamage;
            var amplify = Owner.GetSpellAmplification();
            var reduction = this.GetDamageReduction(targetHit, DamageType);

            return DamageHelpers.GetSpellDamage(damage, amplify, -reduction, damageModifier);
        }
    }
}