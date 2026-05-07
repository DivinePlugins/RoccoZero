using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Entity.EventArgs;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.Items;

using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_antimage;
using Ensage.SDK.Inventory.Metadata;

namespace BAIO.Heroes.Base
{
    [ExportHero(HeroId.npc_dota_hero_antimage)]
    internal class AntiMage : BaseHero
    {
        public antimage_blink Blink { get; private set; }

        public antimage_mana_void ManaVoid { get; private set; }

        // public antimage_counterspell CounterSpell { get; private set; }

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

        public MenuSlider MinimumBlinkRange;
        public MenuHeroToggler BlinkHeroes;

        protected override ComboMode GetComboMode()
        {
            return new AntiMageCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Blink = this.Context.AbilityFactory.GetAbility<antimage_blink>();
            this.ManaVoid = this.Context.AbilityFactory.GetAbility<antimage_mana_void>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.MinimumBlinkRange = factory.CreateSlider("Minimum Blink Range", 400, 0, 1450);
            this.BlinkHeroes = factory.CreateHeroToggler("Blink Heroes", new());

            this.MantaHeroes = itemMenu.CreateHeroToggler("Manta", new());
            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.AbyssalBladeHeroes = itemMenu.CreateHeroToggler("Abyssal Blade", new());
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                BlinkHeroes.AddValue(heroId, true);
                MantaHeroes.AddValue(heroId, true);
                NullifierHeroes.AddValue(heroId, true);
                AbyssalBladeHeroes.AddValue(heroId, true);
            }
            else
            {
                BlinkHeroes.RemoveValue(heroId);
                MantaHeroes.RemoveValue(heroId);
                NullifierHeroes.RemoveValue(heroId);
                AbyssalBladeHeroes.RemoveValue(heroId);
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.ManaVoid.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && !x.IsMagicImmune()
                     && this.ManaVoid.CanHit(x)
                     && this.ManaVoid.GetDamage(x) > x.Health);

            if (killstealTarget != null)
            {
                if (this.ManaVoid.Cast(killstealTarget))
                {
                    await this.AwaitKillstealDelay(this.ManaVoid.GetCastDelay(killstealTarget), token);
                }
            }
            await Task.Delay(125, token);
        }
    }
}
