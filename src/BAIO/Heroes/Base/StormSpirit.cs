using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Handlers;
using BAIO.Heroes.Modes.Combo;
using BAIO.Heroes.Modes.Harass;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Update;
using Divine.Zero.Log;

using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_storm_spirit;
using Ensage.SDK.Inventory.Metadata;
using Divine.Menu.Items;
using Divine.Renderer;
using Divine.Menu.EventArgs;
using Divine.Prediction;
using Divine.Numerics;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Entity;
using BAIO.Core.Extensions;

namespace BAIO.Heroes.Base
{
    public enum DrawsTypes
    {
        PercentLeft,
        PercentWillBeUsed,
        RawCost,
        RawLeft
    }

    [ExportHero(HeroId.npc_dota_hero_storm_spirit)]
    public class StormSpirit : BaseHero
    {
        [ItemBinding]
        public item_soul_ring SoulRing { get; private set; }

        [ItemBinding]
        public item_shivas_guard ShivasGuard { get; private set; }


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
        public item_armlet Armlet { get; private set; }

        private Item TeleportScrool { get; set; }

        [ItemBinding]
        private item_travel_boots Travel1 { get; set; }

        [ItemBinding]
        private item_travel_boots_2 Travel2 { get; set; }

        public TravelBoots TravelBoots
        {
            get
            {
                if (this.Travel2 != null)
                {
                    return this.Travel2;
                }
                return this.Travel1;
            }
        }

        public storm_spirit_static_remnant Remnant { get; private set; }

        public storm_spirit_electric_vortex Vortex { get; private set; }

        public storm_spirit_overload Overload { get; set; }

        public storm_spirit_ball_lightning Ulti { get; private set; }

        public Menu OverloadKeyMenu;

        public MenuHoldKey EscapeKey;
        public MenuHoldKey OverloadKey;

        public MenuSlider DistanceForUlt;
        public MenuSlider DrawingX;
        public MenuSlider DrawingY;
        public MenuSlider OverloadKeyDelay;
        public MenuSelector DrawingPrefMenu { get; private set; }
        public MenuSwitcher IgnoreOverload { get; private set; }
        public MenuSwitcher CheckModifier { get; private set; }

        public MenuHeroToggler BtAndOrchidHeroes;
        public MenuHeroToggler SheepHeroes;
        public MenuHeroToggler NullifierHeroes;

        public TaskHandler EscapeHandler { get; private set; }

        public bool InOverload { get; set; }
        public bool InUlti { get; set; }

        public DrawsTypes DrawType;

        protected override ComboMode GetComboMode()
        {
            return new StormSpiritCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new StormSpiritHarass(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.OverloadKeyMenu = factory.CreateMenu("overloadmenu", "Overload Menu");
            this.OverloadKey = OverloadKeyMenu.CreateHoldKey("Overload Charge Key");
            this.OverloadKey.SetTooltip("Will use ulti on position to get a overload charge");
            this.CheckModifier = OverloadKeyMenu.CreateSwitcher("Check Overload modifier");
            this.CheckModifier.SetTooltip("Will check if you have overload modifier and won't use ulti if you do");
            this.OverloadKeyDelay = OverloadKeyMenu.CreateSlider("Overload key delay/cooldown", 100, 1, 500);

            this.DistanceForUlt = factory.CreateSlider("Max Distance for Ult", 1000, 0, 10000);
            this.DistanceForUlt.SetTooltip("Enemies outside of this range will not be chased.");
            this.EscapeKey = factory.CreateHoldKey("Escape Key");
            this.EscapeKey.SetTooltip("Will use ulti + tp/travel to run away in dangerous situations.");
            this.IgnoreOverload = factory.CreateSwitcher("Ignore Overload for vortex", false);
            this.DrawingPrefMenu = factory.CreateSelector("Drawing Type", new[] { "PercentWillBeUsed", "PercentLeft", "RawCost", "RawLeft" });
            this.DrawingX = factory.CreateSlider("Drawing X", 0, 0, 4000);
            this.DrawingY = factory.CreateSlider("Drawing Y", 0, 0, 4000);

            this.BtAndOrchidHeroes = itemMenu.CreateHeroToggler("Orchid/Bloodthorn", new());
            this.SheepHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());

            this.EscapeHandler = TaskHandler.Run(this.OnUpdate);

            this.Remnant = this.Context.AbilityFactory.GetAbility<storm_spirit_static_remnant>();
            this.Vortex = this.Context.AbilityFactory.GetAbility<storm_spirit_electric_vortex>();
            this.Overload = this.Context.AbilityFactory.GetAbility<storm_spirit_overload>();
            this.Ulti = this.Context.AbilityFactory.GetAbility<storm_spirit_ball_lightning>();

            this.DrawingPrefMenu.ValueChanged += this.DrawingPrefMenuPropertyChanged;

            ModifierManager.ModifierAdded += this.OnOverloadAdded;
            ModifierManager.ModifierRemoved += this.OnOverloadRemoved;
            ModifierManager.ModifierAdded += this.OnUltiAdded;
            ModifierManager.ModifierRemoved += this.OnUltiRemoved;
            RendererManager.Draw += this.OnDraw;
        }

