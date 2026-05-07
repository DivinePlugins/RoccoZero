using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class PassiveSpell : CAbility
    {
        protected PassiveSpell(Ability ability)
            : base(ability)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return IsReady && !Owner.UnitState.HasFlag(UnitState.PassivesDisabled);
            }
        }
    }
}