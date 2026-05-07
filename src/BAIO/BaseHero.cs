using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Handlers;
using BAIO.Interfaces;
using BAIO.Modes;
using BAIO.UnitManager;

using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Entity.EventArgs;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Particle;
using Divine.Update;
using Divine.Zero.Log;

using Ensage.SDK.Service;

namespace BAIO
{
    public abstract class BaseHero : IHero
    {
        public Config Config;

        internal IServiceContext Context { get; private set; }

        internal Hero Owner { get; private set; }

        private TaskHandler KillstealHandler { get; set; }

        internal bool IsKillstealing;

        private ComboMode ComboMode { get; set; }

        private HarassMode HarassMode { get; set; }

        internal Printer Printer;

        public Updater Updater { get; set; }

        public TaskHandler UnitHandler { get; private set; }

        public TaskHandler BodyBlockHandler { get; private set; }

        public TaskHandler WardsHandler { get; private set; }

        protected abstract ComboMode GetComboMode();

        protected virtual HarassMode GetHarassMode()
        {
            return new HarassMode(this);
        }

        protected virtual void InventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual async Task KillStealAsync(CancellationToken token)
        {
            // ¯\_(ツ)_/¯
        }

        private void OnKillstealPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                this.KillstealHandler.RunAsync();
            }
            else
            {
                this.KillstealHandler.Cancel();
            }
        }

        public async Task AwaitKillstealDelay(int delay, CancellationToken token = default(CancellationToken))
        {
            this.IsKillstealing = true;
            try
            {
                await Task.Delay(delay, token);
            }
            finally
            {
                this.IsKillstealing = false;
            }
        }

        protected virtual void TargetIndicatorUpdater()
        {
            if ((this.ComboMode == null) || !this.Config.General.DrawTargetIndicator)
            {
                return;
            }

            if (this.ComboMode.CanExecute && (this.ComboMode.CurrentTarget != null))
            {
                ParticleManager.CreateTargetLineParticle("TargetIndicator", this.Owner, this.ComboMode.CurrentTarget.Position, Color.Red);
            }
            else
            {
                ParticleManager.DestroyParticle("TargetIndicator");
            }
        }

        private void OnDrawTargetLinePropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (!e.Value)
            {
                ParticleManager.DestroyParticle("TargetIndicator");
            }
        }

        private async Task Bodyblock(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Config.General.ComboKey || this.ComboMode.CurrentTarget == null)
            {
                await Task.Delay(250, token);
                return;
            }

            if (!this.Config.General.Enabled)
            {
                return;
            }

            if (this.Updater.AllUnits.Any() && this.ComboMode.CurrentTarget != null && this.Config.Hero.Enabled &&
                this.Updater.AllUnits.First().Unit.Name != "npc_dota_templar_assassin_psionic_trap")
            {
                BodyBlockerUnit = this.Updater.AllUnits.OrderBy(x => x.Unit.Distance2D(this.ComboMode.CurrentTarget)).First().Unit;
            }
            else
            {
                BodyBlockerUnit = null;
            }

            if (BodyBlockerUnit != null && this.Config.Hero.Enabled)
            {
                var angle = this.ComboMode.CurrentTarget.FindRotationAngle(this.BodyBlockerUnit.Position);

                if (angle > 1.3)
                {
                    var delta = angle * 0.6f;
                    var position = this.ComboMode.CurrentTarget.Position;
                    var side1 = position + this.ComboMode.CurrentTarget.Vector3FromPolarAngle(delta)
                                * Math.Max(this.Config.Hero.BlockSensitivity.Value, 150);
                    var side2 = position + this.ComboMode.CurrentTarget.Vector3FromPolarAngle(-delta)
                                * Math.Max(this.Config.Hero.BlockSensitivity.Value, 150);
                    this.BodyBlockerUnit.Move(side1.Distance(this.BodyBlockerUnit.Position) < side2.Distance(this.BodyBlockerUnit.Position) ? side1 : side2);
                }
                else
                {
                    if (this.ComboMode.CurrentTarget.IsMoving && angle < 0.3 && this.BodyBlockerUnit.IsMoving)
                    {
                        this.BodyBlockerUnit.Stop();
                        return;
                    }

                    if (this.BodyBlockerUnit.Distance2D(this.ComboMode.CurrentTarget.InFront(this.Config.Hero.BlockSensitivity)) >
                        50)
                    {
                        this.BodyBlockerUnit.Move(this.ComboMode.CurrentTarget.InFront(this.Config.Hero.BlockSensitivity));
                    }
                    else
                    {
                        this.BodyBlockerUnit.Attack(this.ComboMode.CurrentTarget);
                        return;
                    }
                }
            }

            await Task.Delay(50, token);
        }

        public Unit BodyBlockerUnit { get; set; }

        private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Config.General.ComboKey)
            {
                await Task.Delay(250, token);
                return;
            }

            if (!this.Config.General.Enabled)
            {
                return;
            }

            foreach (var unit in this.Updater.AllUnits)
            {
                if (unit.Unit != BodyBlockerUnit && this.Config.Hero.ControlUnits && unit.Unit.Name != "npc_dota_templar_assassin_psionic_trap")
                {
                    if (this.ComboMode.CurrentTarget == null || Core.Extensions.UnitExtensions.IsInvul(this.ComboMode.CurrentTarget) ||
                        this.ComboMode.CurrentTarget.IsAttackImmune())
                    {
                        Player.Attack(unit.Unit, (Unit) null);
                        //unit.UnitMovementManager.Orbwalk(null);
                    }
                    else
                    {
                        Player.Attack(unit.Unit, this.ComboMode.CurrentTarget);
                        //unit.UnitMovementManager.Orbwalk(this.ComboMode.CurrentTarget);
                    }
                }

                var ability1 = unit.Ability;
                var ability2 = unit.Ability2;
                var ability3 = unit.Ability3;
                var ability4 = unit.Ability4;

                if (this.ComboMode.CurrentTarget != null && this.Config.Hero.UseUnitAbilities &&
                    !Core.Extensions.UnitExtensions.IsInvul(this.ComboMode.CurrentTarget) && !this.ComboMode.CurrentTarget.IsAttackImmune() && !this.ComboMode.CurrentTarget.IsMagicImmune() &&
                        (ability1 != null && Core.Extensions.AbilityExtensions.CanBeCasted(ability1) ||
                         ability2 != null && Core.Extensions.AbilityExtensions.CanBeCasted(ability2) ||
                         ability3 != null && Core.Extensions.AbilityExtensions.CanBeCasted(ability3) ||
                         ability4 != null && Core.Extensions.AbilityExtensions.CanBeCasted(ability4)))
                {
                    if (ability1 != null
                        && Core.Extensions.AbilityExtensions.CanBeCasted(ability1)
                        && UnitCastingChecks(ability1.Name, unit.Unit, this.ComboMode.CurrentTarget, ability1)
                        && (ability1.TargetTeamType == TargetTeamType.Enemy ||
                            ability1.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability1) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability1) - 70))
                    {
                        if (ability1.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability1.Cast())
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability1.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if (ability1.Cast(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability1.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if (ability1.Cast(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability1 != null
                             && Core.Extensions.AbilityExtensions.CanBeCasted(ability1)
                             && ability1.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability1))
                    {
                        if (ability1.Cast(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }

                    if (ability2 != null
                        && Core.Extensions.AbilityExtensions.CanBeCasted(ability2)
                        && UnitCastingChecks(ability2.Name, unit.Unit, this.ComboMode.CurrentTarget, ability2)
                        && Core.Extensions.AbilityExtensions.CanHit(ability2, this.ComboMode.CurrentTarget)
                        && (ability2.TargetTeamType == TargetTeamType.Enemy ||
                            ability2.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability2) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability2) - 70))
                    {
                        if (ability2.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability2.Cast())
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability2.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if (ability2.Cast(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability2.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if (ability2.Cast(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability2 != null
                             && Core.Extensions.AbilityExtensions.CanBeCasted(ability2)
                             && ability2.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability2))
                    {
                        if (ability2.Cast(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }

                    if (ability3 != null
                        && Core.Extensions.AbilityExtensions.CanBeCasted(ability3)
                        && UnitCastingChecks(ability3.Name, unit.Unit, this.ComboMode.CurrentTarget, ability3)
                        && Core.Extensions.AbilityExtensions.CanHit(ability3, this.ComboMode.CurrentTarget)
                        && (ability3.TargetTeamType == TargetTeamType.Enemy ||
                            ability3.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability3) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability3) - 70))
                    {
                        if (ability3.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability3.Cast())
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability3.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if(ability3.Cast(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability3.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if(ability3.Cast(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability3 != null
                             && Core.Extensions.AbilityExtensions.CanBeCasted(ability3)
                             && ability3.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability3))
                    {
                        if (ability3.Cast(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }

                    if (ability4 != null
                        && Core.Extensions.AbilityExtensions.CanBeCasted(ability4)
                        && UnitCastingChecks(ability4.Name, unit.Unit, this.ComboMode.CurrentTarget, ability4)
                        && Core.Extensions.AbilityExtensions.CanHit(ability4, this.ComboMode.CurrentTarget)
                        && (ability4.TargetTeamType == TargetTeamType.Enemy ||
                            ability4.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability4) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability4) - 70))
                    {
                        if (ability4.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability4.Cast())
                            {
                                await Task.Delay(180, token);

                            }
                        }
                        else if (ability4.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if (ability4.Cast(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability4.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if (ability4.Cast(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability4 != null
                             && Core.Extensions.AbilityExtensions.CanBeCasted(ability4)
                             && ability4.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability4))
                    {
                        if (ability4.Cast(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }
                }
            }

            await Task.Delay(100, token);
        }
        private int WardsRange => this.Owner.HasAghanimsScepter() ? 875 : 650;

        public virtual async Task WardsAttack(CancellationToken tk)
        {
            if (this.ComboMode.CurrentTarget == null)
            {
                return;
            }

            var wardsShouldAttack = EntityManager.GetEntities<Unit>().Where(x =>
                x != null && !GameManager.IsPaused && x.IsValid && x.Distance2D(this.ComboMode.CurrentTarget) <= WardsRange &&
                x.Name.Contains("npc_dota_shadow_shaman_ward") &&
                UnitExtensions.CanAttack(x, this.ComboMode.CurrentTarget)).ToList();

            if (!wardsShouldAttack.Any())
            {
                return;
            }

            if (Player.Attack(wardsShouldAttack, this.ComboMode.CurrentTarget))
            {
                await Task.Delay(200, tk);
            }

            await Task.Delay(100, tk);
            //foreach (var ward in wardsShouldAttack)
            //{
            //    if (ward == null || GameManager.IsPaused || !ward.IsValid ||
            //        !Core.SDK.Extensions.UnitExtensions.CanAttack(ward, this.ComboMode.CurrentTarget) ||
            //        ward.Distance2D(this.ComboMode.CurrentTarget) >= WardsRange) continue;

            //    if (ward.Attack(this.ComboMode.CurrentTarget))
            //    {
            //        await Task.Delay(200, tk);
            //    }
            //}
        }

        public bool UnitCastingChecks(string name, Unit hero, Unit target, Ability ability = null)
        {
            if (name == "lone_druid_savage_roar_bear" && ability != null && !target.HasModifier("modifier_lone_druid_spirit_bear_entangle_effect"))
            {
                return false;
            }

            if (name == "templar_assassin_self_trap" && ability != null &&
                target.HasModifier("modifier_templar_assassin_trap_slow") && target.Distance2D(hero) <= 250)
            {
                return false;
            }

            return true;
        }

        protected virtual void OnActivate()
        {
            try
            {
                this.Context = new ServiceContext();

                this.Context.Inventory.Attach(this);
                this.Owner = EntityManager.LocalHero;

                this.Config = new Config(this.Owner.HeroId);
                this.ComboMode = this.GetComboMode();
                this.Context.Orbwalker.RegisterMode(this.ComboMode);
                this.HarassMode = this.GetHarassMode();
                this.Context.Orbwalker.RegisterMode(this.HarassMode);

                this.KillstealHandler = TaskHandler.Run(this.KillStealAsync, true, this.Config.General.Killsteal);

                UpdateManager.CreateIngameUpdate(this.TargetIndicatorUpdater);

                this.Config.General.DrawTargetIndicator.ValueChanged += this.OnDrawTargetLinePropertyChanged;
                this.Config.General.Killsteal.ValueChanged += this.OnKillstealPropertyChanged;
                this.Context.Inventory.CollectionChanged += this.InventoryChanged;

                this.Updater = new Updater(this);
                this.UnitHandler = TaskHandler.Run(OnUpdate);
                this.BodyBlockHandler = TaskHandler.Run(Bodyblock);

                if (this.Owner.HeroId == HeroId.npc_dota_hero_shadow_shaman)
                {
                    this.WardsHandler = TaskHandler.Run(WardsAttack);
                }

                UpdateManager.BeginInvoke(() =>
                {
                    EntityManager.EntityAdded += OnEntityAdded;
                    EntityManager.EntityRemoved += OnEntityRemoved;
                });
            }
            catch (TaskCanceledException)
            {
                // ignore.
            }
            catch (Exception e)
            {
                LogManager.Error($"Exception in OnActivate: {e}");
            }
        }

        protected virtual void OnDeactivate()
        {
            this.Context.Inventory.Detach(this);

            this.Context.Orbwalker.UnregisterMode(this.HarassMode);
            this.Context.Orbwalker.UnregisterMode(this.ComboMode);

            this.Config.General.DrawTargetIndicator.ValueChanged -= this.OnDrawTargetLinePropertyChanged;
            this.Config.General.Killsteal.ValueChanged -= this.OnKillstealPropertyChanged;
            this.Context.Inventory.CollectionChanged -= this.InventoryChanged;

            UpdateManager.DestroyIngameUpdate(this.TargetIndicatorUpdater);
            this.UnitHandler?.Cancel();
            this.BodyBlockHandler?.Cancel();
            this.KillstealHandler.Cancel();

            this.Config.Dispose();

            this.Context.Dispose();

            EntityManager.EntityAdded -= OnEntityAdded;
            EntityManager.EntityRemoved -= OnEntityRemoved;
        }

        private protected virtual void OnMenuAllyHeroChange(HeroId heroId, bool add)
        {
        }

        private protected virtual void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
        }

        private void OnEntityAdded(EntityAddedEventArgs e)
        {
            if (e.Entity is not Hero hero || hero.IsIllusion)
            {
                return;
            }

            if (hero.IsAlly(Owner))
            {
                OnMenuAllyHeroChange(hero.HeroId, true);
            }
            else
            {
                OnMenuEnemyHeroChange(hero.HeroId, true);
            }
        }

        private void OnEntityRemoved(EntityRemovedEventArgs e)
        {
            if (e.Entity is not Hero hero || hero.IsIllusion)
            {
                return;
            }

            if (hero.IsAlly(Owner))
            {
                OnMenuAllyHeroChange(hero.HeroId, false);
            }
            else
            {
                OnMenuEnemyHeroChange(hero.HeroId, false);
            }
        }

        public void Activate()
        {
            OnActivate();
        }

        public void Deactivate()
        {
            OnDeactivate();
        }
    }
}