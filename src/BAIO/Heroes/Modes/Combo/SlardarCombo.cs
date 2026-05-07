//using System;
//using System.Collections.Generic;
//using Ensage.Common.Extensions;
//using Ensage.Common.Threading;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Prediction;

//namespace BAIO.Heroes.Modes.Combo
//{
//    using System.Linq;
//    using System.Reflection;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using Base;
//    using BAIO.Modes;
//    using Ensage;
//    using Ensage.Common.Menu;
//    using Ensage.SDK.Extensions;
//    using Ensage.SDK.Helpers;
//    using log4net;
//    using PlaySharp.Toolkit.Logging;

//    internal class SlardarCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly Slardar Slardar;

//        private TaskHandler StunHandler;

//        private List<Unit> targets;

//        public SlardarCombo(Slardar hero)
//            : base(hero)
//        {
//            this.Slardar = hero;
//            this.StunHandler = TaskHandler.Run(this.AntiFail);
//        }

//        protected override void OnDeactivate()
//        {
//            this.StunHandler.Cancel();

//            base.OnDeactivate();
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token))
//            {
//                return;
//            }

//            var blink = this.Slardar.BlinkDagger;
//            var forceStaff = this.Slardar.ForceStaff;
//            if (blink != null && forceStaff != null)
//            {
//                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange + 600 * 1.3f);
//            }
//            else if (blink != null)
//            {
//                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.3f);
//            }

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible || this.CurrentTarget.IsInvulnerable())
//            {
//                this.Slardar.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            var ulti = this.Slardar.Ulti;
//            var stun = this.Slardar.Stun;
//            var speed = this.Slardar.Speed;

//            var forceStaffReady = (forceStaff != null) && forceStaff.CanBeCasted;
//            var blinkReady = blink != null && blink.CanBeCasted;
//            var distance = this.Owner.Distance2D(this.CurrentTarget);
//            var distance2HitPos = this.Owner.Distance2D(this.CurrentTarget.BasePredict(450 + GameManager.Ping));
//            var pos = this.CurrentTarget.BasePredict(450 + GameManager.Ping);
//            var isInvis = this.Slardar.Owner.IsInvisible();

//            var sBlade = this.Slardar.ShadowBlade;
//            var sEdge = this.Slardar.SilverEdge;
//            if (!isInvis && sBlade?.CanBeCasted == true || sEdge?.CanBeCasted == true
//                     && this.Owner.Distance2D(this.CurrentTarget) < 2000 &&
//                this.Slardar.InvisHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                sBlade?.Cast();
//                sEdge?.Cast();
//            }


//            if (isInvis && this.CurrentTarget != null && this.Owner.CanAttack() &&
//                this.CurrentTarget.Distance2D(this.Owner) <= this.Owner.AttackRange)
//            {
//                this.Owner.Attack(this.CurrentTarget);
//                await Task.Delay((int)(this.Owner.AttackBackswing() + GameManager.Ping), token);
//            }

//            var linkens = this.CurrentTarget.IsLinkensProtected();
//            await BreakLinken(token);

//            if (this.Slardar.ForceStaffPush && forceStaffReady && !isInvis && blinkReady)
//            {
//                if (distance2HitPos < 1800 && distance2HitPos > 1200 && stun.CanBeCasted)
//                {
//                    this.Owner.MoveToDirection(pos);
//                    await Task.Delay((int) (0.2 * 1000 + GameManager.Ping + 200), token);

//                    if (Owner.InFront(600).Distance(pos) < 1200)
//                    {
//                        forceStaff.Cast(Owner);
//                        await Task.Delay(10, token);
//                    }
//                }
//            }

//            if (blinkReady && !isInvis && this.Owner.Distance2D(this.CurrentTarget) > 300 && distance2HitPos < 1200)
//            {
//                if (this.Slardar.UseUltiBefore)
//                {
//                    if (ulti.CanBeCasted && !linkens && !this.CurrentTarget.HasModifier(ulti.TargetModifierName))
//                    {
//                        ulti.Cast(this.CurrentTarget);
//                        await Task.Delay(ulti.GetCastDelay(), token);
//                    }
//                    if (speed.CanBeCasted)
//                    {
//                        speed.Cast();
//                    }
//                }
//                targets = this.Slardar.Context.TargetSelector.GetTargets().Where(x => x.IsValid && !x.IsMagicImmune() &&
//                x.Distance2D(this.CurrentTarget) <= 600 && x.Distance2D(this.Owner) <= 1200).ToList();

//                var input = new PredictionInput(this.Owner, this.CurrentTarget, stun.GetCastDelay() / 1000f, float.MaxValue, 1200, 350, PredictionSkillshotType.SkillshotCircle, true, targets, true);
//                var output = this.Slardar.Context.PredictionManager.GetPrediction(input);
//                if (output.HitChance >= HitChance.Medium)
//                {
//                    blink.Cast(output.CastPosition);
//                }

