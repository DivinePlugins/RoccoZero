//using System;
//using System.Collections.Generic;
//using Ensage.Common.Extensions;
//using Ensage.Common.Threading;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.Renderer.Particle;
//using SharpDX;

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

//    internal class MagnusCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly Magnus Magnus;

//        private TaskHandler StunHandler;
//        private TaskHandler SkewerHandler;

//        private List<Hero> targets;
//        private PredictionInput Input;
//        private PredictionOutput Output;
//        private Hero teammateys;
//        private Unit fountain;
//        private Hero skewerOnlyteammateys;
//        private Vector3 skewerPosition;
//        private Vector3 position;
//        private Vector3 skewerOnlyPosition;
//        private Vector3 skewerOnlySkewerPosition;

//        public MagnusCombo(Magnus hero)
//            : base(hero)
//        {
//            this.Magnus = hero;
//            this.StunHandler = TaskHandler.Run(this.AntiFail);
//            this.SkewerHandler = TaskHandler.Run(this.SkewerStandalone);
//            UpdateManager.CreateIngameUpdate(OnUpdate, 25);
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token))
//            {
//                return;
//            }

//            var blink = this.Magnus.BlinkDagger;
//            var forceStaff = this.Magnus.ForceStaff;

//            if (blink != null && forceStaff != null)
//            {
//                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange + 600 * 1.3f);
//            }
//            else if (blink != null)
//            {
//                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.3f);
//            }

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
//            {
//                this.Magnus.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            var ulti = this.Magnus.ReversePolarity;
//            var shock = this.Magnus.Shockwave;
//            var skewer = this.Magnus.Skewer;
//            var empower = this.Magnus.Empower;

//            var forceStaffReady = (forceStaff != null) && forceStaff.CanBeCasted;
//            var blinkReady = blink != null && blink.CanBeCasted;
//            var distance = this.Owner.Distance2D(this.CurrentTarget);
//            var distance2HitPos = this.Owner.Distance2D(this.CurrentTarget.BasePredict(450 + GameManager.Ping));
//            var pos = this.CurrentTarget.BasePredict(450 + GameManager.Ping);
//            var isInvis = this.Magnus.Owner.IsInvisible();
//            var skewerTarget = this.Magnus.SkewerTarget;

//            var sBlade = this.Magnus.ShadowBlade;
//            var sEdge = this.Magnus.SilverEdge;
//            if (this.CurrentTarget != null && !isInvis && sBlade?.CanBeCasted == true || sEdge?.CanBeCasted == true
//                && this.Owner.Distance2D(this.CurrentTarget) < 2000 &&
//                this.Magnus.InvisHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                sBlade?.Cast();
//                sEdge?.Cast();
//            }


//            if (isInvis && this.CurrentTarget != null && this.Owner.CanAttack() &&
//                this.CurrentTarget.Distance2D(this.Owner) <= this.Owner.AttackRange)
//            {
//                this.Owner.Attack(this.CurrentTarget);
//                await Task.Delay((int) (this.Owner.AttackBackswing() + GameManager.Ping), token);
//            }

//            var linkens = this.CurrentTarget.IsLinkensProtected();
//            await BreakLinken(token);

//            if (forceStaffReady && !isInvis && blinkReady)
//            {
//                if (distance2HitPos < 1800 && distance2HitPos > 1200 && ulti.CanBeCasted)
//                {
//                    this.Owner.MoveToDirection(pos);
//                    await Task.Delay((int) (0.1 * 1000 + GameManager.Ping + 200), token);

//                    if (Owner.InFront(600).Distance(pos) < 1200)
//                    {
//                        forceStaff.Cast(Owner);
//                        await Task.Delay(10, token);
//                    }
//                }

//                if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) &&
//                    this.Owner.Distance2D(this.CurrentTarget) <= 350
//                    && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= this.Magnus.Amount.Value)
//                {
//                    this.Owner.MoveToDirection(skewerPosition);
//                    await Task.Delay((int) (0.2 * 1000 + GameManager.Ping + 200), token);
//                    ulti.Cast();
//                    await Task.Delay(ulti.GetCastDelay(this.CurrentTarget) + 100, token);
//                    if (skewer.CanBeCasted && !ulti.CanBeCasted && this.CurrentTarget.HasModifier("modifier_stunned"))
//                    {
//                        skewer.Cast(skewerPosition);
//                        await Task.Delay(skewer.GetCastDelay(skewerPosition), token);
//                    }
//                }
//                else
//                {
//                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
//                }
//            }

