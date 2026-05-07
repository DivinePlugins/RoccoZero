using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Extensions;
using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.Items;

using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;

namespace BAIO.Heroes.Base
{
    [ExportHero(HeroId.npc_dota_hero_pugna)]
    internal class Pugna : BaseHero
    {
        public Ability Decrepify { get; set; }

        public Ability Ward { get; set; }

        public Ability Drain { get; set; }

        public Ability Blast { get; set; }

        public bool IsHealing { get; set; }

        public Unit HealTarget { get; set; }

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
        public item_dagon Dagon1 { get; set; }

        [ItemBinding]
        public item_dagon_2 Dagon2 { get; set; }

        [ItemBinding]
        public item_dagon_3 Dagon3 { get; set; }

        [ItemBinding]
        public item_dagon_4 Dagon4 { get; set; }

        [ItemBinding]
        public item_dagon_5 Dagon5 { get; set; }

        public Dagon Dagon => Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;

        public MenuSlider UseBlinkPrediction { get; set; }

        public MenuHeroToggler HealTargetHeroes { get; set; }

        public MenuSlider DrainHP { get; set; }

        public MenuSlider WardTargets { get; set; }

        public MenuSlider SelfHPDrain { get; set; }

        public MenuSlider PostDrainHP { get; set; }

        public MenuSlider HealAllyTo { get; set; }

        protected override ComboMode GetComboMode()
        {
            return new PugnaCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Decrepify = this.Owner.GetAbilityById(AbilityId.pugna_decrepify);
            this.Blast = this.Owner.GetAbilityById(AbilityId.pugna_nether_blast);
            this.Ward = this.Owner.GetAbilityById(AbilityId.pugna_nether_ward);
            this.Drain = this.Owner.GetAbilityById(AbilityId.pugna_life_drain);


            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.UseBlinkPrediction = factory.CreateSlider("Blink Prediction", 200, 0, 600);
            this.UseBlinkPrediction.SetTooltip("Will blink to set distance. Set to 0 if you want to disable it.");
            this.DrainHP = factory.CreateSlider("Heal ally HP", 30, 0, 100);
            this.DrainHP.SetTooltip("Allies HP to begin healing");
            this.SelfHPDrain = factory.CreateSlider("Min HP to Heal", 60, 0, 100);
            this.SelfHPDrain.SetTooltip("HP threshold to start healing");
            this.PostDrainHP = factory.CreateSlider("Post drain HP", 30, 0, 100);
            this.PostDrainHP.SetTooltip("HP threshold to stop healing. (this value must be higher than post drain HP)");
            this.HealAllyTo = factory.CreateSlider("Post drain HP for ally", 100, 0, 100);
            this.HealAllyTo.SetTooltip("Heal ally to this hp (this value must be higher than heal ally HP)");
            this.WardTargets = factory.CreateSlider("Targets for ward", 0, 1, 5);
            this.WardTargets.SetTooltip("Targets in range of ward for usage");
            this.HealTargetHeroes = factory.CreateHeroToggler("Heal Targets", new());
        }

        private protected override void OnMenuAllyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                HealTargetHeroes.AddValue(heroId, false);
            }
            else
            {
                HealTargetHeroes.RemoveValue(heroId);
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Blast.CanBeCasted())
            {
                await Task.Delay(125, token);
                return;
            }

            var damageBlast = Blast.GetAbilitySpecialData("blast_damage");
            damageBlast *= this.Owner.GetSpellAmplification();

            bool comboMana = Blast.AbilityData.GetManaCost(Blast.Level - 1) + Decrepify.AbilityData.GetManaCost(Decrepify.Level - 1) <
                             Owner.Mana;

            var decrepifyKillable =
                EntityManager.GetEntities<Hero>().FirstOrDefault(
                    x =>
                        x.IsAlive && x.Team != this.Owner.Team && !x.IsIllusion
                        && x.Health < damageBlast * (1 - x.MagicalDamageResistance)
                        && Blast != null && Blast.IsValid && x.Distance2D(this.Owner) <= 900
                        && Decrepify.CanBeCasted(x) && Blast.CanBeCasted(x)
                        && !x.IsMagicImmune() && comboMana);

            var blastKillable =
                EntityManager.GetEntities<Hero>().FirstOrDefault(
                    x =>
                        x.IsAlive && x.Team != this.Owner.Team && !x.IsIllusion
                        && x.Health < damageBlast * (1 - x.MagicalDamageResistance)
                        && Blast.CanBeCasted(x) && !x.IsMagicImmune() && Blast.CanHit(x)
                        && Owner.Distance2D(x.Position) <= 800);

            if (!this.Owner.IsChanneling())
            {

                if (decrepifyKillable != null)
                {
                    if (Decrepify.Cast(decrepifyKillable))
                    {
                        await this.AwaitKillstealDelay((int)this.Decrepify.GetCastDelay(this.Owner, decrepifyKillable, true), token); // decrepify
                    }

                    if (Blast.CanHit(decrepifyKillable))
                    {
                        if (Blast.Cast(decrepifyKillable))
                        {
                            await this.AwaitKillstealDelay((int)this.Blast.GetCastDelay(this.Owner, decrepifyKillable, true), token);
                        }
                    }
                }

                if (blastKillable != null)
                {
                    if (Blast.Cast(blastKillable.Position))
                    {
                        await this.AwaitKillstealDelay((int)this.Blast.GetCastDelay(this.Owner, blastKillable, true), token);
                    }
                }

            }
        }
    }
}
