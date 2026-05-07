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
//using Ensage.Common.Enums;
//using Ensage.Common.Extensions;
//using Ensage.Common.Extensions.SharpDX;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.PredictionManager.Collision;
//using Ensage.SDK.Renderer.Particle;
//using log4net;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using SharpDX.DXGI;
//using AbilityExtensions = Ensage.SDK.Extensions.AbilityExtensions;

//namespace BAIO.Heroes.Modes.Combo
//{
//    internal class PuckCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly Puck Puck;
//        private Vector3 OrbPredictionPos;
//        private Unit UltiTarget;
//        private Unit fountain;
//        private PredictionOutput output;
//        private TaskHandler EscapeHandler;
//        private bool castingShift = false;

//        public PuckCombo(Puck hero)
//            : base(hero)
//        {
//            this.Puck = hero;
//            UpdateManager.CreateIngameUpdate(OnUpdate, 25);
//            EscapeHandler = TaskHandler.Run(PuckPrisonBreak);
//        }

//        protected override void OnDeactivate()
//        {
//            UpdateManager.DestroyIngameUpdate(this.OnUpdate);

//            base.OnDeactivate();
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token))
//            {
//                return;
//            }

//            this.MaxTargetRange = Math.Max(this.MaxTargetRange, Puck.Orb.CastRange + 1200 * 1.1f);

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
//            {
//                this.Puck.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            var orbUnit =
//                EntityManager.GetEntities<Unit>().Where(x => x.NetworkName == "CDOTA_BaseNPC" && x.Team == this.Owner.Team)
//                    .ToList();

//            fountain =
//                EntityManager.GetEntities<Unit>()
//                    .FirstOrDefault(
//                        x => x.IsValid && x.Team != this.Owner.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);

//            var jaunt = this.Puck.Jaunt;
//            if (this.Puck.SniperMode.Value)
//            {
//                if (orbUnit != null)
//                {
//                    for (int i = 0; i < orbUnit.Count(); ++i)
//                    {
//                        if (orbUnit[i].Distance2D(this.CurrentTarget) >= 1000 &&
//                            this.Owner.Distance2D(this.CurrentTarget) <= 300 &&
//                            orbUnit[i].DayVision == 450 && orbUnit[i].Distance2D(fountain) > 800)
//                        {
//                            jaunt.Cast();
//                        }
//                    }
//                }
//            }

//            if (orbUnit != null)
//            {
//                for (int i = 0; i < orbUnit.Count(); ++i)
//                {
//                    if (orbUnit[i].Distance2D(this.CurrentTarget) <= 200 &&
//                        this.Owner.Distance2D(this.CurrentTarget) >= 400 &&
//                        orbUnit[i].DayVision == 450 && orbUnit[i].Distance2D(fountain) > 800)
//                    {
//                        jaunt.Cast();
//                    }
//                }
//            }

//            var linkens = this.CurrentTarget.IsLinkensProtected();
//            await BreakLinken(token);

//            try
//            {
//                var blink = this.Puck.BlinkDagger;
//                if (blink != null && blink.CanBeCasted)
//                {
//                    blink.Cast(this.CurrentTarget.Position);
//                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget) + 50, token);
//                }

//                var eBlade = this.Puck.EtherealBlade;
//                if (eBlade != null && eBlade.CanBeCasted && !linkens && eBlade.CanHit(this.CurrentTarget))
//                {
//                    eBlade.Cast(this.CurrentTarget);
//                    await Task.Delay(eBlade.GetHitTime(this.CurrentTarget), token);
//                }

//                var veil = this.Puck.VeilOfDiscord;
//                if (veil != null && veil.CanBeCasted && veil.CanHit(this.CurrentTarget))
//                {
//                    veil.Cast(this.CurrentTarget.Position);
//                    await Task.Delay(veil.GetCastDelay(), token);
//                }

//                var hex = this.Puck.Sheepstick;
//                if (hex != null && hex.CanBeCasted && !linkens && hex.CanHit(this.CurrentTarget))
//                {
//                    hex.Cast(this.CurrentTarget);
//                    await Task.Delay(hex.GetCastDelay(), token);
//                }

//                var dagon = this.Puck.Dagon;
//                if ((eBlade == null || !eBlade.CanBeCasted) && dagon != null && dagon.CanBeCasted && !linkens &&
//                    dagon.CanHit(this.CurrentTarget))
//                {
//                    dagon.Cast(this.CurrentTarget);
//                    await Task.Delay(dagon.GetCastDelay(), token);
//                }

//                var shivas = this.Puck.Shiva;
//                if (shivas != null && shivas.CanBeCasted && shivas.CanHit(this.CurrentTarget))
//                {
//                    shivas.Cast();
//                    await Task.Delay(shivas.GetCastDelay(), token);
//                }

