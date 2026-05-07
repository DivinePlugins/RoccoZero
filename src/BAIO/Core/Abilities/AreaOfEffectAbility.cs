namespace Ensage.SDK.Abilities
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;

    using Ensage.SDK.Extensions;

    public abstract class AreaOfEffectAbility : RangedAbility, IAreaOfEffectAbility
    {
        protected AreaOfEffectAbility(Ability ability)
            : base(ability)
        {
        }

        public virtual float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public override bool CanHit(params Unit[] targets)
        {
            return targets.All(x => x.Distance2D(this.Owner) < (this.CastRange + this.Radius));
        }
    }
}