//            if (blinkReady && !isInvis && this.Owner.Distance2D(this.CurrentTarget) > 300 && distance2HitPos < 1200
//                && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= this.Magnus.Amount.Value)
//            {
//                if (Output.HitChance >= HitChance.Medium)
//                {
//                    blink.Cast(Output.CastPosition);
//                }

//                if (forceStaffReady)
//                {
//                    if (distance2HitPos < 750 && !ulti.CanHit(this.CurrentTarget) && ulti.CanBeCasted)
//                    {
//                        this.Owner.MoveToDirection(pos);
//                        await Task.Delay((int) (0.2 * 1000 + GameManager.Ping + 200), token);

//                        if (Owner.InFront(600).Distance(pos) < 300)
//                        {
//                            forceStaff.Cast(Owner);
//                            await Task.Delay(10, token);
//                        }
//                    }
//                }

//                if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) &&
//                    this.Owner.Distance2D(this.CurrentTarget) <= 350
//                    && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= this.Magnus.Amount.Value)
//                {
//                    this.Owner.MoveToDirection(skewerPosition);
//                    await Task.Delay((int) (0.1 * 1000 + GameManager.Ping + 100), token);
//                    ulti.Cast();
//                    await Task.Delay(ulti.GetCastDelay(this.CurrentTarget) + 100, token);
//                    if (skewer.CanBeCasted && !ulti.CanBeCasted && this.CurrentTarget.HasModifier("modifier_stunned"))
//                    {
//                        skewer.Cast(skewerPosition);
//                        await Task.Delay(skewer.GetCastDelay(skewerPosition), token);
//                    }
//                }
//                else
//                {
//                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
//                }
//            }

//            if (!isInvis && forceStaffReady)
//            {
//                if (distance2HitPos < 800 && distance2HitPos > 600 && ulti.CanBeCasted)
//                {
//                    this.Owner.MoveToDirection(pos);
//                    await Task.Delay((int) (400 + GameManager.Ping), token);

//                    if (Owner.InFront(600).Distance(pos) < 200)
//                    {
//                        forceStaff.Cast(Owner);
//                        await Task.Delay(10, token);
//                    }
//                }

//                if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) &&
//                    this.Owner.Distance2D(this.CurrentTarget) <= 350
//                    && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= this.Magnus.Amount.Value)
//                {
//                    this.Owner.MoveToDirection(skewerPosition);
//                    await Task.Delay((int) (0.1 * 1000 + GameManager.Ping + 100), token);
//                    ulti.Cast();
//                    await Task.Delay(ulti.GetCastDelay(this.CurrentTarget) + 100, token);
//                    if (skewer.CanBeCasted && !ulti.CanBeCasted && this.CurrentTarget.HasModifier("modifier_stunned"))
//                    {
//                        skewer.Cast(skewerPosition);
//                        await Task.Delay(skewer.GetCastDelay(skewerPosition), token);
//                    }
//                }
//                else
//                {
//                    await Task.Delay(200, token);
//                }
//            }

//            if (!isInvis && !forceStaffReady && !blinkReady && skewer.CanBeCasted
//                && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= this.Magnus.Amount.Value)
//            {
//                var skewerInput = new PredictionInput(this.Owner,
//                    this.CurrentTarget,
//                    skewer.GetCastDelay(this.CurrentTarget.Position) / 1000f,
//                    skewer.Speed,
//                    skewer.CastRange,
//                    skewer.Radius);
//                var skewerOutput = this.Magnus.Context.PredictionManager.GetPrediction(skewerInput);

//                if (skewer.CanBeCasted && ulti.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) >= 600)
//                {
//                    skewer.Cast(skewerOutput.CastPosition);
//                    await Task.Delay(skewer.GetCastDelay(skewerOutput.CastPosition), token);
//                }

