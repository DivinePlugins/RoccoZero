using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities;
using Divine.Prediction;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class ConeSpell : PredictionSpell
    {
        protected ConeSpell(Ability ability)
            : base(ability)
        {
        }

        public virtual float EndRadius
        {
            get
            {
                return Radius;
            }
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotCone;

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < Range + EndRadius;
        }
    }
}