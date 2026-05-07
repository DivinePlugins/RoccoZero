namespace BAIO.Heroes.Base
{
    using System.Linq;

    using BAIO.Core.Extensions;
    using BAIO.Heroes.Modes.Combo;
    using BAIO.Interfaces;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;
    using Divine.Numerics;
    using Divine.Particle;
    using Divine.Update;

    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_dark_willow;
    using Ensage.SDK.Inventory.Metadata;

    public enum ShadowRealmStatus
    {
        NotSkilled,

        Active,

        Deactivated
    }

    [ExportHero(HeroId.npc_dota_hero_dark_willow)]
    public class DarkWillow : BaseHero
    {
        public dark_willow_bedlam Bedlam { get; private set; }
        public dark_willow_bramble_maze BrambleMaze { get; private set; }
        public dark_willow_cursed_crown CursedCrown { get; private set; }
        public dark_willow_shadow_realm ShadowRealm { get; private set; }
        public dark_willow_terrorize Terrorize { get; private set; }

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
        public MenuHeroToggler NullifierHeroes;
        public MenuHeroToggler HexHeroes;

        public ShadowRealmStatus RealmStatus { get; private set; }

        protected override ComboMode GetComboMode()
        {
            return new DarkWillowCombo(this);
        }

        public bool ShouldHit(Unit target)
        {
            var hitEnemy = false;
            if (this.Owner.HasAghanimsScepter() || !this.Owner.HasModifier(this.ShadowRealm.ModifierName))
            {
                hitEnemy = true;
            }
            else
            {
                if (this.Owner.HasModifier(this.ShadowRealm.ModifierName) && target.Distance2D(this.Owner) + 150 <= this.ShadowRealm.CastRange &&
                    this.Owner.FindModifier(this.ShadowRealm.ModifierName).ElapsedTime >= this.ShadowRealm.Ability.GetAbilitySpecialData("max_damage_duration"))
                {
                    hitEnemy = true;
                }
            }
            return hitEnemy;
        }


        private void DrawRangePropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (this.ShadowRealm.Ability.Level <= 0)
            {
                return;
            }

            if (e.Value)
            {
                ParticleManager.CreateRangeParticle("shadow_realm_range_draw", this.Owner, this.ShadowRealm.CastRange, Color.Green);
            }
            else
            {
                ParticleManager.DestroyParticle("shadow_realm_range_draw");
            }
        }

        private void DrawRange_OnUpdate()
        {
            if (this.ShadowRealm.Ability.Level <= 0)
            {
                this.RealmStatus = ShadowRealmStatus.NotSkilled;
                return;
            }

            var range = this.ShadowRealm.CastRange;

            var enemies = EntityManager.GetEntities<Hero>()
                .Where(x => x != null && x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion &&
                            this.Owner.IsEnemy(x)).ToList();

            if (!this.Owner.HasModifier(this.ShadowRealm.ModifierName))
            {
                if (this.DrawRange && this.RealmStatus != ShadowRealmStatus.Deactivated)
                {
                    ParticleManager.DestroyParticle("shadow_realm_range_draw");
                    ParticleManager.CreateRangeParticle("shadow_realm_range_draw", this.Owner, range, Color.Red);
                }

                this.RealmStatus = ShadowRealmStatus.Deactivated;
            }
            else
            {
                if (this.DrawRange && this.RealmStatus != ShadowRealmStatus.Active)
                {
                    ParticleManager.DestroyParticle("shadow_realm_range_draw");
                    ParticleManager.CreateRangeParticle("shadow_realm_range_draw", this.Owner, range, Color.Green);
                }

                this.RealmStatus = ShadowRealmStatus.Active;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Bedlam = this.Context.AbilityFactory.GetAbility<dark_willow_bedlam>();
            this.Terrorize = this.Context.AbilityFactory.GetAbility<dark_willow_terrorize>();
            this.ShadowRealm = this.Context.AbilityFactory.GetAbility<dark_willow_shadow_realm>();
            this.CursedCrown = this.Context.AbilityFactory.GetAbility<dark_willow_cursed_crown>();
            this.BrambleMaze = this.Context.AbilityFactory.GetAbility<dark_willow_bramble_maze>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRange = factory.CreateSwitcher("Draw Shadow Realm Range");
            this.DrawRange.ValueChanged += this.DrawRangePropertyChanged;

            this.NullifierHeroes = itemMenu.CreateHeroToggler("Nullifier", new());
            this.HexHeroes = itemMenu.CreateHeroToggler("VesHexsel", new());

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
            }
            else
            {
                NullifierHeroes.RemoveValue(heroId);
                HexHeroes.RemoveValue(heroId);
            }
        }
    }
}
