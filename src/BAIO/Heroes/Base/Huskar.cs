namespace BAIO.Heroes.Base
{
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Heroes.Modes.Combo;
    using BAIO.Heroes.Modes.Harass;
    using BAIO.Interfaces;
    using BAIO.Modes;

    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Menu.Items;
    using Divine.Numerics;
    using Divine.Order;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders.Components;
    using Divine.Particle;
    using Divine.Update;
    using Divine.Zero.Log;

    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_huskar;
    using Ensage.SDK.Inventory.Metadata;

    [ExportHero(HeroId.npc_dota_hero_huskar)]
    public class Huskar : BaseHero
    {
        public huskar_inner_fire InnerFire { get; private set; }

        public huskar_burning_spear FireSpear { get; private set; }

        public huskar_life_break Ulti { get; private set; }

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
        public item_armlet Armlet { get; private set; }

        [ItemBinding]
        public item_hurricane_pike HurricanePike { get; private set; }

        public MenuSwitcher DrawRangeItem { get; private set; }
        public MenuSlider SatanicPercent { get; private set; }
        public MenuSlider FireSpearThreshold { get; private set; }
        public MenuSlider InnerFireThreshold { get; private set; }
        public MenuSwitcher InformationTab { get; private set; }

        public MenuHeroToggler HalberdHeroes;
        public MenuHeroToggler MedallionHeroes;
        public MenuHeroToggler BloodthornHeroes;
        public MenuHeroToggler HurricaneHeroes;

        public float UltiCastRange
        {
            get
            {
                return this.Ulti.CastRange;
            }
        }

        public bool IsArmletEnabled { get; set; }

        public bool IsFireSpearGucci(Unit target)
        {
            bool useSpears = this.Owner.HealthPercent() >= FireSpearThreshold.Value / 100f;

            return useSpears;
        }

        public async Task UseFireSpear(Unit target, CancellationToken token = default(CancellationToken))
        {
            this.FireSpear.Cast(target);
            await Task.Delay(this.FireSpear.GetCastDelay(target), token);
        }

        protected override ComboMode GetComboMode()
        {
            return new HuskarCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new HuskarHarass(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.InnerFire = this.Context.AbilityFactory.GetAbility<huskar_inner_fire>();
            this.FireSpear = this.Context.AbilityFactory.GetAbility<huskar_burning_spear>();
            this.Ulti = this.Context.AbilityFactory.GetAbility<huskar_life_break>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRangeItem = factory.CreateSwitcher("Draw Ulti Range");
            this.SatanicPercent = factory.CreateSlider("Satanic Percent", 20, 1, 100);
            this.FireSpearThreshold = factory.CreateSlider("Stop using Burning Spear", 10, 0, 100);
            this.FireSpearThreshold.SetTooltip("Will stop attacking with Burning Spear if HP is lower than that percent");
            this.InnerFireThreshold = factory.CreateSlider("Inner Fire Threshold", 0, 20, 100);
            this.InnerFireThreshold.SetTooltip("Set this to 100 if you want it to be always used");

            this.HalberdHeroes = itemMenu.CreateHeroToggler("Halberd", new());
            this.MedallionHeroes = itemMenu.CreateHeroToggler("Medallion/SolarCrest", new());
            this.BloodthornHeroes = itemMenu.CreateHeroToggler("Bloodthorn", new());
            this.InformationTab = itemMenu.CreateSwitcher("Hover over for info");
            this.InformationTab.SetTooltip("Hurricane pike will be used if guaranteed kill. Select heroes you want to use when they are in attack range.");
            this.HurricaneHeroes = itemMenu.CreateHeroToggler("HurricanePike", new());

            if (this.DrawRangeItem)
            {
                UpdateManager.CreateIngameUpdate(25, this.UpdateDrawRange);
            }

            OrderManager.OrderAdding += this.OnExecuteOrder;
        }

        protected override void OnDeactivate()
        {
            UpdateManager.DestroyIngameUpdate(this.UpdateDrawRange);
            OrderManager.OrderAdding -= this.OnExecuteOrder;

            base.OnDeactivate();
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                HalberdHeroes.AddValue(heroId, true);
                MedallionHeroes.AddValue(heroId, true);
                BloodthornHeroes.AddValue(heroId, true);
                HurricaneHeroes.AddValue(heroId, false);
            }
            else
            {
                HalberdHeroes.RemoveValue(heroId);
                MedallionHeroes.RemoveValue(heroId);
                BloodthornHeroes.RemoveValue(heroId);
                HurricaneHeroes.RemoveValue(heroId);
            }
        }

        private void UpdateDrawRange()
        {
            if (this.DrawRangeItem && this.Ulti.Ability.Level > 0)
            {
                ParticleManager.CreateRangeParticle("huskarUltiRange", this.Owner, this.UltiCastRange, Color.Red);
            }
            else
            {
                ParticleManager.DestroyParticle("huskarUltiRange");
            }
        }

        private void OnExecuteOrder(OrderAddingEventArgs e)
        {
            var order = e.Order;
            if ((this.Armlet != null) && !e.IsCustom && (order.Type == OrderType.CastToggle) &&
                (order.Ability == this.Armlet.Ability))
            {
                LogManager.Debug($"Armlet activated by you");
                this.IsArmletEnabled = !this.Armlet.Enabled;
            }
        }
    }
}