using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class RangedItem : ActiveItem
    {
        protected RangedItem(Item item)
            : base(item)
        {
        }

        public override float CastRange
        {
            get
            {
                return BaseCastRange + Owner.Base.BonusCastRange;
            }
        }
    }
}