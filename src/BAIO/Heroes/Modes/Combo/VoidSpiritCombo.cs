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
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Prediction;
//using log4net;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using EntityExtensions = Ensage.Common.Extensions.EntityExtensions;
//using UnitExtensions = Ensage.Common.Extensions.UnitExtensions;

//namespace BAIO.Heroes.Modes.Combo
//{
//    internal class VoidSpiritCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly VoidSpirit VoidSpirit;
//        public TaskHandler Handlee { get; private set; }


//        public VoidSpiritCombo(VoidSpirit hero)
//            : base(hero)
//        {
//            this.VoidSpirit = hero;
//            Entity.OnParticleEffectAdded += this.EntityOnParticleEffectAdded;
//            ModifierManager.ModifierRemoved += this.UnitOnModifierRemoved;
//            this.Handlee = TaskHandler.Run(this.OnUpdate);
//        }

//        protected override void OnDeactivate()
//        {
//            this.Handlee.Cancel();
//            Entity.OnParticleEffectAdded -= this.EntityOnParticleEffectAdded;
//            ModifierManager.ModifierRemoved -= this.UnitOnModifierRemoved;
//            this.PortalLocations.Clear();
//            base.OnDeactivate();
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token))
//            {
//                return;
//            }

//            this.MaxTargetRange = Math.Max(this.MaxTargetRange, VoidSpirit.Ulti.CastRange * 1.1f);

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
//            {
//                this.VoidSpirit.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            try
//            {
//                if (!Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_void_spirit_dissimilate_phase"))
//                {
//                    var linkens = UnitExtensions.IsLinkensProtected(this.CurrentTarget);
//                    await BreakLinken(token);

//                    var veil = this.VoidSpirit.VeilOfDiscord;
//                    if (veil != null && veil.CanBeCasted && veil.CanHit(CurrentTarget))
//                    {
//                        veil.Cast(CurrentTarget.Position);
//                        await Task.Delay(veil.GetCastDelay(), token);
//                    }

//                    var hex = this.VoidSpirit.Sheepstick;
//                    if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
//                        this.VoidSpirit.HexHeroes[((Hero)CurrentTarget).HeroId])
//                    {
//                        hex.Cast(CurrentTarget);
//                        await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                    }

//                    var etherealBlade = this.VoidSpirit.EtherealBlade;
//                    if (etherealBlade != null && etherealBlade.CanBeCasted && etherealBlade.CanHit(CurrentTarget) && !linkens)
//                    {
//                        etherealBlade.Cast(CurrentTarget);
//                        await Task.Delay(etherealBlade.GetCastDelay(this.CurrentTarget), token);
//                    }

//                    var mjollnir = this.VoidSpirit.Mjollnir;
//                    if (mjollnir != null && mjollnir.CanBeCasted)
//                    {
//                        mjollnir.Cast(this.Owner);
//                        await Task.Delay(mjollnir.GetCastDelay(), token);
//                    }

//                    var bloodthorn = this.VoidSpirit.BloodThorn;
//                    if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens && this.VoidSpirit.OrchidHeroes[((Hero)CurrentTarget).HeroId])
//                    {
//                        bloodthorn.Cast(CurrentTarget);
//                        await Task.Delay(bloodthorn.GetCastDelay(), token);
//                    }

//                    var orchid = this.VoidSpirit.Orchid;
//                    if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens && this.VoidSpirit.OrchidHeroes[((Hero)CurrentTarget).HeroId])
//                    {
//                        orchid.Cast(CurrentTarget);
//                        await Task.Delay(orchid.GetCastDelay(), token);
//                    }

//                    var remnant = this.VoidSpirit.Remnant;
//                    if (remnant.CanBeCasted && EntityExtensions.Distance2D(this.Owner, this.CurrentTarget) <= remnant.CastRange)
//                    {
//                        var input = remnant.GetPredictionInput(this.CurrentTarget);
//                        var output = this.VoidSpirit.Context.PredictionManager.GetPrediction(input);

//                        if (output.HitChance >= HitChance.Medium)
//                        {
//                            remnant.Cast(Vector3Extensions.Extend(output.CastPosition, Ensage.SDK.Extensions.UnitExtensions.InFront(output.Unit, 50), 50));
//                            await Task.Delay(remnant.GetCastDelay(), token);
//                        }
//                    }

