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
//using Ensage.SDK.Prediction;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_spectre)]
//    class Spectre : BaseHero
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public Ability SpectralDagger { get; private set; }

//        public Ability Reality { get; private set; }

//        public Ability Haunt { get; private set; }

//        public Ability ShadowStep { get; set; }

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
//        public MenuHeroToggler MantaHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler DiffusalHeroes;

//        public MenuSwitcher KillstealWithUlti { get; private set; }
//        public MenuSlider MinimumUltiDistance { get; private set; }
//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new SpectreCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.SpectralDagger = this.Owner.GetAbilityById(AbilityId.spectre_spectral_dagger);
//            this.Reality = this.Owner.GetAbilityById(AbilityId.spectre_reality);
//            this.Haunt = this.Owner.GetAbilityById(AbilityId.spectre_haunt);
//            this.ShadowStep = null;

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

//            this.KillstealWithUlti = factory.Item("Killsteal with ulti", true);
//            this.MinimumUltiDistance = factory.Item("Minimum Ult Distance", new Slider(2000, 0, 10000));
//            this.MinimumUltiDistance.SetTooltip(
//                "Will only use ulti if distance to the target is bigger than that value";
//        }

//        protected override async Task KillStealAsync(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive || !AbilityExtensions.CanBeCasted(this.SpectralDagger))
//            {
//                await Task.Delay(125, token);
//                return;
//            }

//            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                x => x.IsAlive && x.Team != this.Owner.Team && AbilityExtensions.CanBeCasted(this.SpectralDagger) &&
//                AbilityExtensions.CanHit(this.SpectralDagger, x) &&
//                !x.IsIllusion &&
//                     x.Health < SpectralDaggerDamage(this.SpectralDagger.Level - 1, x));

//            var hauntIllusions = EntityManager.GetEntities<Unit>().Where(x => x.Name == this.Owner.Name && x.IsAlive && x.IsIllusion && !x.IsInvulnerable()
//            && x.IsAlly(this.Owner) && x.HasModifier("modifier_spectre_haunt")).ToList();

//            if (killstealTarget != null)
//            {
//                if (hauntIllusions.Count != 0)
//                {
//                    if (Reality.Cast(killstealTarget.Position))
//                    {
//                        var castDelay = AbilityExtensions.GetCastDelay(this.Reality, this.Owner, killstealTarget);
//                        await this.AwaitKillstealDelay((int)castDelay + 20, token);
//                    }
//                }
//                else if (AbilityExtensions.CanBeCasted(this.Haunt) && KillstealWithUlti && this.Owner.Distance2D(killstealTarget) > this.SpectralDagger.CastRange)
//                {
//                    if (this.Haunt.Cast())
//                    {
//                        var castDelay = AbilityExtensions.GetCastDelay(this.Haunt, this.Owner, killstealTarget);
//                        await this.AwaitKillstealDelay((int)castDelay + 20, token);

//                        this.Reality.Cast(killstealTarget.Position);
//                    }
//                }

//                if (killstealTarget.Distance2D(this.Owner) <= this.SpectralDagger.CastRange)
//                {
//                    if (this.SpectralDagger.Cast(killstealTarget))
//                    {
//                        var castDelay = AbilityExtensions.GetCastDelay(this.SpectralDagger, this.Owner, killstealTarget);
//                        await this.AwaitKillstealDelay((int)castDelay, token);
//                    }
//                }
//            }

//            await Task.Delay(125, token);
//        }

//        private float SpectralDaggerDamage(uint level, Unit target)
//        {
//            return this.SpectralDagger.GetDamage(level) * (1.0f - target.MagicDamageResist);
//        }
//    }
//}