//                if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) &&
//                    this.Owner.Distance2D(this.CurrentTarget) <= 400 && !skewer.CanBeCasted)
//                {
//                    ulti.Cast();
//                    await Task.Delay(ulti.GetCastDelay(this.CurrentTarget) + 100, token);
//                }
//            }

//            if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) && this.Owner.Distance2D(this.CurrentTarget) <= 350
//                && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= this.Magnus.Amount.Value)
//            {
//                this.Owner.MoveToDirection(skewerPosition);
//                await Task.Delay((int) (0.1 * 1000 + GameManager.Ping + 100), token);
//                ulti.Cast();
//                await Task.Delay(ulti.GetCastDelay(this.CurrentTarget) + 100, token);
//                if (skewer.CanBeCasted && !ulti.CanBeCasted && this.CurrentTarget.HasModifier("modifier_stunned"))
//                {
//                    skewer.Cast(skewerPosition);
//                    await Task.Delay(skewer.GetCastDelay(skewerPosition), token);
//                }
//            }
//            else
//            {
//                await Task.Delay(50, token);
//            }

//            if (shock.CanBeCasted && shock.CanHit(this.CurrentTarget) &&
//                this.Owner.Distance2D(this.CurrentTarget) <= shock.CastRange)
//            {
//                shock.Cast(this.CurrentTarget);
//                await Task.Delay(shock.GetCastDelay(this.CurrentTarget) + (int) GameManager.Ping + 50, token);
//            }

//            var bloodthorn = this.Magnus.BloodThorn;
//            if (!isInvis && bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
//                this.Magnus.OrchidBloodthornHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                bloodthorn.Cast(CurrentTarget);
//                await Task.Delay(bloodthorn.GetCastDelay(), token);
//            }

//            var orchid = this.Magnus.Orchid;
//            if (!isInvis && orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
//                this.Magnus.OrchidBloodthornHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                orchid.Cast(CurrentTarget);
//                await Task.Delay(orchid.GetCastDelay(), token);
//            }

//            this.OrbwalkToTarget();
//        }

//        private void OnUpdate()
//        {
//            var teammates = EntityManager.GetEntities<Hero>().Where(
//                    x =>
//                        x.IsValid && x.Team == this.Owner.Team &&
//                        x.Distance2D(this.Owner) <= this.Magnus.Empower.CastRange
//                        && this.Magnus.EmpowerHeroes.Value.IsEnabled(x.Name) &&
//                        !x.HasModifier("modifier_magnataur_empower"))
//                .OrderBy(x => x.Distance2D(this.Owner)).FirstOrDefault();

//            var condition = ((!this.Magnus.ReversePolarity.CanBeCasted && !this.Magnus.Skewer.CanBeCasted) ||
//                             !this.Magnus.Config.General.ComboKey || !this.Owner.IsChanneling());

//            if (teammates != null && this.Owner.IsAlive && this.Magnus.Empower.CanBeCasted && condition && !this.Magnus.Empower.Ability.IsInAbilityPhase)
//            {
//                this.Magnus.Empower.Cast(teammates);
//                Task.Delay(this.Magnus.Empower.GetCastDelay(teammates) + 100);
//            }

//            if (this.Magnus.DebugDrawings)
//            {
//                if (this.Magnus.Context.TargetSelector.IsActive)
//                {
//                    Context.Particle.Remove("Metin");
//                }
//                else
//                {
//                    Magnus.Context.Particle.AddOrUpdate(
//                        Owner,
//                        "Metin",
//                        "materials/ensage_ui/particles/text.vpcf",
//                        ParticleAttachment.AbsOrigin,
//                        RestartType.None,
//                        0,
//                        Owner.Position - new Vector3(0, 200, 0),
//                        1,
//                        new Vector3(121, 8611111, 231651),
//                        2,
//                        new Vector3(111, 1111121, 115111),
//                        3,
//                        new Vector3(113, 1151114, 111111),
//                        4,
//                        new Vector3(111, 1111111, 111118),
//                        5,
//                        new Vector3(111, 1111511, 111111),
//                        6,
//                        new Vector3(111, 1111111, 111111),
//                        7,
//                        new Vector3(511, 1111111, 111111),
//                        10,
//                        new Vector3(50, 16, 0),
//                        11,
//                        new Vector3(255, 0, 0));
//                }
//            }


