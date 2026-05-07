//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Extensions;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Ensage.SDK.Extensions;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_winter_wyvern)]
//    internal class WinterWyvern : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public Ability ArcticBurn { get; private set; }

//        public Ability SplinterBlast { get; private set; }

//        public Ability ColdEmbrace { get; private set; }

//        public Ability WintersCurse { get; private set; }

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
//        public MenuSwitcher UseBlink;
//        public MenuSwitcher HealOnlyInCombo;

//        public MenuHeroToggler HeroestoHeal;
//        public MenuHeroToggler HeroestoUlti;
//        public MenuSlider HeroCountToUlti;
//        public MenuSlider HealThreshold;

//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new WinterWyvernCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.ArcticBurn = Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner, AbilityId.winter_wyvern_arctic_burn);
//            this.SplinterBlast = Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner, AbilityId.winter_wyvern_splinter_blast);
//            this.ColdEmbrace = Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner, AbilityId.winter_wyvern_cold_embrace);
//            this.WintersCurse = Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner, AbilityId.winter_wyvern_winters_curse);

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.HeroestoHeal = factory.Item("Heroes to heal: ",
//                new HeroToggler(new Dictionary<string, bool>(), false, true, true));
//            HeroestoHeal.SetTooltip("Will only use Heal if combo key is pressed.";
//            this.HealThreshold = factory.Item("Heal Threshold", new Slider(50, 0, 100));
//            this.HealOnlyInCombo = factory.Item("Heal only combo active", false);

//            this.HeroestoUlti = factory.Item("Heroes to Ulti: ",
//                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.HeroCountToUlti = factory.Item("Hero count to ult", new Slider(1, 0, 5));
//            this.HeroCountToUlti.SetTooltip("Will only use ulti if target has that many heroes around him";
//            this.MantaHeroes = itemMenu.Item("Manta",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.UseBlink = itemMenu.Item("Use blink?", true);
//        }

//        protected override async Task KillStealAsync(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.SplinterBlast.CanBeCasted())
//            {
//                await Task.Delay(125, token);
//                return;
//            }

//            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                x => x.IsAlive
//                     && (x.Team != this.Owner.Team)
//                     && !x.IsIllusion
//                     && this.SplinterBlast.CanHit(x)
//                     && GetBlastDamage(x) > x.Health);

//            if (killstealTarget != null)
//            {
//                if (this.SplinterBlast.Cast(SplinterBlastUnit(killstealTarget)))
//                {
//                    await this.AwaitKillstealDelay((int)this.SplinterBlast.GetCastDelay(this.Owner as Hero, SplinterBlastUnit(killstealTarget)), token);
//                }
//            }
//            await Task.Delay(125, token);
//        }

//        public int NearHeroesCount(Unit unit)
//        {
//            return unit.GetAlliesInRange<Hero>(500).Count();
//        }

//        public Unit SplinterBlastUnit(Unit unit)
//        {
//            return unit.GetAlliesInRange<Unit>(500).FirstOrDefault();
//        }

//        public float GetBlastDamage(params Unit[] targets)
//        {
//            var totalDamage = 0.0f;

//            var damage = this.SplinterBlast.GetDamage(SplinterBlast.Level - 1);
//            var amplify = this.Owner.GetSpellAmplification();
//            foreach (var target in targets)
//            {
//                var reduction = this.SplinterBlast.GetDamageReduction(target, DamageType.Magical);
//                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
//            }

//            return totalDamage;
//        }

//        public float GetBlastHitTime(Unit target)
//        {
//            var hitTime = 0f;
//            if (this.Owner.Distance2D(target) >= 650)
//            {
//                hitTime += 1f;
//            }

//            if (this.Owner.Distance2D(target) <= 650)
//            {
//                hitTime += this.Owner.Distance2D(target) / 650;
//            }

//            return hitTime;
//        }
//    }
//}
