namespace BAIO.Heroes.Base
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Heroes.Modes.Combo;
    using BAIO.Heroes.Modes.Harass;
    using BAIO.Interfaces;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Creeps;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;
    using Divine.Numerics;
    using Divine.Particle;
    using Divine.Update;

    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_drow_ranger;
    using Ensage.SDK.Inventory.Metadata;

    public enum UltiStatus
    {
        NotSkilled,

        Active,

        Danger,

        Deactivated
    }

    [ExportHero(HeroId.npc_dota_hero_drow_ranger)]
    public class DrowRanger : BaseHero
    {
        public drow_ranger_frost_arrows FrostArrows { get; private set; }
        public drow_ranger_wave_of_silence WaveOfSilence { get; private set; }
        //public drow_ranger_multishot Multishot { get; private set; }
        public Ability Multishot { get; private set; }
        public drow_ranger_marksmanship Marksmanship { get; private set; }

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
        public MenuSwitcher DrawRange { get; private set; }
        public MenuSwitcher MaximizeArrowUpTime { get; private set; }
        public MenuSwitcher DrawUltiPrediction { get; private set; }
        public MenuHeroToggler SilenceHeroes;
        public MenuHeroToggler NullifierHeroes;
        public MenuHeroToggler HexHeroes;

        public Unit LastTarget { get; private set; }
        public float LastTargetHitTime { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public UltiStatus UltiStatus { get; private set; }
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

                if (this.Mjollnir != null)
                {
                    totalMana += this.Mjollnir.Ability.ManaCost;
                }
                if (this.HurricanePike != null)
                {
                    totalMana += this.HurricanePike.Ability.ManaCost;
                }
                if (this.Mom != null)
                {
                    totalMana += this.Mom.Ability.ManaCost;
                }
                if (this.Manta != null)
                {
                    totalMana += this.Manta.Ability.ManaCost;
                }

                return totalMana;
            }
        }

        protected override ComboMode GetComboMode()
        {
            return new DrowRangerCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new DrowHarass(this);
        }

        public bool ShouldUseFrostArrow(Unit target)
        {
            var useArrow = false;
            if ((this.ComboMana <= (this.Owner.Mana - this.FrostArrows.Ability.ManaCost)) &&
                this.FrostArrows.CanBeCasted)
            {
                var closeCreeps = EntityManager.GetEntities<Creep>().Any(x =>
                    x != null && x.IsValid && x.IsVisible && x.IsAlive && x.IsSpawned && x.IsEnemy(this.Owner) &&
                    (x.Distance2D(this.Owner) <= 500.0f));
                useArrow = !this.MaximizeArrowUpTime;

                if (!useArrow)
                {
                    var modifier = target.Modifiers.FirstOrDefault(x => x.Name == this.FrostArrows.TargetModifierName);
                    var duration = 1.5f;
                    var hitTime = this.FrostArrows.GetHitTime(target) / 1000.0f;

                    if (this.LastTarget != target)
                    {
                        this.LastTargetHitTime = 0;
                    }

                    var timeDiff = GameManager.GameTime - this.LastTargetHitTime;
                    var arrivalTime = hitTime - timeDiff;
                    if (modifier == null)
                    {
                        useArrow = (duration + arrivalTime) <= hitTime;
                    }
                    else if (this.MaximizeArrowUpTime)
                    {
                        if (arrivalTime > 0)
                        {
                            useArrow = (modifier.RemainingTime + duration + arrivalTime) <= hitTime;
                        }
                        else
                        {
                            useArrow = modifier.RemainingTime <= hitTime;
                        }
                    }
                }
            }
            return useArrow;
        }

        public async Task UseFrostArrows(Unit target, CancellationToken token = default(CancellationToken))
        {
            this.LastTarget = target;
            this.LastTargetHitTime = GameManager.GameTime;
            this.FrostArrows.Cast(target);
            await Task.Delay(this.FrostArrows.GetCastDelay(target), token);
        }

        private void DrawRangePropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (this.Marksmanship.Ability.Level <= 0)
            {
                return;
            }

            if (e.Value)
            {
                ParticleManager.CreateRangeParticle("marksmanship_range_draw", this.Owner, this.Marksmanship.Radius, Color.Green);
            }
            else
            {
                ParticleManager.DestroyParticle("marksmanship_range_draw");
            }
        }

        private void DrawRange_OnUpdate()
        {
            if (this.Marksmanship.Ability.Level <= 0)
            {
                this.UltiStatus = UltiStatus.NotSkilled;
                return;
            }

            var range = this.Marksmanship.Ability.GetAbilitySpecialData("disable_range");

            var enemies = EntityManager.GetEntities<Hero>()
                .Where(x => x != null && x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion &&
                            this.Owner.IsEnemy(x)).ToList();

            if (enemies.Any(x => x.Position.Distance(this.Owner.Position) < range))
            {
                if (this.DrawRange && this.UltiStatus != UltiStatus.Deactivated)
                {
                    ParticleManager.DestroyParticle("marksmanship_range_draw");
                    ParticleManager.CreateRangeParticle("marksmanship_range_draw", this.Owner, range, Color.Red);
                }

                this.UltiStatus = UltiStatus.Deactivated;
            }
            else if (enemies.Any(x => x.Distance2D(this.Owner) <= (range * 1.5f)))
            {
                if (this.DrawRange && this.UltiStatus != UltiStatus.Danger)
                {
                    ParticleManager.DestroyParticle("marksmanship_range_draw");
                    ParticleManager.CreateRangeParticle("marksmanship_range_draw", this.Owner, range, Color.Orange);
                }

                this.UltiStatus = UltiStatus.Danger;
            }
            else
            {
                if (this.DrawRange && this.UltiStatus != UltiStatus.Active)
                {
                    ParticleManager.DestroyParticle("marksmanship_range_draw");
                    ParticleManager.CreateRangeParticle("marksmanship_range_draw", this.Owner, range, Color.Green);
                }

                this.UltiStatus = UltiStatus.Active;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.FrostArrows = this.Context.AbilityFactory.GetAbility<drow_ranger_frost_arrows>();
            this.WaveOfSilence = this.Context.AbilityFactory.GetAbility<drow_ranger_wave_of_silence>();
            //this.Multishot = this.Context.AbilityFactory.GetAbility<drow_ranger_multishot>();
            this.Multishot = this.Owner.GetAbilityById(AbilityId.drow_ranger_multishot);
            this.Marksmanship = this.Context.AbilityFactory.GetAbility<drow_ranger_marksmanship>();

            this.ProjectileSpeed = this.Owner.ProjectileSpeed();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRange = factory.CreateSwitcher("Draw Ulti range");
            this.DrawRange.ValueChanged += this.DrawRangePropertyChanged;
            this.MaximizeArrowUpTime = factory.CreateSwitcher("Maximize Frost Arrows uptime", false);
            this.MaximizeArrowUpTime.SetTooltip("Useful for mana efficiency in early game.");

            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.HexHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.SilenceHeroes = itemMenu.CreateHeroToggler("Silence Heroes", new());

            UpdateManager.CreateIngameUpdate(100, this.DrawRange_OnUpdate); // updates every 0.1 seconds

        }

        protected override void OnDeactivate()
        {
            this.DrawRange.ValueChanged -= this.DrawRangePropertyChanged;
            UpdateManager.DestroyIngameUpdate(this.DrawRange_OnUpdate);

            base.OnDeactivate();
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                NullifierHeroes.AddValue(heroId, true);
                HexHeroes.AddValue(heroId, true);
                SilenceHeroes.AddValue(heroId, true);
            }
            else
            {
                NullifierHeroes.RemoveValue(heroId);
                HexHeroes.RemoveValue(heroId);
                SilenceHeroes.RemoveValue(heroId);
            }
        }
    }
}