//                    var mod = this.Owner.FindModifier("modifier_void_spirit_astral_step_charge_counter");
//                    var stacks = mod?.StackCount;
//                    var ulti = this.VoidSpirit.Ulti;
//                    if (ulti.CanBeCasted)
//                    {
//                        if (this.Owner.Distance2D(this.CurrentTarget) <= ulti.Ability.GetAbilitySpecialData("max_travel_distance") && stacks > 1)
//                        {
//                            var input = new PredictionInput(this.Owner, this.CurrentTarget, ulti.GetCastDelay() / 1000f, float.MaxValue, ulti.Ability.GetAbilitySpecialData("max_travel_distance")
//                                , ulti.Radius);
//                            var output = this.VoidSpirit.Context.PredictionManager.GetPrediction(input);
//                            if (output.HitChance >= HitChance.Medium)
//                            {
//                                for (int i = 0; i < 1; i++)
//                                {
//                                    if (ulti.Cast(output.CastPosition))
//                                    {
//                                        await Task.Delay(ulti.GetCastDelay(output.CastPosition), token);
//                                    }
//                                }
//                            }
//                        }
//                        else if (this.Owner.Distance2D(this.CurrentTarget) <=
//                                 ulti.Ability.GetAbilitySpecialData("max_travel_distance") &&
//                                 this.Owner.Distance2D(this.CurrentTarget) > 400)
//                        {
//                            var input = new PredictionInput(this.Owner, this.CurrentTarget, ulti.GetCastDelay() / 1000f, float.MaxValue, ulti.Ability.GetAbilitySpecialData("max_travel_distance")
//                                , ulti.Radius);
//                            var output = this.VoidSpirit.Context.PredictionManager.GetPrediction(input);
//                            if (output.HitChance >= HitChance.Medium)
//                            {
//                                if (ulti.Cast(output.CastPosition))
//                                {
//                                    await Task.Delay(ulti.GetCastDelay(output.CastPosition), token);
//                                }
//                            }
//                        }
//                    }

//                    var targets =
//                        EntityManager.GetEntities<Hero>().Where(
//                                x =>
//                                    x.IsValid && x.Team != this.Owner.Team && !x.IsIllusion && !Ensage.SDK.Extensions.UnitExtensions.IsMagicImmune(x) &&
//                                    x.Distance2D(this.Owner) <= this.VoidSpirit.E.Radius)
//                            .ToList();

//                    var E = this.VoidSpirit.E;
//                    if (E.CanBeCasted && this.CurrentTarget.Distance2D(this.Owner) <= E.Radius &&
//                        targets.Count >= this.VoidSpirit.ResonantPulseCount.Value.Value)
//                    {
//                        if (E.Cast())
//                        {
//                            await Task.Delay(E.GetCastDelay(), token);
//                        }
//                    }

//                    var W = this.VoidSpirit.W;
//                    if (W.CanBeCasted && this.CurrentTarget.Distance2D(this.Owner) <= 400)
//                    {
//                        if (W.Cast())
//                        {
//                            await Task.Delay(W.GetCastDelay(), token);
//                        }
//                    }

//                    var abyssal = this.VoidSpirit.AbyssalBlade;
//                    if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
//                        this.VoidSpirit.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
//                    {
//                        abyssal.Cast(CurrentTarget);
//                        await Task.Delay(abyssal.GetCastDelay(), token);
//                    }

//                    var shivas = this.VoidSpirit.ShivasGuard;
//                    if (shivas != null && shivas.CanBeCasted && shivas.CanHit(this.CurrentTarget))
//                    {
//                        if (shivas.Cast())
//                        {
//                            await Task.Delay(10, token);
//                        }
//                    }

//                    var nullifier = this.VoidSpirit.Nullifier;
//                    if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
//                        this.VoidSpirit.NullifierHeroes[((Hero)CurrentTarget).HeroId])
//                    {
//                        nullifier.Cast(CurrentTarget);
//                        await Task.Delay(nullifier.GetCastDelay(), token);
//                    }

