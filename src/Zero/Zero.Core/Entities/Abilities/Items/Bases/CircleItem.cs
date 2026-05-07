using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Prediction;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class CircleItem : PredictionItem
    {
        protected CircleItem(Item item)
            : base(item)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotCircle;

        public override float Speed { get; } = float.MaxValue;

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < CastRange + Radius;
        }
    }
}