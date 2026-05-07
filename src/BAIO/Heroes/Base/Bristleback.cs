using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Game;
using Divine.Menu.Items;

using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_bristleback;
using Ensage.SDK.Inventory.Metadata;

namespace BAIO.Heroes.Base
{
    [ExportHero(HeroId.npc_dota_hero_bristleback)]
    internal class Bristleback : BaseHero
    {
        public bristleback_quill_spray QuillSpray { get; private set; }

        public bristleback_viscous_nasal_goo NasalGoo { get; private set; }

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

        protected override ComboMode GetComboMode()
        {
            return new BristlebackCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.QuillSpray = this.Context.AbilityFactory.GetAbility<bristleback_quill_spray>();
            this.NasalGoo = this.Context.AbilityFactory.GetAbility<bristleback_viscous_nasal_goo>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            /* this.TurnTheOtherCheek = factory.Item("Turn The Other Cheek", new KeyBind(70));
             this.TurnTheOtherCheek.SetTooltip("Will try to turn back to enemies."; */

            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.AbyssalBladeHeroes = itemMenu.CreateHeroToggler("Abyssal Blade", new());
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                NullifierHeroes.AddValue(heroId, true);
                AbyssalBladeHeroes.AddValue(heroId, true);
            }
            else
            {
                NullifierHeroes.RemoveValue(heroId);
                AbyssalBladeHeroes.RemoveValue(heroId);
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.QuillSpray.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.QuillSpray.CanHit(x)
                     && this.QuillSpray.GetDamage(x) > x.Health);

            if (killstealTarget != null)
            {
                if (this.QuillSpray.Cast())
                {
                    await this.AwaitKillstealDelay(this.QuillSpray.GetCastDelay(), token);
                }
            }
            await Task.Delay(125, token);
        }
    }
}
