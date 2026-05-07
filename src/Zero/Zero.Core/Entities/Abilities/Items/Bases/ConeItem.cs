using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Prediction;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class ConeItem : PredictionItem
    {
        protected ConeItem(Item item)
            : base(item)
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