//                var rift = this.Puck.Rift;
//                if (rift.CanBeCasted && rift.CanHit(this.CurrentTarget) &&
//                    this.Owner.Distance2D(this.CurrentTarget) <= rift.CastRange + rift.Radius - 150f)
//                {
//                    var input = rift.GetPredictionInput(this.CurrentTarget);
//                    var predOut = rift.GetPredictionOutput(input);
//                    if (predOut.HitChance >= HitChance.Medium)
//                    {
//                        if (rift.Cast(predOut.CastPosition))
//                        {
//                            await Task.Delay(rift.GetCastDelay(), token);
//                        }
//                    }
//                }

//                var nullifier = this.Puck.Nullifier;
//                if (nullifier != null && nullifier.CanBeCasted && !linkens && nullifier.CanHit(CurrentTarget))
//                {
//                    nullifier.Cast(CurrentTarget);
//                    await Task.Delay(nullifier.GetCastDelay(), token);
//                }

//                var orb = this.Puck.Orb;
//                var atos = this.Puck.RodOfAtos;
//                if (orb.CanBeCasted && orb.CanHit(this.CurrentTarget) && !linkens &&
//                    !this.CurrentTarget.IsInvul())
//                {
//                    if (atos?.CanBeCasted == true && atos.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsStunned() &&
//                        !this.CurrentTarget.IsRooted())
//                    {
//                        var orbPreAtosInput = orb.GetPredictionInput(this.CurrentTarget);
//                        var orbPreAtosOutput = orb.GetPredictionOutput(orbPreAtosInput);

//                        if (orbPreAtosOutput.HitChance != HitChance.OutOfRange)
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(atos.GetHitTime(this.CurrentTarget), token);
//                        }
//                    }

//                    var orbInput = orb.GetPredictionInput(this.CurrentTarget);
//                    var orbOutput = orb.GetPredictionOutput(orbInput);

//                    if (orbOutput.HitChance >= HitChance.Medium)
//                    {
//                        this.OrbPredictionPos = orbOutput.UnitPosition;
//                        orb.Cast(orbOutput.CastPosition);
//                        await Task.Delay(orb.GetCastDelay(), token);
//                    }
//                }

//                var ulti = this.Puck.DreamCoil;
//                if (ulti.CanBeCasted && ulti.CanHit(CurrentTarget) && this.CurrentTarget.IsAlive &&
//                    output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.Team != this.Owner.Team) + 1 >=
//                    (this.Puck.AmountMenu.Value))
//                {
//                    ulti.Cast(output.CastPosition);
//                    await Task.Delay(ulti.GetCastDelay(output.CastPosition), token);
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

//        private void OnUpdate()
//        {
//            if (this.Puck.Context.TargetSelector.IsActive)
//            {
//                UltiTarget = this.CurrentTarget;
//                Context.Particle.Remove("Text");
//            }
//            else
//            {
//                Context.Particle.AddOrUpdate(
//                    Owner,
//                    "Text",
//                    "materials/ensage_ui/particles/text.vpcf",
//                    ParticleAttachment.AbsOrigin,
//                    RestartType.None,
//                    0,
//                    Owner.Position - new Vector3(0, 200, 0),
//                    1,
//                    new Vector3(121, 8611111, 231651),
//                    2,
//                    new Vector3(111, 1111121, 115111),
//                    3,
//                    new Vector3(113, 1151114, 111111),
//                    4,
//                    new Vector3(111, 1111111, 111118),
//                    5,
//                    new Vector3(111, 1111511, 111111),
//                    6,
//                    new Vector3(111, 1111111, 111111),
//                    7,
//                    new Vector3(511, 1111111, 111111),
//                    10,
//                    new Vector3(50, 16, 0),
//                    11,
//                    new Vector3(255, 0, 0));
//            }

//            var Targets =
//                EntityManager.GetEntities<Hero>().OrderBy(x => x == UltiTarget).Where(
//                    x => x.IsValid && x.IsVisible && x.Team != Owner.Team && !x.IsIllusion).ToList();

//            if (UltiTarget != null)
//            {
//                var Input =
//                    new PredictionInput(
//                        Owner,
//                        UltiTarget,
//                        1,
//                        float.MaxValue,
//                        2000,
//                        420,
//                        PredictionSkillshotType.SkillshotCircle,
//                        true,
//                        Targets)
//                    {
//                        CollisionTypes = CollisionTypes.None
//                    };

//                output = this.Puck.Context.PredictionManager.GetPrediction(Input);
//            }

