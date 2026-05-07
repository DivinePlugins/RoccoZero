namespace Ensage.SDK.Abilities
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;
    using Divine.Prediction;

    using Ensage.SDK.Extensions;
    using Ensage.SDK.Prediction;

    public abstract class ConeAbility : PredictionAbility
    {
        protected ConeAbility(Ability ability)
            : base(ability)
        {
        }

        public virtual float EndRadius
        {
            get
            {
                return this.Radius;
            }
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotCone;

        public override bool CanHit(params Unit[] targets)
        {
            return targets.All(x => x.Distance2D(this.Owner) < (this.Range + this.EndRadius));
        }
    }
}