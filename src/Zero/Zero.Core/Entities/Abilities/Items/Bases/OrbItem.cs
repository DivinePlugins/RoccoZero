using System.Linq;

using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class OrbItem : AutocastItem
    {
        protected OrbItem(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return Owner.CanAttack() && base.CanBeCasted;
            }
        }

        public override float CastPoint
        {
            get
            {
                return Owner.AttackPoint;
            }
        }

        public override float CastRange
        {
            get
            {
                return Owner.AttackRange();
            }
        }

        public override float Speed
        {
            get
            {
                return Owner.ProjectileSpeed();
            }
        }

        public override float GetDamage(params CUnit[] targets)
        {
            if (!targets.Any())
            {
                return Owner.MinimumDamage + Owner.BonusDamage;
            }

            return Owner.GetAttackDamage(targets.First());
        }

        public override float GetDamage(CUnit target, float damageModifier, float targetHealth = float.MinValue)
        {
            return Owner.GetAttackDamage(target, false, damageModifier);
        }
    }
}