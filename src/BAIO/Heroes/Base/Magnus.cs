//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_antimage;
//using Ensage.SDK.Abilities.npc_dota_hero_magnataur;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;

//namespace BAIO.Heroes.Base
//{
//    public enum SkewerTarget
//    {
//        Teammate,
//        Fountain
//    }

//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_magnataur)]
//    internal class Magnus : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public magnataur_shockwave Shockwave { get; private set; }

//        public magnataur_empower Empower { get; private set; }

//        public magnataur_skewer Skewer { get; private set; }

//        public magnataur_reverse_polarity ReversePolarity { get; private set; }

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

//        [ItemBinding]
//        public item_armlet Armlet { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler AbyssalBladeHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler OrchidBloodthornHeroes;
//        public MenuHeroToggler InvisHeroes;
//        public MenuHeroToggler EmpowerHeroes;
//        public MenuSlider Amount;
//        public MenuItem<StringList> SkewerPrefMenu { get; private set; }
//        public MenuSwitcher AutoCombo;
//        public MenuSwitcher DebugDrawings;
//        public MenuItem<KeyBind> SkewerStandalone;
//        #endregion

//        public SkewerTarget SkewerTarget;

//        protected override ComboMode GetComboMode()
//        {
//             return new MagnusCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.Shockwave = this.Context.AbilityFactory.GetAbility<magnataur_shockwave>();
//            this.Empower = this.Context.AbilityFactory.GetAbility<magnataur_empower>();
//            this.Skewer = this.Context.AbilityFactory.GetAbility<magnataur_skewer>();
//            this.ReversePolarity = this.Context.AbilityFactory.GetAbility<magnataur_reverse_polarity>();

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.Amount = factory.Item("Minimum Heroes to RP", new Slider(2, 1, 5));
//            this.AutoCombo = factory.Item("Auto Combo *NOT ACTIVE YET*", false);
//            this.SkewerPrefMenu = factory.Item("Insec Type",
//                new StringList(new[] { "Teammate", "Fountain" }, 0));
//            this.SkewerTarget = this.SkewerPrefMenu.GetEnum<SkewerTarget>();
//            this.SkewerStandalone = factory.Item("Only Skewer", new KeyBind(70));

//            this.AutoCombo.Item.Tooltip = "Will start combo automatically if Minimum Heroes count is matched.";
//            this.EmpowerHeroes = factory.Item("Empower Heroes",
//                    new HeroToggler(new Dictionary<string, bool>(), false, true, true));
//            this.EmpowerHeroes.Item.Tooltip = "Will use empower on that heroes whenever they don't have empower on them";
//            this.DebugDrawings = factory.Item("Enable Debug Drawings", false);

//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Orchid/Bloodthorn",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.InvisHeroes = itemMenu.Item("ShadowBlade / SilverEdge",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));

//            this.SkewerPrefMenu.ValueChanged += this.SkewerPrefMenuPropertyChanged;
//        }

//        protected override void OnDeactivate()
//        {
//            base.OnDeactivate();
//            this.SkewerPrefMenu.ValueChanged -= this.SkewerPrefMenuPropertyChanged;
//        }

//        private void SkewerPrefMenuPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            this.SkewerTarget = this.SkewerPrefMenu.GetEnum<SkewerTarget>();
//        }

//        protected override async Task KillStealAsync(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Shockwave.CanBeCasted)
//            {
//                await Task.Delay(125, token);
//                return;
//            }

//            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                x => x.IsAlive
//                     && (x.Team != this.Owner.Team)
//                     && !x.IsIllusion
//                     && this.Shockwave.CanHit(x)
//                     && this.Shockwave.GetDamage(x) > x.Health);

//            if (killstealTarget != null)
//            {
//                if (this.Shockwave.Cast(killstealTarget.Position))
//                {
//                    await this.AwaitKillstealDelay(this.Shockwave.GetCastDelay(killstealTarget), token);
//                }
//            }
//            await Task.Delay(125, token);
//        }
//    }
//}
