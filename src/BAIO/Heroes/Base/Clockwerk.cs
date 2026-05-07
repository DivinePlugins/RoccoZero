using System.Collections.Generic;

namespace BAIO.Heroes.Base
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Handlers;
    using BAIO.Heroes.Modes.Combo;
    using BAIO.Interfaces;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;
    using Divine.Modifier;
    using Divine.Modifier.EventArgs;
    using Divine.Numerics;
    using Divine.Prediction;
    using Divine.Renderer;
    using Divine.Zero.Log;

    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_rattletrap;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory.Metadata;


    [ExportHero(HeroId.npc_dota_hero_rattletrap)]
    public class Clockwerk : BaseHero
    {
        public rattletrap_battery_assault BatteryAssault { get; private set; }

        public rattletrap_power_cogs Cogs { get; private set; }

        public rattletrap_rocket_flare Rocket { get; private set; }

        public rattletrap_hookshot Ulti { get; private set; }

        public Ability Overclocking { get; private set; }

        [ItemBinding]
        public item_abyssal_blade AbyssalBlade { get; private set; }

        [ItemBinding]
        public item_manta Manta { get; private set; }

        [ItemBinding]
        public item_nullifier Nullifier { get; private set; }

        [ItemBinding]
        public item_cyclone Euls { get; private set; }

        [ItemBinding]
        public item_diffusal_blade DiffusalBlade { get; private set; }

        [ItemBinding]
        public item_invis_sword ShadowBlade { get; private set; }

        [ItemBinding]
        public item_silver_edge SilverEdge { get; private set; }

        [ItemBinding]
        public item_blink BlinkDagger { get; private set; }

        [ItemBinding]
        public item_bloodthorn BloodThorn { get; private set; }

        [ItemBinding]
        public item_black_king_bar BlackKingBar { get; set; }

        [ItemBinding]
        public item_orchid Orchid { get; private set; }

        [ItemBinding]
        public item_mjollnir Mjollnir { get; private set; }

        [ItemBinding]
        public item_force_staff ForceStaff { get; private set; }

        [ItemBinding]
        public item_ethereal_blade EtherealBlade { get; private set; }

        [ItemBinding]
        public item_veil_of_discord VeilOfDiscord { get; private set; }

        [ItemBinding]
        public item_shivas_guard ShivasGuard { get; private set; }

        [ItemBinding]
        public item_sheepstick Sheepstick { get; private set; }

        [ItemBinding]
        public item_rod_of_atos RodOfAtos { get; private set; }

        [ItemBinding]
        public item_urn_of_shadows Urn { get; private set; }

        [ItemBinding]
        public item_spirit_vessel Vessel { get; private set; }

        [ItemBinding]
        public item_lotus_orb Lotus { get; private set; }

        [ItemBinding]
        public item_solar_crest SolarCrest { get; private set; }

        [ItemBinding]
        public item_blade_mail BladeMail { get; private set; }

        [ItemBinding]
        public item_medallion_of_courage Medallion { get; private set; }

        [ItemBinding]
        public item_heavens_halberd HeavensHalberd { get; private set; }

        [ItemBinding]
        public item_satanic Satanic { get; private set; }

        [ItemBinding]
        public item_mask_of_madness Mom { get; private set; }

        [ItemBinding]
        public item_power_treads Treads { get; private set; }

        public MenuHeroToggler UrnHeroes;
        public MenuHeroToggler VesselHeroes;
        public MenuHeroToggler NullifierHeroes;
        public MenuHeroToggler HalberdHeroes;
        public MenuHeroToggler OrchidHeroes;
        public MenuSwitcher BladeMailUsage;
        public MenuSwitcher LotusUsage;

        public Menu Drawings { get; private set; }

        public MenuSwitcher InformationDrawings { get; private set; }
        public MenuSwitcher InsecDrawings { get; private set; }
        public MenuSlider PosX { get; private set; }
        public MenuSlider PosY { get; private set; }
        public MenuSlider TextSize { get; private set; }
        public MenuToggleKey Insec { get; private set; }
        public MenuSelector MinimumHookChanceMenu { get; private set; }
        public MenuSwitcher RocketPush { get; private set; }

        public bool IsHookModifier { get; set; }
        public HitChance MinimumHookChance { get; private set; }

        public TaskHandler RocketHandler { get; private set; }

        protected override ComboMode GetComboMode()
        {
            return new ClockwerkCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.BatteryAssault = this.Context.AbilityFactory.GetAbility<rattletrap_battery_assault>();
            this.Cogs = this.Context.AbilityFactory.GetAbility<rattletrap_power_cogs>();
            this.Rocket = this.Context.AbilityFactory.GetAbility<rattletrap_rocket_flare>();
            this.Ulti = this.Context.AbilityFactory.GetAbility<rattletrap_hookshot>();
            this.Overclocking = this.Owner.GetAbilityById(AbilityId.rattletrap_overclocking);

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.Drawings = factory.CreateMenu("Drawings");
            var drawings = this.Drawings;

            this.InformationDrawings = drawings.CreateSwitcher("Enable Information Drawings");
            this.InsecDrawings = drawings.CreateSwitcher("Draw insec indicator");
            this.PosX = drawings.CreateSlider("Drawing X", 1520, 0, 3000);
            this.PosY = drawings.CreateSlider("Drawing Y", 920, 0, 3000);
            this.TextSize = drawings.CreateSlider("Text size", 50, 1, 100);

            this.MinimumHookChanceMenu = factory.CreateSelector("Minimum Hook Chance", new[] { "Medium", "Low", "High" });
            this.RocketPush = factory.CreateSwitcher("Rocket Pusher", false);
            this.RocketPush.SetTooltip("Will use rocket on creepwaves to push.");
            this.Insec = factory.CreateToggleKey("Insec controller");
            this.Insec.SetTooltip("If enabled, will try to push target with cogs rather than trap them inside of cogs.");
            //this.MinimumHookChance = this.MinimumHookChanceMenu.GetEnum<HitChance>();

            this.UrnHeroes = itemMenu.CreateHeroToggler("Urn", new());
            this.VesselHeroes = itemMenu.CreateHeroToggler("Vessel", new());
            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.HalberdHeroes = itemMenu.CreateHeroToggler("Halberd", new());
            this.OrchidHeroes = itemMenu.CreateHeroToggler("Orchid Bloodthorn", new());
            this.BladeMailUsage = itemMenu.CreateSwitcher("Use BladeMail?");
            this.LotusUsage = itemMenu.CreateSwitcher("Use Lotus?");

            this.RocketHandler = TaskHandler.Run(this.OnUpdate);
            this.MinimumHookChanceMenu.ValueChanged += this.MinimumHookChanceValueChanged;
           // this.RocketPush.ValueChanged += this.RocketPushPropertyChanged;
            ModifierManager.ModifierAdded += this.OnHookAdded;
            ModifierManager.ModifierRemoved += this.OnHookRemoved;
            RendererManager.Draw += OnDraw;
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                UrnHeroes.AddValue(heroId, true);
                VesselHeroes.AddValue(heroId, true);
                NullifierHeroes.AddValue(heroId, true);
                HalberdHeroes.AddValue(heroId, true);
                OrchidHeroes.AddValue(heroId, true);
            }
            else
            {
                UrnHeroes.RemoveValue(heroId);
                VesselHeroes.RemoveValue(heroId);
                NullifierHeroes.RemoveValue(heroId);
                HalberdHeroes.RemoveValue(heroId);
                OrchidHeroes.RemoveValue(heroId);
            }
        }

        private void OnDraw()
        {
            try
            {
                if (this.InformationDrawings)
                {
                    if (this.InsecDrawings)
                    {
                        var startPos = new Vector2(this.PosX.Value, this.PosY.Value);
                        var insecInd = this.Insec.Value;
                        RendererManager.DrawText("Insec: " + (insecInd ? "ON" : "OFF"), startPos, insecInd ? Color.LawnGreen : Color.Red, this.TextSize);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Error($"{exception}");
            }
        }

        protected override void OnDeactivate()
        {
            this.MinimumHookChanceMenu.ValueChanged -= this.MinimumHookChanceValueChanged;
            // this.RocketPush.ValueChanged -= this.RocketPushPropertyChanged;

            ModifierManager.ModifierAdded -= this.OnHookAdded;
            ModifierManager.ModifierRemoved -= this.OnHookRemoved;
            RendererManager.Draw -= OnDraw;
            this.RocketHandler.Cancel();

            base.OnDeactivate();
        }

       /* private void RocketPushPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RocketPush.Value = this.RocketPush.Item.GetValue<bool>();
        }*/

        private void MinimumHookChanceValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            this.MinimumHookChance = e.NewValue switch
            {
                "Medium" => HitChance.Medium,
                "Low" => HitChance.Low,
                "High" => HitChance.High,
                _ => throw new NotImplementedException(),
            };
        }

        private void OnHookAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if ((sender is Hero) && this.Owner.Team != sender.Team && (modifier.Name == "modifier_rattletrap_hookshot"))
            {
                LogManager.Debug($"Hook detected");
                this.IsHookModifier = true;
            }
        }

        private void OnHookRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if (this.Owner.Team != sender.Team && (modifier.Name == "modifier_rattletrap_hookshot"))
            {
                this.IsHookModifier = false;
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Rocket.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
                x => x.IsAlive && x.Team != this.Owner.Team && this.Rocket.CanBeCasted && this.Rocket.CanHit(x) &&
                     x.Distance2D(this.Owner) < 5000 && !x.IsIllusion &&
                     x.Health < GetDamage(x));

            if (killstealTarget != null)
            {
                var input = new PredictionInput(Owner, killstealTarget,
                        this.Rocket.GetCastDelay() / 1000f,
                        1750,
                        float.MaxValue,
                        600,
                        PredictionSkillshotType.SkillshotCircle,
                        true,
                        null)
                    {Delay = Math.Max(Rocket.CastPoint + GameManager.Ping, 0) / 1000f};
                var output = this.Rocket.GetPredictionOutput(input);

                if (output.HitChance >= HitChance.Medium)
                {
                    Rocket.Cast(output.CastPosition);
                    await this.AwaitKillstealDelay(Rocket.GetCastDelay(), token);
                }
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Rocket.CanBeCasted || !this.RocketPush.Value ||
                this.Config.General.ComboKey)
            {
                await Task.Delay(250, token);
                return;
            }

            var creeps = EntityManager.GetEntities<Unit>().OrderBy(x => x.Distance2D(this.Owner)).Where(
                x =>
                    x.IsValid && x is Creep && x.IsSpawned && !x.IsNeutral && x.Health + 50 <= GetDamage(x) &&
                    !x.IsMoving && x.IsAlive &&
                    x.Team != this.Owner.Team && x.Distance2D(this.Owner) > 1000).ToList();

            if (creeps.Any())
            {
                var input = new PredictionInput(Owner, creeps.Last(),
                    this.Rocket.GetCastDelay() / 1000f,
                    1750,
                    float.MaxValue,
                    600,
                    PredictionSkillshotType.SkillshotCircle,
                    true,
                    creeps)
                { Delay = Math.Max(Rocket.CastPoint + GameManager.Ping, 0) / 1000f };
                var output = this.Rocket.GetPredictionOutput(input);

                if (output.HitChance >= HitChance.Medium)
                {
                    Rocket.Cast(output.CastPosition);
                    await Task.Delay(Rocket.GetCastDelay(), token);
                }
            }
        }

        protected float RawDamage
        {
            get
            {
                var damage = this.Rocket.Ability.GetAbilitySpecialData("damage");

                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_clockwerk_2);
                if (talent?.Level > 0)
                {
                    damage += talent.GetAbilitySpecialData("value");
                }

                return damage;
            }
        }

        public float GetDamage(params Unit[] targets)
        {
            var totalDamage = 0.0f;

            var damage = this.RawDamage;
            var amplify = this.Owner.GetSpellAmplification();
            foreach (var target in targets)
            {
                var reduction = this.Rocket.Ability.GetDamageReduction(target, DamageType.Magical);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }
    }
}