namespace BAIO.Modes
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Divine.Entity.Entities.Units;
    using Divine.Extensions;

    using Ensage.SDK.Orbwalker.Modes;

    public abstract class ComboMode : KeyPressOrbwalkingModeAsync
    {
        private readonly BaseHero baseHero;

        protected ComboMode(BaseHero hero, float maxTargetRange = 1000.0f)
            : base(hero.Context, hero.Config.General.ComboKey)
        {
            this.baseHero = hero;
            this.MaxTargetRange = maxTargetRange;
        }

        public override bool CanExecute
        {
            get
            {
                var result = base.CanExecute;
                if (!result)
                {
                    this.CurrentTarget = null;
                }

                return result;
            }
        }

        public Unit CurrentTarget { get; protected set; }

        public float MaxTargetRange { get; protected set; }

        protected async Task<bool> ShouldExecute(CancellationToken token)
        {
            if (!this.baseHero.Owner.IsAlive)
            {
                this.CurrentTarget = null;
                await Task.Delay(125, token);
                return false;
            }

            if (this.baseHero.IsKillstealing)
            {
                await Task.Delay(125, token);
                return false;
            }

            if (!this.baseHero.Config.General.Enabled)
            {
                await Task.Delay(125, token);
                return false;
            }

            if (!this.baseHero.Config.General.LockTarget || (this.CurrentTarget == null) || !this.CurrentTarget.IsValid || !this.CurrentTarget.IsAlive)
            {
                this.CurrentTarget = this.baseHero.Context.TargetSelector.GetTargets().Where(x => x.Distance2D(this.Owner) <= this.MaxTargetRange).OrderByDescending(x => !x.IsIllusion).FirstOrDefault();
            }

            return true;
        }

        public virtual void OrbwalkToTarget()
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid && !this.CurrentTarget.IsEthereal() && !this.CurrentTarget.IsInvulnerable() &&
                (!this.baseHero.Config.General.KiteMode || this.Owner.Distance2D(this.CurrentTarget) <= this.Owner.AttackRange(this.CurrentTarget)))
            {
                this.baseHero.Context.Orbwalker.OrbwalkTo(this.CurrentTarget);
            }
            else
            {
                this.baseHero.Context.Orbwalker.OrbwalkTo(null);
            }
        }
    }
}