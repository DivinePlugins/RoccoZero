using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Prediction;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class LineItem : PredictionItem
    {
        protected LineItem(Item item)
            : base(item)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotLine;

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < Range + Radius;
        }
    }
}