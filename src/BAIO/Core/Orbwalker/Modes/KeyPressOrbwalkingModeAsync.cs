namespace Ensage.SDK.Orbwalker.Modes
{
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;

    using Ensage.SDK.Service;

    public abstract class KeyPressOrbwalkingModeAsync : OrbwalkingModeAsync
    {
        private bool canExecute;

        protected KeyPressOrbwalkingModeAsync(IServiceContext context, MenuHoldKey key)
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

        protected override void OnActivate()
        {
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