namespace Ensage.SDK.Orbwalker.Modes
{
    using Divine.Entity.Entities.Units;

    using Ensage.SDK.Service;

    public abstract class OrbwalkingMode : IOrbwalkingMode
    {
        protected OrbwalkingMode(IServiceContext context)
        {
            this.Context = context;
            this.Owner = context.Owner;
            this.Orbwalker = context.Orbwalker;
        }

        public abstract bool CanExecute { get; }

        protected IServiceContext Context { get; }

        protected IOrbwalkerManager Orbwalker { get; }

        protected Unit Owner { get; }

        public abstract void Execute();

        public void Activate()
        {
            OnActivate();
        }

        public void Deactivate()
        {
            OnDeactivate();
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnDeactivate()
        {
        }
    }
}