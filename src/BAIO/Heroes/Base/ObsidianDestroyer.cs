namespace BAIO.Heroes.Base
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Handlers;
    using BAIO.Heroes.Modes.Combo;
    using BAIO.Heroes.Modes.Harass;
    using BAIO.Interfaces;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Menu.Items;
    using Divine.Zero.Log;

    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_obsidian_destroyer;
    using Ensage.SDK.Inventory.Metadata;

    [ExportHero(HeroId.npc_dota_hero_obsidian_destroyer)]
    public class ObsidianDestroyer : BaseHero
    {
        public obsidian_destroyer_arcane_orb ArcaneOrb { get; private set; }
        public obsidian_destroyer_astral_imprisonment Astral { get; private set; }
        public obsidian_destroyer_equilibrium Equilibrium { get; private set; }
        public obsidian_destroyer_sanity_eclipse SanityEclipse { get; private set; }

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

        [ItemBinding]
        public item_hurricane_pike HurricanePike { get; private set; }

        public MenuHeroToggler NullifierHeroes;
        public MenuHeroToggler HexHeroes;
        public MenuSlider UseBlinkPrediction { get; set; }
        public MenuSlider MinimumTargetToUlti { get; set; }
        public MenuSlider HurricanePercentage { get; set; }
        public MenuSwitcher AutoEqu { get; set; }
        public MenuSlider EquThreshold { get; set; }

        public float ComboMana
        {
            get
            {
                var totalMana = 0.0f;

                if (this.ShadowBlade != null)
                {
                    totalMana += this.ShadowBlade.Ability.ManaCost;
                }
                else if (this.SilverEdge != null)
                {
                    totalMana += this.SilverEdge.Ability.ManaCost;
                }

                if (this.Orchid != null)
                {
                    totalMana += this.Orchid.Ability.ManaCost;
                }
                else if (this.BloodThorn != null)
                {
                    totalMana += this.BloodThorn.Ability.ManaCost;
                }

                if (this.VeilOfDiscord != null)
                {
                    totalMana += this.VeilOfDiscord.Ability.ManaCost;
                }
                if (this.RodOfAtos != null)
                {
                    totalMana += this.RodOfAtos.Ability.ManaCost;
                }
                if (this.HurricanePike != null)
                {
                    totalMana += this.HurricanePike.Ability.ManaCost;
                }
                if (this.Sheepstick != null)
                {
                    totalMana += this.Sheepstick.Ability.ManaCost;
                }
                if (this.ShivasGuard != null)
                {
                    totalMana += this.ShivasGuard.Ability.ManaCost;
                }

                return totalMana;
            }
        }

        protected TaskHandler EquilibriumCastHandler { get; private set; }

        protected override ComboMode GetComboMode()
        {
            return new ObsidianDestroyerCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new ObsidianDestroyerHarass(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.ArcaneOrb = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_arcane_orb>();
            this.Astral = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_astral_imprisonment>();
            this.Equilibrium = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_equilibrium>();
            this.SanityEclipse = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_sanity_eclipse>();

            this.EquilibriumCastHandler = TaskHandler.Run(this.EquilibriumCaster);

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.UseBlinkPrediction = factory.CreateSlider("Blink Prediction", 200, 0, 600);
            this.UseBlinkPrediction.SetTooltip("Will blink to set distance. Set to 0 if you want to disable it.");

            this.MinimumTargetToUlti = factory.CreateSlider("Ulti Target Count", 1, 1, 5);
            this.MinimumTargetToUlti.SetTooltip("Minimum required enemy heroes to cast ulti. Atleast 1 of them should die too.");

            this.EquThreshold = factory.CreateSlider("Equilibrium Threshold", 75, 1, 100);
            this.EquThreshold.SetTooltip("Will use Equilibrium only if % mana is lower than value.");

            this.AutoEqu = factory.CreateSwitcher("Auto Equilibrium");

            this.HurricanePercentage = itemMenu.CreateSlider("Hurricane Usage Percent", 20, 0, 100);

            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.HexHeroes = itemMenu.CreateHeroToggler("Hex", new());
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this.EquilibriumCastHandler.Cancel();
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                NullifierHeroes.AddValue(heroId, true);
                HexHeroes.AddValue(heroId, true);
            }
            else
            {
                NullifierHeroes.RemoveValue(heroId);
                HexHeroes.RemoveValue(heroId);
            }
        }

        private async Task EquilibriumCaster(CancellationToken token)
        {
            var astralled = EntityManager.GetEntities<Unit>().Where(
                x => x.Team != this.Owner.Team
                     && !x.IsIllusion
                     && x.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                     && x.GetModifierByName("modifier_obsidian_destroyer_astral_imprisonment_prison").RemainingTime <= 0.5f).ToList();
            try
            {
                if (astralled.Count() >= 1 && this.AutoEqu)
                {
                    this.Equilibrium.Cast();
                    await Task.Delay(this.Equilibrium.GetCastDelay(), token);
                }
                else
                {
                    await Task.Delay(150, token);
                }
            }
            catch (TaskCanceledException)
            {
                //
            }
            catch (Exception e)
            {
                LogManager.Debug($"{e}");
            }
        }

        public bool ShouldUseArcaneOrb(Unit target)
        {
            var useArcaneOrb = false;
            if ((this.ComboMana <= (this.Owner.Mana - this.ArcaneOrb.Ability.ManaCost)) &&
                this.ArcaneOrb.CanBeCasted && this.Owner.CanAttack() && this.Context.Orbwalker.CanAttack(target))
            {
                useArcaneOrb = true;
            }
            return useArcaneOrb;
        }

        public async Task UseArcaneOrb(Unit target, CancellationToken token = default(CancellationToken))
        {
            this.ArcaneOrb.Cast(target);
            await Task.Delay(this.ArcaneOrb.GetCastDelay(target), token);
        }



        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Astral.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.Astral.CanHit(x)
                     && this.Astral.GetDamage(x) - (x.HealthRegeneration * 5) > x.Health);

            if (killstealTarget != null)
            {
                if (this.Astral.Cast(killstealTarget))
                {
                    await this.AwaitKillstealDelay(this.Astral.GetCastDelay(killstealTarget), token);
                }
            }
            await Task.Delay(125, token);
        }
    }
}
