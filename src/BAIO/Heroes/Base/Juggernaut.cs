//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_juggernaut;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Geometry;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_juggernaut)]
//    class Juggernaut : BaseHero
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private Unit healingWardUnit;

//        private float lastAttackTime;

//        protected TaskHandler WardControlHandler { get; private set; }

//        protected TaskHandler OmnislashBlinkHandler { get; private set; }

//        #region Abilities

//        public juggernaut_omni_slash Omnislash { get; private set; }

//        public juggernaut_healing_ward HealingWardAbility { get; private set; }

//        public juggernaut_blade_fury BladeFury { get; private set; }

//        public Ability SwiftSlash { get; set; }

//        #endregion

//        #region Items

//        [ItemBinding]
//        public item_abyssal_blade AbyssalBlade { get; private set; }

//        [ItemBinding]
//        public item_manta Manta { get; private set; }

//        [ItemBinding]
//        public item_nullifier Nullifier { get; private set; }

//        [ItemBinding]
//        public item_cyclone Euls { get; private set; }

//        [ItemBinding]
//        public item_diffusal_blade DiffusalBlade { get; private set; }

//        [ItemBinding]
//        public item_invis_sword ShadowBlade { get; private set; }

//        [ItemBinding]
//        public item_silver_edge SilverEdge { get; private set; }

//        [ItemBinding]
//        public item_blink BlinkDagger { get; private set; }

//        [ItemBinding]
//        public item_bloodthorn BloodThorn { get; private set; }

//        [ItemBinding]
//        public item_black_king_bar BlackKingBar { get; set; }

//        [ItemBinding]
//        public item_orchid Orchid { get; private set; }

//        [ItemBinding]
//        public item_mjollnir Mjollnir { get; private set; }

//        [ItemBinding]
//        public item_force_staff ForceStaff { get; private set; }

//        [ItemBinding]
//        public item_ethereal_blade EtherealBlade { get; private set; }

//        [ItemBinding]
//        public item_veil_of_discord VeilOfDiscord { get; private set; }

//        [ItemBinding]
//        public item_shivas_guard ShivasGuard { get; private set; }

//        [ItemBinding]
//        public item_sheepstick Sheepstick { get; private set; }

//        [ItemBinding]
//        public item_rod_of_atos RodOfAtos { get; private set; }

//        [ItemBinding]
//        public item_urn_of_shadows Urn { get; private set; }

//        [ItemBinding]
//        public item_spirit_vessel Vessel { get; private set; }

//        [ItemBinding]
//        public item_lotus_orb Lotus { get; private set; }

//        [ItemBinding]
//        public item_solar_crest SolarCrest { get; private set; }

//        [ItemBinding]
//        public item_blade_mail BladeMail { get; private set; }

//        [ItemBinding]
//        public item_medallion_of_courage Medallion { get; private set; }

//        [ItemBinding]
//        public item_heavens_halberd HeavensHalberd { get; private set; }

//        [ItemBinding]
//        public item_satanic Satanic { get; private set; }

//        [ItemBinding]
//        public item_mask_of_madness Mom { get; private set; }

//        [ItemBinding]
//        public item_power_treads Treads { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler AbyssalBladeHeroes;
//        public MenuHeroToggler OmniHeroes;
//        public MenuHeroToggler MantaHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler DiffusalHeroes;

//        public MenuSwitcher BladeFuryOnlyChase { get; private set; }
//        public MenuSwitcher BladeFuryOnCombo { get; private set; }
//        public MenuSwitcher ControlWardMenu { get; set; }
//        public MenuSwitcher OmnislashBlinkMenu { get; private set; }
//        public MenuHeroToggler InvisHeroes;

//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new JuggernautCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.BladeFury = this.Context.AbilityFactory.GetAbility<juggernaut_blade_fury>();
//            this.HealingWardAbility = this.Context.AbilityFactory.GetAbility<juggernaut_healing_ward>();
//            this.Omnislash = this.Context.AbilityFactory.GetAbility<juggernaut_omni_slash>();

