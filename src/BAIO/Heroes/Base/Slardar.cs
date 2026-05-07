//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Extensions;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_slardar;
//using Ensage.SDK.Abilities.npc_dota_hero_sven;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.Renderer.Particle;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_slardar)]
//    public class Slardar : BaseHero
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

//        #region Abilities

//        public slardar_slithereen_crush Stun { get; private set; }

//        public slardar_sprint Speed { get; private set; }

//        public slardar_amplify_damage Ulti { get; set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler BkbHeroes;
//        public MenuHeroToggler BtOrchidHeroes;
//        public MenuHeroToggler HalberdHeroes;
//        public MenuHeroToggler InvisHeroes;
//        public MenuSwitcher UseUltiBefore;
//        public MenuSwitcher ForceStaffPush;
//        #endregion

//        public bool UserUseStun;

//        protected override ComboMode GetComboMode()
//        {
//            return new SlardarCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();
//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.UseUltiBefore = factory.Item("Use Ulti and Sprint before blink", false);
//            this.ForceStaffPush = factory.Item("Push with force staff to stun", true);

//            this.BkbHeroes = itemMenu.Item("Black King Bar",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.BtOrchidHeroes = itemMenu.Item("Bloodthorn / Orchid",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.HalberdHeroes = itemMenu.Item("Halberd",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.InvisHeroes = itemMenu.Item("ShadowBlade / SilverEdge",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));


//            this.Stun = this.Context.AbilityFactory.GetAbility<slardar_slithereen_crush>();
//            this.Speed = this.Context.AbilityFactory.GetAbility<slardar_sprint>();
//            this.Ulti = this.Context.AbilityFactory.GetAbility<slardar_amplify_damage>();

//            Player.OnExecuteOrder += this.OnExecuteOrder;
//        }

//        protected override void OnDeactivate()
//        {
//            base.OnDeactivate();
//            Player.OnExecuteOrder -= this.OnExecuteOrder;
//        }

//        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
//        {
//            if (args.IsPlayerInput && (args.OrderId == OrderId.Ability) && (args.Ability == this.Stun.Ability))
//            {
//                LogManager.Debug($"user stun");
//                this.UserUseStun = !this.Stun.Ability.IsInAbilityPhase;
//            }
//        }
//    }
//}