//            targets = EntityManager.GetEntities<Hero>().OrderBy(x => x == this.CurrentTarget).Where(
//                x => x.IsValid && x.IsVisible && x.Team != Owner.Team && !x.IsIllusion).ToList();

//            if (CurrentTarget != null)
//            {
//                Input = new PredictionInput(this.Owner, this.CurrentTarget,
//                    this.Magnus.ReversePolarity.GetCastDelay() / 1000f, float.MaxValue, 1200, 410,
//                    PredictionSkillshotType.SkillshotCircle, true, targets, true);
//                Output = this.Magnus.Context.PredictionManager.GetPrediction(Input);
//            }

//            if (CurrentTarget != null && Output != null && CurrentTarget.Distance2D(Output.CastPosition) <= 1000 &&
//                Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1 >= (this.Magnus.Amount.Value == 1 ? 0 : this.Magnus.Amount.Value) &&
//                this.Magnus.DebugDrawings)
//            {
//                this.Magnus.Context.Particle.AddOrUpdate(
//                    Owner,
//                    "RpRadius",
//                    "particles/ui_mouseactions/drag_selected_ring.vpcf",
//                    ParticleAttachment.AbsOrigin,
//                    RestartType.None,
//                    0,
//                    Output.CastPosition,
//                    1,
//                    Color.Aqua,
//                    2,
//                    410 * 1.1f);
//            }
//            else
//            {
//                this.Magnus.Context.Particle.Remove("RpRadius");
//            }

//            if (CurrentTarget != null && this.Magnus.DebugDrawings)
//            {
//                Context.Particle.DrawTargetLine(
//                    Owner,
//                    "Target",
//                    CurrentTarget != null
//                        ? CurrentTarget.Position
//                        : (Output != null && Output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.Team != this.Owner.Team) + 1
//                           >= (this.Magnus.Amount.Value == 1 ? 0 : this.Magnus.Amount.Value))
//                            ? Output.CastPosition
//                            : CurrentTarget.Position,
//                    CurrentTarget != null ? Color.Red : Color.Aqua);
//            }
//            else
//            {
//                Context.Particle.Remove("Target");
//            }

//            fountain =
//                EntityManager.GetEntities<Unit>()
//                    .FirstOrDefault(
//                        x => x.IsValid && x.Team == this.Owner.Team && x.Name == "dota_fountain");

//            if (this.CurrentTarget != null)
//            {
//                teammateys =
//                    EntityManager.GetEntities<Hero>().Where(
//                            x =>
//                                x.IsValid && x.Team == this.Owner.Team && x != this.Owner &&
//                                x.Distance2D(targets.FirstOrDefault()) <= this.Magnus.Skewer.CastRange + 400)
//                        .OrderBy(x => x.Distance2D(targets.FirstOrDefault())).FirstOrDefault();

//                if (Magnus.SkewerTarget == SkewerTarget.Teammate && teammateys != null && this.CurrentTarget != null)
//                {
//                    position = teammateys.Position.Extend(this.CurrentTarget.Position,
//                        teammateys.Distance2D(this.CurrentTarget));
//                    skewerPosition = teammateys.Position;
//                }
//                else if (this.CurrentTarget != null)
//                {
//                    position = fountain.Position.Extend(this.CurrentTarget.Position,
//                        fountain.Distance2D(this.CurrentTarget));
//                    skewerPosition = fountain.Position;
//                }
//            }
//            else
//            {
//                teammateys = null;
//            }
//        }

//        private async Task AntiFail(CancellationToken token = new CancellationToken())
//        {
//            if (this.Magnus.ReversePolarity != null && this.Magnus.ReversePolarity.Ability.IsInAbilityPhase)
//            {
//                var enemies = EntityManager.GetEntities<Hero>()
//                    .Count(
//                        x =>
//                            Owner.Team != x.Team && x.IsValid && !x.IsIllusion && x.IsAlive && Owner.Distance2D(x) < 405);

//                if (enemies == 0)
//                {
//                    Owner.Stop();
//                    await Task.Delay(10, token);
//                }
//            }
//        }