//            this.OmnislashBlinkHandler = TaskHandler.Run(this.OmnislashBlinkController, false, false);
//            this.WardControlHandler = TaskHandler.Run(this.HealingWardController, false, false);

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.MantaHeroes = itemMenu.Item("Manta",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.DiffusalHeroes = itemMenu.Item("Diffusal Blade",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.InvisHeroes = itemMenu.Item("ShadowBlade / SilverEdge",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));

//            this.OmnislashBlinkMenu = factory.Item("Use Dagger with Ulti", true);
//            this.OmnislashBlinkMenu.SetTooltip("Use Dagger with ulti to keep ulti going if target is dead";
//            this.OmnislashBlinkMenu.ValueChanged += this.OmniBlinkOnPropertyChanged;

//            this.OmniHeroes = factory.Item("Omnislash Heroes",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));

//            this.ControlWardMenu = factory.Item("Control Healing Ward", true);
//            this.ControlWardMenu.SetTooltip("Enable it to let script control Healing Ward";
//            this.ControlWardMenu.ValueChanged += this.ControlWardOnPropertyChanged;

//            this.BladeFuryOnCombo = factory.Item("Use Bladefurry in combo", true);
//            this.BladeFuryOnCombo.SetTooltip("If disabled, won't use bladefury in combo.";


//            this.BladeFuryOnlyChase = factory.Item("Move to in front of enemy on Blade Fury, don't orbwalk", true);


//            if (this.OmnislashBlinkMenu)
//            {
//                ModifierManager.ModifierAdded += this.OnOmnislashAdded;
//            }

//            if (this.ControlWardMenu)
//            {
//                ObjectManager.OnAddEntity += this.OnHealingWardAdded;
//            }
//        }

//        private async Task OmnislashBlinkController(CancellationToken token)
//        {
//            var startTime = GameManager.GameTime;
//            var duration = this.Omnislash.DamageDuration;
//            var radius = this.Omnislash.Radius;
//            var myTeam = this.Owner.Team;

//            Unit lastTarget = null;

//            await Task.Delay((int) (this.Omnislash.TickRate * 1000), token);
//            while (((GameManager.GameTime - startTime) <= duration) && this.Owner.HasModifier(this.Omnislash.ModifierName))
//            {
//                var closestTarget = EntityManager<Unit>
//                    .Entities.Where(
//                        x =>
//                            x.IsVisible && x.IsAlive && (x.Team != myTeam) && !(x is Building) && x.IsRealUnit() &&
//                            (x.Distance2D(this.Owner) < radius))
//                    .OrderBy(x => x.Distance2D(this.Owner))
//                    .FirstOrDefault();

//                var blinkToNextTarget = closestTarget == null;
//                if (blinkToNextTarget)
//                {
//                    // null
//                }

//                if (!blinkToNextTarget)
//                {
//                    if (closestTarget.Health < this.Omnislash.GetTickDamage(closestTarget))
//                    {
//                        var timeFromLastAttack = GameManager.GameTime - this.lastAttackTime;
//                        var timeTillNextAttack = this.Omnislash.TickRate - timeFromLastAttack;
//                        if (timeTillNextAttack > 0.01)
//                        {
//                            var waitTime = (int) ((timeTillNextAttack + (this.Omnislash.TickRate / 4)) * 1000.0f);

//                            await Task.Delay(waitTime, token);
//                            blinkToNextTarget = true;
//                        }
//                    }
//                }

//                if (blinkToNextTarget)
//                {
//                    if (this.BlinkDagger.CanBeCasted)
//                    {
//                        var nextTarget = EntityManager<Hero>
//                            .Entities.Where(
//                                x => x.IsVisible
//                                     && x.IsAlive
//                                     && (x.Team != myTeam)
//                                     && (x != closestTarget)
//                                     && (x != lastTarget)
//                                     && !x.IsIllusion
//                                     && (x.Distance2D(this.Owner) < (this.BlinkDagger.CastRange + (radius / 2))))
//                            .OrderBy(x => x.Health)
//                            .FirstOrDefault();

//                        if (nextTarget != null)
//                        {
//                            var blinkPos = (nextTarget.Position - this.Owner.Position).Normalized();
//                            blinkPos = this.Owner.Position +
//                                       (blinkPos *
//                                        Math.Min(this.BlinkDagger.CastRange, nextTarget.Distance2D(this.Owner)));
//                            this.BlinkDagger.Cast(blinkPos);
//                        }
//                    }

//                    return;
//                }

//                lastTarget = closestTarget;
//                await Task.Delay(50, token);
//            }
//        }

//        private void OmniBlinkOnPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (this.OmnislashBlinkMenu)
//            {
//                ModifierManager.ModifierAdded += this.OnOmnislashAdded;
//            }
//            else
//            {
//                ModifierManager.ModifierAdded -= this.OnOmnislashAdded;
//                this.OmnislashBlinkHandler.Cancel();
//            }
//        }

//        private void OnOmnislashAdded(ModifierAddedEventArgs e)
//        {
//            if (sender != this.Owner)
//            {
//                return;
//            }

//            if (args.Modifier.Name != this.Omnislash.ModifierName)
//            {
//                return;
//            }

//            if (this.OmnislashBlinkMenu && (this.BlinkDagger != null))
//            {
//                this.OmnislashBlinkHandler.RunAsync();
//            }
//        }

//        private async Task HealingWardController(CancellationToken token)
//        {
//            if (UnitExtensions.HasAghanimsScepter(this.Owner))
//            {
//                this.SwiftSlash = this.Owner.Spellbook.Spells.FirstOrDefault<Ability>(x => x.Name == "juggernaut_swift_slash");
//            }

//            if (this.healingWardUnit == null)
//            {
//                return;
//            }

//            var team = this.healingWardUnit.Team;
//            var healingRadius = this.HealingWardAbility.Radius;
//            while ((this.healingWardUnit != null) && this.healingWardUnit.IsValid && this.healingWardUnit.IsAlive)
//            {
//                if (GameManager.IsPaused)
//                {
//                    await Task.Delay(125, token);
//                    continue;
//                }

//                var enemyHeroes =
//                    EntityManager.GetEntities<Hero>().Where(
//                            x =>
//                                x.IsAlive && x.IsVisible && (x.Team != team) &&
//                                (x.Distance2D(this.healingWardUnit) < 1000))
//                        .ToList();

//                var alliedHeroes = EntityManager<Hero>
//                    .Entities.Where(
//                        x =>
//                            x.IsAlive && x.IsVisible && (x.Team == team) && (x.HealthPercent() <= 0.9f) &&
//                            (x.Distance2D(this.healingWardUnit) < 800))
//                    .OrderBy(x => x.HealthPercent())
//                    .ToList();

//                if (!alliedHeroes.Any())
//                {
//                    if (!this.Owner.IsAlive)
//                    {
//                        await Task.Delay(125, token);
//                        continue;
//                    }

//                    alliedHeroes.Add(this.Owner);
//                }

//                var avoidCircles = new List<Polygon.Circle>(enemyHeroes.Count);
//                foreach (var enemyHero in enemyHeroes)
//                {
//                    var dangerRange = enemyHero.AttackRange(this.healingWardUnit);
//                    dangerRange = enemyHero.IsMelee ? dangerRange * 2f : dangerRange * 1.2f;

//                    var circle = new Polygon.Circle(enemyHero.Position, dangerRange);

//                    avoidCircles.Add(circle);
//                }

//                var healCircles = new List<Polygon.Circle>(alliedHeroes.Count);
//                foreach (var alliedHero in alliedHeroes)
//                {
//                    var circle = new Polygon.Circle(alliedHero.Position, healingRadius);
//                    if (avoidCircles.Exists(x => x.Center.Distance(circle.Center) <= Math.Abs(x.Radius - circle.Radius)))
//                    {
//                        continue;
//                    }

//                    healCircles.Add(circle);
//                }

//                var hasMoved = false;
//                if (healCircles.Any())
//                {
//                    while (healCircles.Count > 1)
//                    {
//                        var mecResult = MEC.GetMec(healCircles.Select((target) => target.Center).ToList());
//                        if ((mecResult.Radius != 0f) && (mecResult.Radius < healingRadius))
//                        {
//                            var movePos = new Vector3(
//                                healCircles.Count <= 2
//                                    ? (healCircles[0].Center + healCircles[1].Center) / 2
//                                    : mecResult.Center,
//                                this.healingWardUnit.Position.Z);

//                            if (avoidCircles.TrueForAll(x => !x.IsInside(movePos)))
//                            {
//                                this.healingWardUnit.Move(movePos);
//                                hasMoved = true;
//                                break;
//                            }
//                        }
//                        var itemToRemove = healCircles.Where(x => x.Center != this.Owner.Position.ToVector2())
//                            .MaxOrDefault((target) => healCircles[0].Center.DistanceSquared(target.Center));
//                        healCircles.Remove(itemToRemove);
//                    }
//                }

//                if (!healCircles.Any() || !hasMoved)
//                {
//                    var isOwnerLow = this.Owner.HealthPercent() <= 0.5f;
//                    var heroPos = isOwnerLow ? this.Owner.Position : alliedHeroes.First().Position;
//                    if (!avoidCircles.Any())
//                    {
//                        this.healingWardUnit.Move(heroPos);
//                    }
//                    else
//                    {
//                        var z = this.healingWardUnit.Position.Z;

//                        var clusterPos = Vector3.Zero;
//                        foreach (var avoidCircle in avoidCircles)
//                        {
//                            clusterPos += avoidCircle.Center.ToVector3(z);
//                        }

//                        clusterPos /= avoidCircles.Count;

//                        var movePos = (clusterPos - heroPos).Normalized();
//                        movePos = heroPos + (movePos * healingRadius * -1f);
//                        this.healingWardUnit.Move(movePos);
//                    }
//                }

//                await Task.Delay(125, token);
//            }

//            this.healingWardUnit = null;
//        }

//        private void ControlWardOnPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (this.ControlWardMenu)
//            {
//                ObjectManager.OnAddEntity += this.OnHealingWardAdded;
//            }
//            else
//            {
//                ObjectManager.OnAddEntity -= this.OnHealingWardAdded;
//                this.WardControlHandler.Cancel();
//            }
//        }

//        private void OnHealingWardAdded(EntityEventArgs args)
//        {
//            var unit = args.Entity as Unit;
//            if ((unit != null) && (unit.Team == this.Owner.Team) &&
//                (args.Entity.Name == "npc_dota_juggernaut_healing_ward"))
//            {
//                this.healingWardUnit = unit;
//                this.WardControlHandler.RunAsync();
//            }
//        }
//    }
//}