        protected override void OnDeactivate()
        {
            this.DrawingPrefMenu.ValueChanged -= this.DrawingPrefMenuPropertyChanged;
            ModifierManager.ModifierAdded -= this.OnOverloadAdded;
            ModifierManager.ModifierRemoved -= this.OnOverloadRemoved;
            ModifierManager.ModifierAdded -= this.OnUltiAdded;
            ModifierManager.ModifierRemoved -= this.OnUltiRemoved;
            RendererManager.Draw -= this.OnDraw;
            this.EscapeHandler.Cancel();
            base.OnDeactivate();
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                BtAndOrchidHeroes.AddValue(heroId, true);
                SheepHeroes.AddValue(heroId, true);
                NullifierHeroes.AddValue(heroId, true);
            }
            else
            {
                BtAndOrchidHeroes.RemoveValue(heroId);
                SheepHeroes.RemoveValue(heroId);
                NullifierHeroes.RemoveValue(heroId);
            }
        }

        private void DrawingPrefMenuPropertyChanged(MenuSelector selector, SelectorEventArgs e)
        {
            this.DrawType = e.NewValue switch
            {
                "PercentLeft" => DrawsTypes.PercentLeft,
                "PercentWillBeUsed" => DrawsTypes.PercentWillBeUsed,
                "RawCost" => DrawsTypes.RawCost,
                "RawLeft" => DrawsTypes.RawLeft,
                _ => throw new NotImplementedException(),
            };
        }

