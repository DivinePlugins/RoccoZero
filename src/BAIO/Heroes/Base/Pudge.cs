using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Handlers;
using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Numerics;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.Particle;
using Divine.Prediction;
using Divine.Renderer;
using Divine.Zero.Log;

using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_pudge;
using Ensage.SDK.Inventory.Metadata;

namespace BAIO.Heroes.Base
{
    [ExportHero(HeroId.npc_dota_hero_pudge)]
    internal class Pudge : BaseHero
    {
        public pudge_meat_hook Hook { get; private set; }

        public pudge_rot Rot { get; private set; }

        public pudge_dismember Dismember { get; private set; }

        [ItemBinding]
        public item_soul_ring SoulRing { get; private set; }

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

        [ItemBinding]
        public item_dagon Dagon1 { get; set; }

        [ItemBinding]
        public item_dagon_2 Dagon2 { get; set; }

        [ItemBinding]
        public item_dagon_3 Dagon3 { get; set; }

        [ItemBinding]
        public item_dagon_4 Dagon4 { get; set; }

        [ItemBinding]
        public item_dagon_5 Dagon5 { get; set; }

        public Dagon Dagon => Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;

        public Menu Drawings { get; private set; }
        public MenuSelector MinimumHookChanceItem { get; private set; }
        public MenuSwitcher DrawHookRange { get; private set; }
        public MenuSwitcher BladeMailUsage;
        public MenuSwitcher InformationDrawings { get; private set; }
        public MenuToggleKey KeepPosition { get; private set; }
        public MenuToggleKey BlinkCombo { get; private set; }
        public MenuHoldKey AllyHook { get; private set; }
        public MenuSlider PosX { get; private set; }
        public MenuSlider PosY { get; private set; }
        public MenuSlider TextSize { get; private set; }
        public MenuSlider HealThreshold { get; private set; }

        public MenuHeroToggler UrnHeroes;
        public MenuHeroToggler VesselHeroes;
        public MenuHeroToggler AbyssalBladeHeroes;
        public MenuHeroToggler HalberdHeroes;
        public MenuHeroToggler NullifierHeroes;
        public MenuHeroToggler EbHeroes;
        public MenuHeroToggler ScepterHealHeroes;

        public bool HasUserEnabledRot { get; private set; }
        public bool HookModifierDetected { get; set; }
        public HitChance MinimumHookChance { get; private set; }
        public TaskHandler OnUpdateHandler { get; private set; }

        public Vector3 ClosestTpPoint { get; set; }

        public List<Vector3> TpColors { get; } = new List<Vector3>()
        {
            new Vector3(0.2f, 0.4588236f, 1),
            new Vector3(0.4f, 1, 0.7490196f),
            new Vector3(0.7490196f, 0, 0.7490196f),
            new Vector3(0.9529412f, 0.9411765f, 0.04313726f),
            new Vector3(1, 0.4196079f, 0),
            new Vector3(0.9960785f, 0.5254902f, 0.7607844f),
            new Vector3(0.6313726f, 0.7058824f, 0.2784314f),
            new Vector3(0.3960785f, 0.8509805f, 0.9686275f),
            new Vector3(0, 0.5137255f, 0.1294118f),
            new Vector3(0.6431373f, 0.4117647f, 0)
        };


        protected override ComboMode GetComboMode()
        {
            return new PudgeCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Hook = this.Context.AbilityFactory.GetAbility<pudge_meat_hook>();
            this.Rot = this.Context.AbilityFactory.GetAbility<pudge_rot>();
            this.Dismember = this.Context.AbilityFactory.GetAbility<pudge_dismember>();

            this.ClosestTpPoint = new Vector3();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.Drawings = factory.CreateMenu("Drawings");

            var drawings = this.Drawings;

            this.UrnHeroes = itemMenu.CreateHeroToggler("Urn", new());
            this.VesselHeroes = itemMenu.CreateHeroToggler("Vessel", new());
            this.HalberdHeroes = itemMenu.CreateHeroToggler("Halberd", new());
            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.AbyssalBladeHeroes = itemMenu.CreateHeroToggler("Abyssal Blade", new());
            this.EbHeroes = itemMenu.CreateHeroToggler("Ethereal Blade", new());

            this.MinimumHookChanceItem = factory.CreateSelector("Minimum Hook Chance", new[] { "Medium", "Low", "High" });
            this.BladeMailUsage = itemMenu.CreateSwitcher("Use BladeMail?");

            this.DrawHookRange = drawings.CreateSwitcher("Draw hook range");
            this.InformationDrawings = drawings.CreateSwitcher("Enable Information Drawings");
            this.KeepPosition = drawings.CreateToggleKey("Keep position when combo key pressed");
            this.BlinkCombo = drawings.CreateToggleKey("Blink Combo");
            this.ScepterHealHeroes = factory.CreateHeroToggler("Heal Heroes", new());
            this.HealThreshold = factory.CreateSlider("Heal Threshold", 1000, 1, 2500);
            this.AllyHook = factory.CreateHoldKey("Hook Ally");
            this.PosX = drawings.CreateSlider("Drawing X", 1520, 0, 4000);
            this.PosY = drawings.CreateSlider("Drawing Y", 920, 0, 4000);
            this.TextSize = drawings.CreateSlider("Text size", 50, 1, 100);

            this.MinimumHookChanceItem.ValueChanged += this.MinimumHookChancePropertyChanged;

            this.OnUpdateHandler = TaskHandler.Run(this.OnUpdate);
            ModifierManager.ModifierAdded += this.OnHookAdded;
            ModifierManager.ModifierRemoved += this.OnHookRemoved;
            OrderManager.OrderAdding += this.OnExecuteOrder;
            RendererManager.Draw += OnDraw;
        }

