using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities;
using Divine.Prediction;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class CircleSpell : PredictionSpell
    {
        protected CircleSpell(Ability ability)
            : base(ability)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotCircle;

        public override float Speed { get; } = float.MaxValue;

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < (CastRange + Radius);
        }
    }
}