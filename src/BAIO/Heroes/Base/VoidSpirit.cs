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
//using Ensage.SDK.Abilities.npc_dota_hero_void_spirit;
//using Ensage.SDK.Extensions;
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
//    [ExportHero(HeroId.npc_dota_hero_void_spirit)]
//    internal class VoidSpirit : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public void_spirit_aether_remnant Remnant { get; private set; }

//        public void_spirit_dissimilate W { get; private set; }

//        public void_spirit_resonant_pulse E { get; private set; }

//        public void_spirit_astral_step Ulti { get; private set; }

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

//        public MenuHeroToggler AbyssalBladeHeroes;
//        public MenuHeroToggler HexHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler OrchidHeroes;

//        public MenuSlider MinimumBlinkRange;
//        public MenuSlider ResonantPulseCount;
//        public MenuSwitcher RandomlyJump;

//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new VoidSpiritCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.Remnant = this.Context.AbilityFactory.GetAbility<void_spirit_aether_remnant>();
//            this.W = this.Context.AbilityFactory.GetAbility<void_spirit_dissimilate>();
//            this.E = this.Context.AbilityFactory.GetAbility<void_spirit_resonant_pulse>();
//            this.Ulti = this.Context.AbilityFactory.GetAbility<void_spirit_astral_step>();



//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.MinimumBlinkRange = factory.Item("Minimum Blink Range", new Slider(400, 0, 1450));
//            this.ResonantPulseCount = factory.Item("Resonant Pulse Enemy Hero Count", new Slider(1, 1, 5));
//            this.RandomlyJump = factory.Item("Randomly Jump in Dissimilate", true);

//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.HexHeroes = itemMenu.Item("Hex Heroes",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.OrchidHeroes = itemMenu.Item("Orchid/Bloodthorn Heroes",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//        }

//        protected override async Task KillStealAsync(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.E.CanBeCasted)
//            {
//                await Task.Delay(125, token);
//                return;
//            }

//            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                x => x.IsAlive
//                     && (x.Team != this.Owner.Team)
//                     && !x.IsIllusion
//                     && !x.IsMagicImmune()
//                     && this.E.CanHit(x)
//                     && x.Distance2D(this.Owner) <= E.Radius
//                     && this.E.GetDamage(x) > x.Health);

//            if (killstealTarget != null)
//            {
//                if (this.E.Cast())
//                {
//                    await this.AwaitKillstealDelay(this.E.GetCastDelay(killstealTarget), token);
//                }
//            }
//            await Task.Delay(125, token);
//        }
//    }
//}
