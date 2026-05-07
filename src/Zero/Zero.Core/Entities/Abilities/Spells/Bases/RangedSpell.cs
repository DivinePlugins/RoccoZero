using Divine.Entity.Entities.Abilities;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class RangedSpell : ActiveSpell
    {
        protected RangedSpell(Ability ability)
            : base(ability)
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