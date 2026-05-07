using System;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Extensions;
using BAIO.Core.Handlers;
using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Modifier;
using Divine.Modifier.EventArgs;

using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;

using UnitExtensions = Divine.Extensions.UnitExtensions;

namespace BAIO.Heroes.Base
{
    public enum InsecType
    {
        TeamMate,
        Fountain
    }

    [ExportHero(HeroId.npc_dota_hero_tusk)]
    public class Tusk : BaseHero
    {
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

        public Ability IceShards { get; private set; }

        public Ability Snowball { get; private set; }

        public Ability LaunchSnowball { get; private set; }

        public Ability TagTeam { get; private set; }

        public Ability WalrusPunch { get; private set; }

        public Ability WalrusKick { get; private set; }

        public MenuHoldKey KickCombo;
        public MenuHoldKey EscapeKey;
        public MenuHoldKey CliffKey;
        public MenuSelector InsecPrefMenu { get; private set; }
        public MenuHeroToggler MedallionCrestHeroes;
        public MenuHeroToggler HalberdHeroes;
        public MenuHeroToggler UrnHeroes;
        public MenuHeroToggler VesselHeroes;
        public MenuHeroToggler AtosHeroes;
        public MenuHeroToggler OrchidBloodthornHeroes;

        public TaskHandler AghsHandler { get; private set; }

        public bool InSnowball { get; set; }

        public InsecType InsecType;

        protected override ComboMode GetComboMode()
        {
            return new TuskCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.KickCombo = factory.CreateHoldKey("Kick Combo");
            this.KickCombo.SetTooltip("Will use ulti with available keys on target.");

            this.CliffKey = factory.CreateHoldKey("Shard specialization Key");
            this.CliffKey.SetTooltip("Will use shards to get on cliffs or block enemy");

            this.InsecPrefMenu = factory.CreateSelector("Insec Type", new[] { "TeamMate", "Fountain" });

            this.MedallionCrestHeroes = itemMenu.CreateHeroToggler("Medallion/Solar Crest", new());
            this.HalberdHeroes = itemMenu.CreateHeroToggler("Halberd", new());
            this.UrnHeroes = itemMenu.CreateHeroToggler("Urn", new());
            this.VesselHeroes = itemMenu.CreateHeroToggler("Vessel", new());

            this.AghsHandler = TaskHandler.Run(this.OnUpdate);

            this.InsecPrefMenu.ValueChanged += this.InsecPrefMenuPropertyChanged;
            ModifierManager.ModifierAdded += this.OnSnowballAdded;
            ModifierManager.ModifierRemoved += this.OnSnowballRemoved;

            this.IceShards = this.Owner.GetAbilityById(AbilityId.tusk_ice_shards);
            this.Snowball = this.Owner.GetAbilityById(AbilityId.tusk_snowball);
            this.TagTeam = this.Owner.GetAbilityById(AbilityId.tusk_tag_team);
            this.LaunchSnowball = this.Owner.GetAbilityById(AbilityId.tusk_launch_snowball);
            this.WalrusPunch = this.Owner.GetAbilityById(AbilityId.tusk_walrus_punch);
        }

        protected override void OnDeactivate()
        {
            this.AghsHandler.Cancel();
            this.InsecPrefMenu.ValueChanged -= this.InsecPrefMenuPropertyChanged;
            ModifierManager.ModifierAdded -= this.OnSnowballAdded;
            ModifierManager.ModifierRemoved -= this.OnSnowballRemoved;

            base.OnDeactivate();
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                MedallionCrestHeroes.AddValue(heroId, false);
                HalberdHeroes.AddValue(heroId, true);
                UrnHeroes.AddValue(heroId, true);
                VesselHeroes.AddValue(heroId, true);
            }
            else
            {
                MedallionCrestHeroes.RemoveValue(heroId);
                HalberdHeroes.RemoveValue(heroId);
                UrnHeroes.RemoveValue(heroId);
                VesselHeroes.RemoveValue(heroId);
            }
        }

        private void InsecPrefMenuPropertyChanged(MenuSelector selector, SelectorEventArgs e)
        {
            this.InsecType = e.NewValue switch
            {
                "TeamMate" => InsecType.TeamMate,
                "Fountain" => InsecType.Fountain,
                _ => throw new NotImplementedException(),
            };
        }

        private void OnSnowballAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if ((sender is Hero) && this.Owner == sender &&
                (modifier.Name == "modifier_tusk_snowball_movement"))
            {
                //LogManager.Debug($"OnUlti");
                this.InSnowball = true;
            }
        }

        private void OnSnowballRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if (this.Owner == sender && (modifier.Name == "modifier_tusk_snowball_movement"))
            {
                this.InSnowball = false;
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive)
            {
                await Task.Delay(250, token);
                return;
            }

            // setting walrus kick on OnUpdate so that it can return null if doesn't have aghanim
            // and can properly set skill if has aghanim.
            if (UnitExtensions.HasAghanimsScepter(this.Owner))
            {
                this.WalrusKick = this.Owner.GetAbilityById(AbilityId.tusk_walrus_kick);
            }
            else
            {
                WalrusKick = null;
            }

            if (this.CliffKey)
            {
                var pos = this.Owner.Position;
                if (this.IceShards != null && this.IceShards.CanBeCasted() && !this.Owner.IsChanneling())
                {
                    if (this.Owner.IsMoving)
                    {
                        this.IceShards.Cast(this.Owner.InFront(((float)this.Owner.MovementSpeed / 18) + (GameManager.Ping / 6))); // magic numbers
                        await Task.Delay((int)this.IceShards.GetCastDelay(this.Owner, this.Owner, true), token);
                    }
                    else
                    {
                        this.IceShards.Cast(this.Owner.InFront(5));
                        await Task.Delay((int)this.IceShards.GetCastDelay(this.Owner, this.Owner, true), token);
                    }
                }
                await Task.Delay(100, token);
            }
        }
    }
}