        private void OnOverloadAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if ((sender is Hero) && this.Owner == sender && (modifier.Name == "modifier_storm_spirit_overload"))
            {
                //LogManager.Debug($"InOverload");
                this.InOverload = true;
            }
        }

        private void OnOverloadRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if (this.Owner == sender && (modifier.Name == "modifier_storm_spirit_overload"))
            {
                this.InOverload = false;
            }
        }

        private void OnUltiAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if ((sender is Hero) && this.Owner == sender &&
                (modifier.Name == "modifier_storm_spirit_ball_lightning"))
            {
                //LogManager.Debug($"OnUlti");
                this.InUlti = true;
            }
        }

        private void OnUltiRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if (this.Owner == sender && (modifier.Name == "modifier_storm_spirit_ball_lightning"))
            {
                this.InUlti = false;
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Ulti.CanBeCasted)
            {
                await Task.Delay(250, token);
                return;
            }

            if (this.EscapeKey)
            {
                try
                {
                    var fountainPosition =
                        EntityManager.GetEntities<Unit>().First(
                            x => x.Team == this.Owner?.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain).Position;

                    var inUltimate = this.InUlti ||
                                     this.Ulti.Ability.IsInAbilityPhase;
                    TeleportScrool = EntityManager.GetEntities<Item>().FirstOrDefault(x => x.Owner == this.Owner && x.IsValid && x.Id == AbilityId.item_tpscroll);

                    if (!inUltimate && this.Ulti.CanBeCasted && !this.Owner.IsChanneling())
                    {
                        if (Extensions.PositionCamera(fountainPosition.X, fountainPosition.Y))
                        {
                            if (this.Ulti.Cast(fountainPosition))
                            {
                                await Task.Delay(150, token);
                            }
                        }
                    }

                    if (TeleportScrool != null && TeleportScrool.CanBeCasted() && !this.Owner.IsChanneling())
                    {
                        TeleportScrool?.Cast(fountainPosition);
                        await Task.Delay(500, token);
                    }

                    await Task.Delay(100, token);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
                catch (Exception e)
                {
                    LogManager.Error(e);
                }
            }

            if (this.OverloadKey)
            {
                var pos = this.Owner.Position;
                var onOverload = this.InOverload;
                var checkModif = this.CheckModifier;
                var delay = this.OverloadKeyDelay;
                var inUltimate = this.InUlti ||
                                 this.Ulti.Ability.IsInAbilityPhase;
                if (!inUltimate && this.Ulti.CanBeCasted && !this.Owner.IsChanneling())
                {
                    if (checkModif && onOverload)
                    {
                        return;
                    }
                    else
                    {
                        var closestEnemy = EntityManager.GetEntities<Hero>()
                            .FirstOrDefault(x => x != null && x.IsValid && x.IsAlive && x.Team != this.Owner.Team && x.Distance2D(this.Owner) <= 1000);
                        if (closestEnemy != null)
                        {
                            this.Ulti.Cast(UltiPos(closestEnemy));
                            await Task.Delay(delay, token);
                        }
                        else
                        {
                            this.Ulti.Cast(UltiPos(Extensions.FacePosition(this.Owner)));
                            await Task.Delay(delay, token);
                        }
                    }
                }
                await Task.Delay(delay, token);
            }
        }

        private void OnDraw()
        {
            if (this.Ulti.Ability.Level <= 0)
            {
                return;
            }

            var drawPos = new Vector2(DrawingX, DrawingY);
            var mousePos = GameManager.MousePosition;
            var cost = Math.Ceiling(this.Ulti.GetManaCost(mousePos));
            var minusCost = this.Owner.Mana - cost;
            var percentLeft = Math.Ceiling((minusCost / this.Owner.MaximumMana) * 100);
            var percentUsed = Math.Ceiling((cost / this.Owner.Mana) * 100);
            var rawLeft = Math.Ceiling((this.Owner.Mana - cost));

            var screenPos = RendererManager.WorldToScreen(mousePos, true);
            if (!screenPos.IsZero)
            {
                if (DrawType == DrawsTypes.PercentLeft)
                {
                    RendererManager.DrawText($"Left Percent {percentLeft}%", drawPos, Color.Red, 30);
                }
                else if (DrawType == DrawsTypes.PercentWillBeUsed)
                {
                    RendererManager.DrawText($"Percent Used {percentUsed}%", drawPos, Color.Red, 30);
                }
                else if (DrawType == DrawsTypes.RawLeft)
                {
                    RendererManager.DrawText($"Raw Left {rawLeft}", drawPos, Color.Red, 30);
                }
                else if (DrawType == DrawsTypes.RawCost)
                {
                    RendererManager.DrawText($"Raw Cost {cost}", drawPos, Color.Red, 30);
                }
            }
        }

        // Bu method rakibe karşı olan açıyı alıp, en uygun ulti pozisyonunu döndürüyor.
        public Vector3 UltiPos(Unit target)
        {
            var l = (target.Distance2D(this.Owner) - 50) / 50;
            var posA = target.Position;
            var posB = this.Owner.Position;
            var x = (posA.X + (l * posB.X)) / (1 + l);
            var y = (posA.Y + (l * posB.Y)) / (1 + l);
            return new Vector3((int)x, (int)y, posA.Z);
        }

        public Vector3 UltiPos(Vector3 pos)
        {
            var l = (pos.Distance2D(this.Owner.Position) - 50) / 50;
            var posA = pos;
            var posB = this.Owner.Position;
            var x = (posA.X + (l * posB.X)) / (1 + l);
            var y = (posA.Y + (l * posB.Y)) / (1 + l);
            return new Vector3((int)x, (int)y, posA.Z);
        }
    }
}