//        private async Task SkewerStandalone(CancellationToken token = new CancellationToken())
//        {
//            var target = this.Context.TargetSelector.GetTargets().FirstOrDefault();
//            if (GameManager.IsPaused || !this.Owner.IsAlive || this.Magnus.Skewer.Ability.Cooldown > 5 || target == null || this.Owner.Distance2D(target) > 1200)
//            {
//                if (this.Magnus.SkewerStandalone.Value)
//                {
//                    this.Magnus.Context.Orbwalker.OrbwalkTo(null);
//                    return;
//                }
//                //await Task.Delay(250, token);
//                return;
//            }

//            try
//            {
//                fountain =
//                    EntityManager.GetEntities<Unit>()
//                        .FirstOrDefault(
//                            x => x.IsValid && x.Team == this.Owner.Team && x.Name == "dota_fountain");

//                skewerOnlyteammateys =
//                    EntityManager.GetEntities<Hero>().Where(
//                            x =>
//                                x.IsValid && x.Team == this.Owner.Team && x != this.Owner &&
//                                x.Distance2D(target) <= this.Magnus.Skewer.CastRange + 400)
//                        .OrderBy(x => x.Distance2D(target)).FirstOrDefault();

//                if (Magnus.SkewerTarget == SkewerTarget.Teammate && skewerOnlyteammateys != null)
//                {
//                    skewerOnlyPosition = target.Position.Extend(skewerOnlyteammateys.Position,
//                        -100);
//                    skewerOnlySkewerPosition = skewerOnlyteammateys.Position;
//                }
//                else
//                {
//                    skewerOnlyPosition = target.Position.Extend(fountain.Position,
//                        -100);
//                    skewerOnlySkewerPosition = fountain.Position;
//                }

//                if (this.Magnus.SkewerStandalone.Value)
//                {
//                    if (this.Owner.Distance2D(target) >= 500 && !this.Owner.IsChanneling())
//                    {
//                        this.Magnus.Context.Orbwalker.OrbwalkTo(null);
//                    }

//                    var blink = this.Magnus.BlinkDagger;
//                    var skewer = this.Magnus.Skewer;
//                    var force = this.Magnus.ForceStaff;
//                    var forceStaffReady = force != null && force.CanBeCasted;

//                    if (forceStaffReady)
//                    {
//                        if (this.Owner.Distance2D(skewerOnlyPosition) < 1800 &&
//                            this.Owner.Distance2D(skewerOnlyPosition) > 1200)
//                        {
//                            this.Owner.MoveToDirection(skewerOnlyPosition);
//                            await Task.Delay((int) (0.1 * 1000 + GameManager.Ping + 200), token);

//                            if (Owner.InFront(600).Distance(skewerOnlyPosition) < 1200)
//                            {
//                                force.Cast(Owner);
//                                await Task.Delay(10, token);
//                            }
//                        }
//                    }

//                    if (blink != null && blink.Item.IsValid && blink.CanBeCasted && skewer.CanBeCasted &&
//                        this.Owner.Distance2D(skewerOnlyPosition) <= blink.CastRange)
//                    {
//                        blink.Cast(skewerOnlyPosition);
//                        await Task.Delay(50, token);
//                        skewer.Cast(skewerOnlySkewerPosition);
//                        await Task.Delay(skewer.GetCastDelay(skewerOnlySkewerPosition), token);
//                    }
//                    else if (skewer.CanBeCasted &&
//                             this.Owner.Distance2D(skewerOnlyPosition) <= 70)
//                    {
//                        skewer.Cast(skewerOnlySkewerPosition);
//                        await Task.Delay(skewer.GetCastDelay(skewerOnlySkewerPosition), token);
//                    }
//                    else
//                    {
//                        this.Magnus.Context.Orbwalker.Move(skewerOnlyPosition);
//                        return;
//                    }
//                }
//                await Task.Delay(100, token);
//            }
//            catch (TaskCanceledException)
//            {   }
//            catch (Exception e)
//            {
//                LogManager.Error(e);
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
//                        breakerChanger = this.Magnus.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.Magnus.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.Magnus.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.Magnus.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.Magnus.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.Magnus.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.Magnus.Nullifier;
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

//                        var atos = this.Magnus.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(
//                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.Magnus.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.Magnus.DiffusalBlade;
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