        private protected override void OnMenuAllyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                ScepterHealHeroes.AddValue(heroId, true);
            }
            else
            {
                ScepterHealHeroes.RemoveValue(heroId);
            }
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                UrnHeroes.AddValue(heroId, true);
                VesselHeroes.AddValue(heroId, true);
                HalberdHeroes.AddValue(heroId, true);
                NullifierHeroes.AddValue(heroId, true);
                AbyssalBladeHeroes.AddValue(heroId, true);
                EbHeroes.AddValue(heroId, true);
            }
            else
            {
                UrnHeroes.RemoveValue(heroId);
                VesselHeroes.RemoveValue(heroId);
                HalberdHeroes.RemoveValue(heroId);
                NullifierHeroes.RemoveValue(heroId);
                AbyssalBladeHeroes.RemoveValue(heroId);
                EbHeroes.RemoveValue(heroId);
            }
        }

        protected override void OnDeactivate()
        {
            this.MinimumHookChanceItem.ValueChanged -= this.MinimumHookChancePropertyChanged;

            OrderManager.OrderAdding -= this.OnExecuteOrder;
            ModifierManager.ModifierAdded -= this.OnHookAdded;
            ModifierManager.ModifierRemoved -= this.OnHookRemoved;
            this.OnUpdateHandler.Cancel();
            RendererManager.Draw -= OnDraw;

            base.OnDeactivate();
        }

        private void MinimumHookChancePropertyChanged(MenuSelector selector, SelectorEventArgs e)
        {
            this.MinimumHookChance = e.NewValue switch
            {
                "Medium" => HitChance.Medium,
                "Low" => HitChance.Low,
                "High" => HitChance.High,
                _ => throw new NotImplementedException(),
            };
        }

        private void OnExecuteOrder(OrderAddingEventArgs e)
        {
            var order = e.Order;

            if (!e.IsCustom && (order.Type == OrderType.CastToggle) && (order.Ability == this.Rot.Ability))
            {
                this.HasUserEnabledRot = !this.Rot.Enabled;
            }
        }

        private void OnHookAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if ((sender is Hero) && this.Owner.IsEnemy(sender) && (modifier.Name == this.Hook.TargetModifierName))
            {
                //LogManager.Debug($"Hook used");
                this.HookModifierDetected = true;
            }
        }
        private void OnHookRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if (this.Owner.IsEnemy(sender) && (modifier.Name == this.Hook.TargetModifierName))
            {
                this.HookModifierDetected = false;
            }
        }

        private void OnDraw()
        {
            try
            {
                if (this.InformationDrawings)
                {
                    var startPos = new Vector2(this.PosX.Value, this.PosY.Value);
                    var keepPos = this.KeepPosition;
                    RendererManager.DrawText("Keep Position: " + (keepPos ? "ON" : "OFF"), startPos, keepPos ? Color.LawnGreen : Color.Red, this.TextSize);
                    var blinkCombo = this.BlinkCombo;
                    RendererManager.DrawText("Blink Combo: " + (blinkCombo ? "ON" : "OFF"), startPos + new Vector2(0, 30), blinkCombo ? Color.LawnGreen : Color.Red, this.TextSize);
                }
            }
            catch (Exception exception)
            {
                LogManager.Error($"{exception}");
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || Dagon == null || !this.Dagon.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTargetDagon = EntityManager.GetEntities<Hero>().FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.Dagon.CanHit(x)
                     && this.Dagon.GetDamage(x) > x.Health);

            if (killstealTargetDagon != null)
            {
                if (this.Dagon.Cast(killstealTargetDagon))
                {
                    await this.AwaitKillstealDelay(this.Dagon.GetCastDelay(killstealTargetDagon), token);
                }
            }
            await Task.Delay(125, token);
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Rot.CanBeCasted)
            {
                await Task.Delay(250, token);
                return;
            }

            if (DrawHookRange && Hook.Ability.Level > 0)
            {
                ParticleManager.CreateRangeParticle("HookRange", Owner, Hook.CastRange, Color.Red);
            }
            else
            {
                ParticleManager.DestroyParticle("HookRange");
            }

            var rotEnabled = this.Rot.Enabled;
            if (rotEnabled && !this.HasUserEnabledRot && !this.HookModifierDetected)
            {
                var enemyNear = EntityManager.GetEntities<Hero>().Any(x => x.IsVisible && x.IsAlive && this.Owner.IsEnemy(x) && this.Rot.CanHit(x));
                if (!enemyNear)
                {
                    this.Rot.Enabled = false;
                    rotEnabled = false;
                    await Task.Delay(this.Rot.GetCastDelay(), token);
                }
            }

            if (this.Owner.HasAghanimsScepter())
            {
                var dismember = this.Dismember;
                var healTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(x =>
                    x != null && x.IsValid && x.Team == this.Owner.Team && x != this.Owner && x.IsVisible &&
                    x.IsAlive && x.Distance2D(this.Owner) <= this.Dismember.CastRange * 1.2f && x.Health < HealThreshold.Value &&
                    !x.HasModifier("modifier_pudge_swallow_hide") &&
                    this.ScepterHealHeroes[x.HeroId]);

                if (healTarget != null && this.Dismember.CanBeCasted)
                {
                    if (this.Dismember.Cast(healTarget))
                    {
                        await Task.Delay(this.Dismember.GetCastDelay(healTarget) + 200, token);
                    }
                }
            }

            await Task.Delay(125, token);
        }
    }
}
