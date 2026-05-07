namespace Ensage.SDK.Abilities
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;
    using Divine.Prediction;

    public abstract class LineAbility : PredictionAbility
    {
        protected LineAbility(Ability ability)
            : base(ability)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotLine;

        public override bool CanHit(params Unit[] targets)
        {
            return targets.All(x => x.Distance2D(this.Owner) < (this.Range + this.Radius));
        }
    }
}