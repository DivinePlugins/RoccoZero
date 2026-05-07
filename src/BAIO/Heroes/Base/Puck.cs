//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Aggregation;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_antimage;
//using Ensage.SDK.Abilities.npc_dota_hero_puck;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_puck)]
//    internal class Puck : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public puck_illusory_orb Orb { get; private set; }

//        public puck_ethereal_jaunt Jaunt { get; private set; }

//        public puck_waning_rift Rift { get; private set; }

//        public puck_dream_coil DreamCoil { get; private set; }

//        public puck_phase_shift PhaseShift { get; private set; }

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

//        [ItemBinding]
//        public item_shivas_guard Shiva { get; private set; }

//        [ItemBinding]
//        public item_dagon Dagon1 { get; set; }

//        [ItemBinding]
//        public item_dagon_2 Dagon2 { get; set; }

//        [ItemBinding]
//        public item_dagon_3 Dagon3 { get; set; }

//        [ItemBinding]
//        public item_dagon_4 Dagon4 { get; set; }

//        [ItemBinding]
//        public item_dagon_5 Dagon5 { get; set; }

//        public Dagon Dagon => Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;

//        [ItemBinding]
//        public item_ethereal_blade EtherealBlade { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler HexHeroes;
//        public MenuHeroToggler DagonHeroes;
//        public MenuSwitcher SniperMode;
//        public MenuSwitcher ComboRadiusMenu;
//        public MenuSwitcher DrawUltiPosition;
//        public MenuSlider AmountMenu;
//        public MenuHoldKey PuckEscape;
//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new PuckCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.Orb = this.Context.AbilityFactory.GetAbility<puck_illusory_orb>();
//            this.Jaunt = this.Context.AbilityFactory.GetAbility<puck_ethereal_jaunt>();
//            this.Rift = this.Context.AbilityFactory.GetAbility<puck_waning_rift>();
//            this.DreamCoil = this.Context.AbilityFactory.GetAbility<puck_dream_coil>();
//            this.PhaseShift = this.Context.AbilityFactory.GetAbility<puck_phase_shift>();

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;
//            var drawingMenu = factory.Menu("Draws");

//            this.ComboRadiusMenu = drawingMenu.Item("Dagger + Ulti Cast Range", true);
//            this.DrawUltiPosition = drawingMenu.Item("Draw Ulti Position", true);

//            this.PuckEscape = factory.Item("Escape", new KeyBind(70));

//            this.SniperMode = factory.Item("Sniper Mode", false);
//            this.SniperMode.SetTooltip("It will use everything and runaway to orb";
//            this.AmountMenu = factory.Item("Amount", new Slider(2, 1, 5));
//            this.HexHeroes = itemMenu.Item("Hex",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.DagonHeroes = itemMenu.Item("Dagon",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//        }
//    }
//}
