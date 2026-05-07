using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Game;
using Divine.Menu.Items;
using Divine.Prediction;

using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_ember_spirit;
using Ensage.SDK.Inventory.Metadata;

namespace BAIO.Heroes.Base
{
    [ExportHero(HeroId.npc_dota_hero_ember_spirit)]
    internal class EmberSpirit : BaseHero
    {

        public ember_spirit_searing_chains SearingChains { get; private set; }

        public ember_spirit_sleight_of_fist Fist { get; private set; }

        public ember_spirit_flame_guard FlameGuard { get; private set; }

        public ember_spirit_fire_remnant FireRemnant { get; private set; }

        public ember_spirit_activate_fire_remnant ActiveFireRemnant { get; private set; }

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

        public MenuHeroToggler AbyssalBladeHeroes;
        public MenuHeroToggler MantaHeroes;
        public MenuHeroToggler NullifierHeroes;
        public MenuHeroToggler HexHeroes;

        public MenuHoldKey FistandChains;
        public MenuSwitcher AutoChain;
        public MenuSlider LeaveSpirits;

        protected override ComboMode GetComboMode()
        {
            return new EmberSpiritCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.SearingChains = this.Context.AbilityFactory.GetAbility<ember_spirit_searing_chains>();
            this.Fist = this.Context.AbilityFactory.GetAbility<ember_spirit_sleight_of_fist>();
            this.FlameGuard = this.Context.AbilityFactory.GetAbility<ember_spirit_flame_guard>();
            this.FireRemnant = this.Context.AbilityFactory.GetAbility<ember_spirit_fire_remnant>();
            this.ActiveFireRemnant = this.Context.AbilityFactory.GetAbility<ember_spirit_activate_fire_remnant>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.FistandChains = factory.CreateHoldKey("Fist N Chains Combo");
            this.FistandChains.SetTooltip("Will only use Fist and Chains.");

            this.AutoChain = factory.CreateSwitcher("Auto Chain on W use");

            this.MantaHeroes = itemMenu.CreateHeroToggler("Manta", new());
            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.AbyssalBladeHeroes = itemMenu.CreateHeroToggler("Abyssal Blade", new());
            this.HexHeroes = itemMenu.CreateHeroToggler("Hex", new());

            this.LeaveSpirits = factory.CreateSlider("Leave X amount of spirits unused", 1, 0, 7);

        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                MantaHeroes.AddValue(heroId, true);
                NullifierHeroes.AddValue(heroId, true);
                AbyssalBladeHeroes.AddValue(heroId, true);
                HexHeroes.AddValue(heroId, true);
            }
            else
            {
                MantaHeroes.RemoveValue(heroId);
                NullifierHeroes.RemoveValue(heroId);
                AbyssalBladeHeroes.RemoveValue(heroId);
                HexHeroes.RemoveValue(heroId);
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Fist.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.Fist.CanHit(x) && this.Fist.GetDamage(x) >= x.Health);

            if (killstealTarget != null)
            {
                var output = PredictionManager.GetPrediction(FistPredictionInput(killstealTarget));
                if (this.Fist.Cast(output.CastPosition))
                {
                    await this.AwaitKillstealDelay(25, token);
                }
            }
            await Task.Delay(125, token);
        }

        public PredictionInput FistPredictionInput(Unit target)
        {
            return new PredictionInput(this.Owner, target, 0.001f, float.MaxValue, this.Fist.CastRange, this.Fist.Radius, PredictionSkillshotType.SkillshotCircle);
        }

    }
}