//                    if (this.VoidSpirit.Dagon != null &&
//                        this.VoidSpirit.Dagon.Item.IsValid &&
//                        this.CurrentTarget != null &&
//                        this.VoidSpirit.Dagon.Item.CanBeCasted(this.CurrentTarget))
//                    {
//                        this.VoidSpirit.Dagon.Cast(this.CurrentTarget);
//                        await Task.Delay(this.VoidSpirit.Dagon.GetCastDelay(this.CurrentTarget), token);
//                    }
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

//            if (!Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_void_spirit_dissimilate_phase"))
//            {
//                this.OrbwalkToTarget();
//            }
//        }

//        private async Task OnUpdate(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive || !PortalLocations.Any())
//            {
//                await Task.Delay(250, token);
//                return;
//            }

//            if (Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_void_spirit_dissimilate_phase"))
//            {
//                if (UnitExtensions.FindModifier(this.Owner, "modifier_void_spirit_dissimilate_phase").RemainingTime >= 0.2f && this.VoidSpirit.RandomlyJump)
//                {
//                    foreach (var portalLocation in PortalLocations.ToList())
//                    {
//                        this.Owner.Move(portalLocation);
//                        await Task.Delay(50, token);
//                        if (UnitExtensions.FindModifier(this.Owner, "modifier_void_spirit_dissimilate_phase")
//                                .RemainingTime <= 0.2f)
//                        {
//                            break;
//                        }
//                    }
//                }
//                else if (this.CurrentTarget != null)
//                {
//                    var closestPortal = this.PortalLocations
//                        .OrderBy(x => VectorExtensions.Distance2D(x, this.CurrentTarget)).FirstOrDefault();
//                    if (this.Owner.Move(closestPortal))
//                    {
//                        await Task.Delay(50, token);
//                    }
//                }
//                else
//                {
//                    var closestToMouse = this.PortalLocations.OrderBy(x => x.Distance2D(GameManager.MousePosition))
//                        .FirstOrDefault();
//                    if (this.Owner.Move(closestToMouse))
//                    {
//                        await Task.Delay(50, token);
//                    }
//                }
//            }
//        }

//        private void UnitOnModifierRemoved(ModifierRemovedEventArgs e)
//        {
//            var modifier = args.Modifier;

//            if (!sender.IsValid || ((Unit) sender) != this.Owner || args.Modifier.Name != "modifier_void_spirit_dissimilate_phase")
//            {
//                return;
//            }

//            PortalLocations.Clear();
//        }

//        private async void EntityOnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
//        {
//            await Task.Delay(1);
//            var particle = args.ParticleEffect;

//            if (!this.IsValid(sender, particle, args.Name))
//            {
//                return;
//            }

//            if (particle.Name == "particles/units/heroes/hero_void_spirit/dissimilate/void_spirit_dissimilate.vpcf")
//            {
//                var portal = args.ParticleEffect.GetControlPoint(0);
//                PortalLocations.Add(portal);
//            }
//        }

//        private List<Vector3> PortalLocations = new List<Vector3>();

//        private bool IsValid(Entity sender, ParticleEffect particle, string name)
//        {
//            if (sender?.IsValid != true)
//            {
//                return false;
//            }

//            if (particle?.IsValid != true)
//            {
//                return false;
//            }

//            if (particle.Name != "particles/units/heroes/hero_void_spirit/dissimilate/void_spirit_dissimilate.vpcf")
//            {
//                return false;
//            }

//            return true;
//        }

//        protected async Task BreakLinken(CancellationToken token)
//        {
//            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
//            {
//                try
//                {
//                    List<KeyValuePair<AbilityId, bool>> breakerChanger = new List<KeyValuePair<AbilityId, bool>>();

//                    if (UnitExtensions.IsLinkensProtected(this.CurrentTarget))
//                    {
//                        breakerChanger = this.VoidSpirit.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.VoidSpirit.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.VoidSpirit.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.VoidSpirit.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.VoidSpirit.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.VoidSpirit.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.VoidSpirit.Nullifier;
//                        if (nullifier != null
//                            && nullifier.Item.Id == order.Key
//                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
//                        {
//                            nullifier.Cast(this.CurrentTarget);
//                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var atos = this.VoidSpirit.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.VoidSpirit.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.VoidSpirit.DiffusalBlade;
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