using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities;
using Divine.Prediction;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class LineSpell : PredictionSpell
    {
        protected LineSpell(Ability ability)
            : base(ability)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotLine;

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < Range + Radius;
        }
    }
}