//                if (forceStaffReady)
//                {
//                    if (distance2HitPos < 750 && !stun.CanHit(this.CurrentTarget) && stun.CanBeCasted)
//                    {
//                        this.Owner.MoveToDirection(pos);
//                        await Task.Delay((int)(0.2 * 1000 + GameManager.Ping + 200), token);

//                        if (Owner.InFront(600).Distance(pos) < 300)
//                        {
//                            forceStaff.Cast(Owner);
//                            await Task.Delay(10, token);
//                        }
//                    }

//                }

//                if (stun.CanBeCasted && stun.CanHit(this.CurrentTarget) && this.Owner.Distance2D(this.CurrentTarget) <= 350)
//                {
//                    stun.Cast();
//                    await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
//                }
//                else
//                {
//                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
//                }
//            }

//            if (!isInvis && this.Slardar.ForceStaffPush && forceStaffReady)
//            {
//                if (distance2HitPos < 800 && distance2HitPos > 600 && stun.CanBeCasted)
//                {
//                    this.Owner.MoveToDirection(pos);
//                    await Task.Delay((int)(400 + GameManager.Ping), token);

//                    if (Owner.InFront(600).Distance(pos) < 200)
//                    {
//                        forceStaff.Cast(Owner);
//                        await Task.Delay(10, token);
//                    }
//                }
//            }

//            if (!isInvis && stun.CanBeCasted && stun.CanHit(this.CurrentTarget) && this.Owner.HasModifier("modifier_slardar_sprint") && this.Owner.Distance2D(this.CurrentTarget) <= 150)
//            {
//                stun.Cast();
//                await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
//            }
//            else if (!isInvis && stun.CanBeCasted && stun.CanHit(this.CurrentTarget) && this.Owner.Distance2D(this.CurrentTarget) <= 100)
//            {
//                stun.Cast();
//                await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
//            }

//            if (distance <= 1000 && !stun.CanBeCasted && !isInvis)
//            {
//                if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) && !linkens && !this.CurrentTarget.HasModifier(ulti.TargetModifierName))
//                {
//                    ulti.Cast(this.CurrentTarget);
//                    await Task.Delay(ulti.GetCastDelay(), token);
//                }
//                if (speed.CanBeCasted)
//                {
//                    speed.Cast();
//                }
//            }

//            var mom = this.Slardar.Mom;
//            var cantCast = !ulti.CanBeCasted && !stun.CanBeCasted && !speed.CanBeCasted;

//            if (!isInvis && mom != null && mom.CanBeCasted && cantCast)
//            {
//                mom.Cast();
//                await Task.Delay(mom.GetCastDelay(), token);
//            }

//            var halberd = this.Slardar.HeavensHalberd;
//            if (!isInvis && halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
//                this.Slardar.HalberdHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                halberd.Cast(CurrentTarget);
//                await Task.Delay(halberd.GetCastDelay(), token);
//            }

//            var bloodthorn = this.Slardar.BloodThorn;
//            if (!isInvis && bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
//                this.Slardar.BtOrchidHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                bloodthorn.Cast(CurrentTarget);
//                await Task.Delay(bloodthorn.GetCastDelay(), token);
//            }

//            var orchid = this.Slardar.Orchid;
//            if (!isInvis && orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
//                this.Slardar.BtOrchidHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                orchid.Cast(CurrentTarget);
//                await Task.Delay(orchid.GetCastDelay(), token);
//            }

//            var bkb = this.Slardar.BlackKingBar;
//            if (!isInvis && bkb != null && bkb.CanBeCasted && this.Slardar.BkbHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                bkb.Cast();
//                await Task.Delay(bkb.GetCastDelay(), token);
//            }

//            var mjollnir = this.Slardar.Mjollnir;
//            if (!isInvis && mjollnir != null && mjollnir.CanBeCasted)
//            {
//                mjollnir.Cast(this.Owner);
//                await Task.Delay(mjollnir.GetCastDelay(), token);
//            }

//            this.OrbwalkToTarget();
//        }

//        private async Task AntiFail(CancellationToken token = new CancellationToken())
//        {
//            if (this.Slardar.Stun != null && this.Slardar.Stun.Ability.IsInAbilityPhase && !this.Slardar.UserUseStun)
//            {
//                var enemies = EntityManager.GetEntities<Hero>()
//                    .Count(x => Owner.Team != x.Team && x.IsValid && !x.IsIllusion && !x.IsInvulnerable() && x.IsAlive && Owner.Distance2D(x) < 300);

//                if (enemies == 0)
//                {
//                    Owner.Stop();
//                    await Task.Delay(10, token);
//                }
//            }
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
//                        breakerChanger = this.Slardar.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.Slardar.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.Slardar.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.Slardar.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.Slardar.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.Slardar.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.Slardar.Nullifier;
//                        if (nullifier != null
//                            && nullifier.Item.Id == order.Key
//                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
//                        {
//                            nullifier.Cast(this.CurrentTarget);
//                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var atos = this.Slardar.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.Slardar.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.Slardar.DiffusalBlade;
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