//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Base;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Prediction;
//using log4net;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

//namespace BAIO.Heroes.Modes.Combo
//{
//    internal class SlarkCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly Slark Slark;

//        private bool PounceCanHit = false;

//        public TaskHandler UltiHandler;

//        public SlarkCombo(Slark hero)
//            : base(hero)
//        {
//            this.Slark = hero;
//            this.UltiHandler = TaskHandler.Run(this.OnUpdate);
//        }

//        protected override void OnDeactivate()
//        {
//            this.UltiHandler.Cancel();

//            base.OnDeactivate();
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token))
//            {
//                return;
//            }

//            var blinkReady = this.Slark.BlinkDagger != null && this.Slark.BlinkDagger.CanBeCasted;
//            this.MaxTargetRange = blinkReady
//                ? Math.Max(this.MaxTargetRange, Slark.PounceDistance + Slark.PounceRadius + this.Slark.BlinkDagger.CastRange * 1.1f)
//                : Math.Max(this.MaxTargetRange, Slark.PounceDistance + Slark.PounceRadius * 1.1f);

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
//            {
//                this.Slark.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            var isInvis = this.Owner.IsInvisible();
//            if (isInvis && this.CurrentTarget != null && this.Owner.CanAttack() &&
//                this.CurrentTarget.Distance2D(this.Owner) <= this.Owner.AttackRange)
//            {
//                this.Owner.Attack(this.CurrentTarget);
//                await Task.Delay((int) (this.Owner.AttackBackswing() + GameManager.Ping), token);
//            }

//            var linkens = this.CurrentTarget.IsLinkensProtected();
//            await BreakLinken(token);

//            try
//            {
//                var blink = this.Slark.BlinkDagger;
//                var sBlade = this.Slark.ShadowBlade;
//                var sEdge = this.Slark.SilverEdge;
//                if (blinkReady && !isInvis && this.CurrentTarget.Distance2D(this.Owner) <= blink.CastRange)
//                {
//                    blink.Cast(CurrentTarget.Position);
//                    await Task.Delay(blink.GetCastDelay(), token);
//                }
//                else if (!isInvis && sBlade?.CanBeCasted == true || sEdge?.CanBeCasted == true
//                         && this.Owner.Distance2D(this.CurrentTarget) < 2500 && !PounceCanHit &&
//                         this.Slark.InvisHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    sBlade?.Cast();
//                    sEdge?.Cast();
//                }

//                var pounce = this.Slark.Pounce;
//                if (!isInvis && pounce.CanBeCasted() && this.Owner.Distance2D(this.CurrentTarget) <= Slark.PounceDistance + Slark.PounceRadius)
//                {
//                    var vectorOfMovement =
//                        new Vector2(
//                            (float) Math.Cos(this.CurrentTarget.RotationRad) * this.CurrentTarget.MovementSpeed,
//                            (float) Math.Sin(this.CurrentTarget.RotationRad) * this.CurrentTarget.MovementSpeed);
//                    var hitPosition = this.Slark.Intercepts(this.CurrentTarget.Position, vectorOfMovement,
//                        this.Owner.Position, this.Slark.PounceSpeed);
//                    var hitPosMod = hitPosition +
//                                    new Vector3(vectorOfMovement.X * (this.Slark.TimeToTurn(this.Owner, hitPosition)),
//                                        vectorOfMovement.Y * (this.Slark.TimeToTurn(this.Owner, hitPosition)), 0);
//                    var hitPosMod2 = hitPosition +
//                                     new Vector3(vectorOfMovement.X * (this.Slark.TimeToTurn(this.Owner, hitPosMod)),
//                                         vectorOfMovement.Y * (this.Slark.TimeToTurn(this.Owner, hitPosMod)), 0);

//                    if (this.Owner.Distance2D(hitPosMod2) > ((Slark.PounceDistance + Slark.PounceRadius) + this.CurrentTarget.HullRadius))
//                    {
//                        return;
//                    }

//                    if (this.Slark.IsFacing(this.Owner, this.CurrentTarget))
//                    {
//                        PounceCanHit = true;
//                        pounce.Cast();
//                        await Task.Delay((int) pounce.GetHitDelay(this.CurrentTarget), token);
//                        PounceCanHit = false;
//                    }
//                }

