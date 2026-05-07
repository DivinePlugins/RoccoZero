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
//using Ensage.SDK.Abilities.npc_dota_hero_grimstroke;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.PredictionManager.Collision;
//using Ensage.SDK.Renderer.Particle;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_grimstroke)]
//    internal class Grimstroke : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public grimstroke_dark_artistry StrokeOfFate { get; private set; }

//        public grimstroke_ink_creature PhantomsEmbrace { get; private set; }

//        public grimstroke_spirit_walk InkSwell { get; private set; }

//        public grimstroke_soul_chain Soulbind { get; private set; }

//        public grimstroke_scepter DarkPortrait { get; set; }

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

//        #endregion

//        #region MenuItems
//        public MenuHeroToggler BloodthornOrchidHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler PortraitToggler;
//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new GrimstrokeCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.StrokeOfFate = this.Context.AbilityFactory.GetAbility<grimstroke_dark_artistry>();
//            this.PhantomsEmbrace = this.Context.AbilityFactory.GetAbility<grimstroke_ink_creature>();
//            this.InkSwell = this.Context.AbilityFactory.GetAbility<grimstroke_spirit_walk>();
//            this.Soulbind = this.Context.AbilityFactory.GetAbility<grimstroke_soul_chain>();
//            this.DarkPortrait = null; // todo: fix after core update

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.BloodthornOrchidHeroes = itemMenu.Item("Bloodthorn/Orchid",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));

//            this.PortraitToggler = factory.Item("Portrait Heroes",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//        }

//        protected override void OnDeactivate()
//        {
//            base.OnDeactivate();
//        }
//    }
//}
