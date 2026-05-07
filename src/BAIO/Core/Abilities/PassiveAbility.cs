namespace Ensage.SDK.Abilities
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units.Components;

    public abstract class PassiveAbility : BaseAbility
    {
        protected PassiveAbility(Ability ability)
            : base(ability)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return this.IsReady && !this.Owner.UnitState.HasFlag(UnitState.PassivesDisabled);
            }
        }
    }
}