//            if (UltiTarget != null && this.Puck.DreamCoil.CanBeCasted && this.Puck.DrawUltiPosition
//                && UltiTarget.Distance2D(output.CastPosition) <= 1000 &&
//                output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.Team != this.Owner.Team) + 1 >= (this.Puck.AmountMenu.Value == 1 ? 0 : this.Puck.AmountMenu.Value))
//            {
//                Context.Particle.AddOrUpdate(
//                    Owner,
//                    "CoilRadius",
//                    "particles/ui_mouseactions/drag_selected_ring.vpcf",
//                    ParticleAttachment.AbsOrigin,
//                    RestartType.None,
//                    0,
//                    output.CastPosition,
//                    1,
//                    Color.Aqua,
//                    2,
//                    375 * 1.1f);
//            }
//            else
//            {
//                Context.Particle.Remove("CoilRadius");
//            }

//            if (this.Puck.BlinkDagger != null && this.Puck.BlinkDagger.CanBeCasted && this.Puck.ComboRadiusMenu.Value)
//            {
//                Context.Particle.DrawRange(
//                    Owner,
//                    "ComboRadius",
//                    this.Puck.BlinkDagger.CastRange + this.Puck.DreamCoil.CastRange,
//                    Color.Red);
//            }
//            else
//            {
//                Context.Particle.Remove("ComboRadius");
//            }
//        }

//        private async Task PuckPrisonBreak(CancellationToken token = new CancellationToken())
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive)
//            {
//                if (this.Puck.PuckEscape.Value)
//                {
//                    this.Context.Orbwalker.OrbwalkTo(null);
//                    return;
//                }
//                //await Task.Delay(250, token);
//                return;
//            }

//            if (this.Puck.PuckEscape.Value)
//            {
//                fountain =
//                    EntityManager.GetEntities<Unit>()
//                        .FirstOrDefault(
//                            x => x.IsValid && x.Team == this.Owner.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);

//                var orb = this.Puck.Orb;
//                var jaunt = this.Puck.Jaunt;
//                var orbTravelDistance = orb.CastRange;
//                var pos = this.Owner.Position.Extend(fountain.Position, orbTravelDistance);
//                var orbDelayMs = (this.Owner.Distance2D(pos) / orb.Speed) * 1000f;
//                var dagger = this.Puck.BlinkDagger;

//                if (!castingShift && !this.Owner.IsChanneling())
//                {
//                    this.Context.Orbwalker.OrbwalkTo(null);
//                }

//                if (orb.CanBeCasted)
//                {
//                    orb.Cast(pos);

//                    if (this.Puck.PhaseShift.CanBeCasted)
//                    {
//                        castingShift = true;
//                        this.Puck.PhaseShift.Cast();
//                        await Task.Delay((int) orbDelayMs, token);
//                        jaunt.Cast();
//                        castingShift = false;
//                    }
//                    else if (this.Puck.Euls != null && this.Puck.Euls.CanBeCasted)
//                    {
//                        this.Puck.Euls.Cast(this.Owner);
//                        await Task.Delay(this.Puck.Euls.GetCastDelay(), token);
//                        jaunt.Cast();
//                    }
//                    jaunt.Cast();
//                }
//                else
//                {
//                    var enemies =
//                        EntityManager.GetEntities<Hero>().Where(x => x.IsValid && x.Team != this.Owner.Team && !x.IsIllusion);

//                    if (this.Puck.PhaseShift.CanBeCasted && enemies.Any())
//                    {
//                        castingShift = true;
//                        this.Puck.PhaseShift.Cast();
//                        await Task.Delay((int) orbDelayMs, token);
//                        if (dagger != null && dagger.CanBeCasted)
//                        {
//                            var posDagger = this.Owner.Position.Extend(fountain.Position, dagger.CastRange);
//                            dagger.Cast(posDagger);
//                            castingShift = false;
//                            await Task.Delay(dagger.GetCastDelay(), token);
//                        }
//                        castingShift = false;
//                    }
//                    else if (this.Puck.Euls != null && dagger != null && this.Puck.Euls.CanBeCasted &&
//                             dagger.Ability.Cooldown <= 3)
//                    {
//                        this.Puck.Euls.Cast(this.Owner);
//                        await Task.Delay(this.Puck.Euls.GetCastDelay(), token);
//                        if (dagger != null && dagger.CanBeCasted)
//                        {
//                            var posDagger = this.Owner.Position.Extend(fountain.Position, dagger.CastRange);
//                            dagger.Cast(posDagger);
//                            await Task.Delay(dagger.GetCastDelay(), token);
//                        }
//                    }
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
//                        breakerChanger = this.Puck.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.Puck.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.Puck.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.Puck.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.Puck.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.Puck.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.Puck.Nullifier;
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

//                        var atos = this.Puck.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(
//                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.Puck.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.Puck.DiffusalBlade;
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