//                var darkPact = this.Slark.DarkPact;
//                if (!isInvis && darkPact.CanBeCasted() && this.Owner.Distance2D(this.CurrentTarget) <= 375 &&
//                    darkPact.CanHit(CurrentTarget))
//                {
//                    darkPact.Cast();
//                    // doesn't need delay.
//                }

//                var abyssal = this.Slark.AbyssalBlade;
//                if (!isInvis && abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
//                    this.Slark.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    abyssal.Cast(CurrentTarget);
//                    await Task.Delay(abyssal.GetCastDelay(), token);
//                }

//                var bloodthorn = this.Slark.BloodThorn;
//                if (!isInvis && bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) &&
//                    !linkens &&
//                    this.Slark.BloodthornOrchidHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    bloodthorn.Cast(this.CurrentTarget);
//                    await Task.Delay(bloodthorn.GetCastDelay(), token);
//                }

//                var orchid = this.Slark.Orchid;
//                if (!isInvis && orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
//                    this.Slark.BloodthornOrchidHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    orchid.Cast(this.CurrentTarget);
//                    await Task.Delay(orchid.GetCastDelay(), token);
//                }

//                var nullifier = this.Slark.Nullifier;
//                if (!isInvis && nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) &&
//                    !linkens &&
//                    this.Slark.NullifierHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    nullifier.Cast(CurrentTarget);
//                    await Task.Delay(nullifier.GetCastDelay(), token);
//                }

//                var diff = this.Slark.DiffusalBlade;
//                if (!isInvis && diff != null && diff.CanBeCasted && diff.CanHit(CurrentTarget) &&
//                    !linkens)
//                {
//                    diff.Cast(CurrentTarget);
//                    await Task.Delay(diff.GetCastDelay(), token);
//                }
//            }
//            catch (TaskCanceledException)
//            {
//                // ignore
//            }
//            catch (Exception e)
//            {
//                LogManager.Debug($"{e}");
//            }

//            this.OrbwalkToTarget();
//        }

//        private async Task OnUpdate(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Slark.MinimumUltiToggle || this.Slark.MinimumUltiHp.Value == 0)
//            {
//                await Task.Delay(250, token);
//                return;
//            }

//            var enemies =
//                EntityManager.GetEntities<Hero>().Where(
//                    x =>
//                        x.IsValid && x.IsAlive && !x.IsIllusion && x.Team != this.Owner.Team &&
//                        x.Distance2D(this.Owner) <= 500);

//            if (enemies.Any() &&
//                (((float) this.Owner.Health / this.Owner.MaximumHealth) * 100f) <= this.Slark.MinimumUltiHp.Value.Value &&
//                this.Slark.ShadowDance.CanBeCasted())
//            {
//                this.Slark.ShadowDance.Cast();
//                await Task.Delay((int) GameManager.Ping + 50, token);
//            }
//            await Task.Delay(100, token);
//        }

//        protected async Task BreakLinken(CancellationToken token)
//        {
//            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
//            {
//                try
//                {
//                    List<KeyValuePair<AbilityId, bool>> breakerChanger = new List<KeyValuePair<AbilityId, bool>>();

//                    if (this.CurrentTarget.IsLinkensProtected())
//                    {
//                        breakerChanger = this.Slark.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.Slark.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.Slark.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.Slark.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.Slark.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.Slark.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.Slark.Nullifier;
//                        if (nullifier != null
//                            && nullifier.Item.Id == order.Key
//                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
//                        {
//                            nullifier.Cast(this.CurrentTarget);
//                            await Task.Delay(
//                                nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget),
//                                token);
//                            return;
//                        }

//                        var atos = this.Slark.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(
//                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.Slark.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.Slark.DiffusalBlade;
//                        if (diff != null
//                            && diff.Item.Id == order.Key
//                            && diff.CanBeCasted && diff.CanHit(this.CurrentTarget))
//                        {
//                            diff.Cast(this.CurrentTarget);
//                            await Task.Delay(diff.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }
//                    }
//                }
//                catch (TaskCanceledException)
//                {
//                    // ignore
//                }
//                catch (Exception e)
//                {
//                    LogManager.Error("Linken break error: " + e);
//                }
//            }
//        }
//    }
//}