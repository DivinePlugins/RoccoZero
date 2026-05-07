namespace Ensage.SDK.Orbwalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine.Entity.Entities;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Update;
    using Divine.Zero.Log;

    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Service;

    public sealed class OrbwalkerManager : IOrbwalkerManager
    {
        public OrbwalkerManager(IServiceContext context)
        {
            this.Context = context;
            this.Owner = context.Owner;

            this.Orbwalker = new Orbwalker(Context);
            Orbwalker.Activate();

            this.OnUpdateHandler = UpdateManager.CreateIngameUpdate(100, this.OnUpdate);
        }

        public Orbwalker Orbwalker { get; private set; }

        public IServiceContext Context { get; }

        private List<IOrbwalkingMode> Modes { get; } = new List<IOrbwalkingMode>();

        private UpdateHandler OnUpdateHandler { get; set; }

        private Unit Owner { get; }

        public bool Attack(Unit target)
        {
            return this.Orbwalker.Attack(target);
        }

        public bool CanAttack(Unit target)
        {
            return this.Orbwalker.CanAttack(target);
        }

        public bool CanMove()
        {
            return this.Orbwalker.CanMove();
        }

        public float GetTurnTime(Entity unit)
        {
            return this.Orbwalker.GetTurnTime(unit);
        }

        public bool Move(Vector3 position)
        {
            return this.Orbwalker.Move(position);
        }

        public bool OrbwalkTo(Unit target)
        {
            return this.Orbwalker.OrbwalkTo(target);
        }

        public void RegisterMode(IOrbwalkingMode mode)
        {
            if (this.Modes.Any(e => e == mode))
            {
                return;
            }

            LogManager.Info($"Register Mode {mode}");
            this.Modes.Add(mode);
            mode.Activate();
        }

        public void UnregisterMode(IOrbwalkingMode mode)
        {
            var oldMode = this.Modes.FirstOrDefault(e => e == mode);
            if (oldMode != null)
            {
                mode.Deactivate();
                this.Modes.Remove(oldMode);

                LogManager.Info($"Unregister Mode {mode}");
            }
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.OnUpdateHandler);

            this.Orbwalker.Deactivate();
        }

        private void OnUpdate()
        {
            // no spamerino
            if (GameManager.IsPaused || GameManager.IsChatOpen || !this.Owner.IsValid || !this.Owner.IsAlive || this.Owner.IsStunned())
            {
                return;
            }

            // modes
            foreach (var mode in this.Modes.Where(e => e.CanExecute))
            {
                mode.Execute();
            }
        }
    }
}