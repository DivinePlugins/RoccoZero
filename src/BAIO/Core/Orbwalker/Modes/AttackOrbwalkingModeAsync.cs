namespace Ensage.SDK.Orbwalker.Modes
{
    using Divine.Entity.Entities.Units;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;

    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector.Modes;

    public abstract class AttackOrbwalkingModeAsync : AutoAttackModeAsync
    {
        private bool canExecute;

        protected AttackOrbwalkingModeAsync(IServiceContext context, MenuHoldKey key)
            : base(context)
        {
            this.MenuKey = key;
        }

        public override bool CanExecute
        {
            get
            {
                return this.canExecute;
            }
        }

        public MenuHoldKey MenuKey { get; }

        protected AutoAttackModeSelector Selector { get; set; }

        protected override Unit GetTarget()
        {
            return this.Selector.GetTarget();
        }

        protected override void OnActivate()
        {
            this.Selector = new AutoAttackModeSelector(this.Owner);

            this.MenuKey.ValueChanged += this.MenuKeyOnPropertyChanged;
        }

        protected override void OnDeactivate()
        {
            this.MenuKey.ValueChanged -= this.MenuKeyOnPropertyChanged;
        }

        private void MenuKeyOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            if (e.Value)
            {
                this.canExecute = true;
            }
            else
            {
                this.canExecute = false;
                this.Cancel();
            }
        }
    }
}