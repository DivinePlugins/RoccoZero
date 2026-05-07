using System.Collections.Generic;
using System.Linq;

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
using Divine.Input;
using Divine.Menu.Items;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Prediction;
using Divine.Renderer;
using Divine.Update;

using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;

using UnitExtensions = Divine.Extensions.UnitExtensions;

namespace BAIO.Heroes.Base
{
    [ExportHero(HeroId.npc_dota_hero_nevermore)]
    public class Nevermore : BaseHero
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

        public Ability RazeShort { get; private set; }

        public Ability RazeMedium { get; private set; }

        public Ability RazeLong { get; set; }

        public Ability Ulti { get; private set; }

        public MenuHoldKey UltiCombo;
        public MenuSwitcher DrawRazeRange;
        public MenuSwitcher PriRaze;
        public MenuSlider DrawingX;
        public MenuSlider DrawingY;
        public MenuToggleKey BkbToggle { get; private set; }
        public MenuHeroToggler EtherealHeroes;
        public MenuHeroToggler VeilOfDiscordHeroes;
        public MenuHeroToggler HexHeroes;
        public MenuHeroToggler OrchidBloodthornHeroes;
        public MenuHeroToggler AtosHeroes;
        public MenuHeroToggler HalberdHeroes;
        public MenuSwitcher UseBlink;
        public MenuSwitcher EullessUlti;

        public bool IsRazeModifier { get; set; }
        public string RazeModifierName { get; } = "modifier_nevermore_shadowraze_counter";
        public bool InUlti { get; set; }

        public List<Ability> Razes;
        public Vector3 PredictedTargetPosition { get; set; }

        public readonly List<string> EvadeAbilities = new List<string>()
        {
            "ursa_enrage",
            "nyx_assassin_spiked_carapace",
            "life_stealer_rage",
            "juggernaut_blade_fury"
        };
        public readonly List<string> EvadeItems = new List<string>()
        {
            "item_blade_mail",
            "item_black_king_bar",
            "item_cyclone",
        };

        protected override ComboMode GetComboMode()
        {
            return new NevermoreCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRazeRange = factory.CreateSwitcher("Draw Raze Indicators");
            this.DrawRazeRange.SetTooltip("When enabled, will draw raze indicators.");
            this.PriRaze = factory.CreateSwitcher("Make sure razes hit");
            this.PriRaze.SetTooltip(
                "When enabled, will stop razes if not guaranteed hit, but skip some attacks in return.");
            this.UltiCombo = factory.CreateHoldKey("Ulti Key");
            this.UltiCombo.SetTooltip("Will use ulti with available items on target.");
            this.EullessUlti = factory.CreateSwitcher("Use ulti without eul too", false);
            this.EullessUlti.SetTooltip("Will use ulti without euls too. Only activate with good ping/fps");
            this.DrawingX = factory.CreateSlider("Drawing X", 0, 0, 4000);
            this.DrawingY = factory.CreateSlider("Drawing Y", 0, 0, 4000);

            this.BkbToggle = itemMenu.CreateToggleKey("Bkb Toggle", Key.None, true);

            this.EtherealHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.VeilOfDiscordHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.HexHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.OrchidBloodthornHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.AtosHeroes = itemMenu.CreateHeroToggler("Hex", new());
            this.HalberdHeroes = itemMenu.CreateHeroToggler("Hex", new());

            this.UseBlink = itemMenu.CreateSwitcher("Use blink in combo?");

            UpdateManager.CreateIngameUpdate(20, OnUpdate);

            ModifierManager.ModifierAdded += this.OnRazeAdded;
            ModifierManager.ModifierRemoved += this.OnRazeRemoved;
            RendererManager.Draw += this.OnDraw;

            this.RazeShort = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_shadowraze1);
            this.RazeMedium = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_shadowraze2);
            this.RazeLong = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_shadowraze3);
            this.Ulti = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_requiem);
        }


        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            UpdateManager.DestroyIngameUpdate(OnUpdate);

            ModifierManager.ModifierAdded -= this.OnRazeAdded;
            ModifierManager.ModifierRemoved -= this.OnRazeRemoved;
            RendererManager.Draw -= this.OnDraw;
        }

        private protected override void OnMenuEnemyHeroChange(HeroId heroId, bool add)
        {
            if (add)
            {
                EtherealHeroes.AddValue(heroId, true);
                VeilOfDiscordHeroes.AddValue(heroId, true);
                HexHeroes.AddValue(heroId, false);
                OrchidBloodthornHeroes.AddValue(heroId, true);
                AtosHeroes.AddValue(heroId, true);
                HalberdHeroes.AddValue(heroId, true);
            }
            else
            {
                EtherealHeroes.RemoveValue(heroId);
                VeilOfDiscordHeroes.RemoveValue(heroId);
                HexHeroes.RemoveValue(heroId);
                OrchidBloodthornHeroes.RemoveValue(heroId);
                AtosHeroes.RemoveValue(heroId);
                HalberdHeroes.RemoveValue(heroId);
            }
        }

        private void OnUpdate()
        {
            if (RazeShort.Level <= 0)
            {
                return;
            }

            Razes = new List<Ability> { RazeShort, RazeMedium, RazeLong };

            if (DrawRazeRange == true)
            {
                var raze1 = UnitExtensions.InFront(Owner, this.RazeShort.GetAbilitySpecialData("shadowraze_range"));
                var raze2 = UnitExtensions.InFront(Owner, this.RazeMedium.GetAbilitySpecialData("shadowraze_range"));
                var raze3 = UnitExtensions.InFront(Owner, this.RazeLong.GetAbilitySpecialData("shadowraze_range"));

                var file = "materials/ensage_ui/particles/alert_range.vpcf";
                ParticleManager.CreateParticle(
                    $"DrawRange_{1}",
                    file,
                    Attachment.AbsOrigin,
                    Owner,
                    RestartType.None,
                    new(0, raze1),
                    new(1, 255, 0, 0),
                    new(2, 200, 255, 40));

                ParticleManager.CreateParticle(
                    $"DrawRange_{2}",
                    file,
                    Attachment.AbsOrigin,
                    Owner,
                    RestartType.None,
                    new(0, raze2),
                    new(1, 255, 0, 0),
                    new(2, 200, 255, 40));

                ParticleManager.CreateParticle(
                    $"DrawRange_{3}",
                    file,
                    Attachment.AbsOrigin,
                    Owner,
                    RestartType.None,
                    new(0, raze3),
                    new(1, 255, 0, 0),
                    new(2, 200, 255, 40));

                var targets = EntityManager.GetEntities<Hero>().FirstOrDefault(
                    x => x.IsAlive && x.Team != this.Owner.Team &&
                         x.Distance2D(this.Owner) < 1500 && !x.IsIllusion);


                if (targets != null && this.RazeShort.CanBeCasted() && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable() &&
                    CanHit(this.RazeShort, targets, GetRazeDelay(targets, RazeShort), true))
                {
                    ParticleManager.CreateParticle(
                        $"DrawRange_{1}",
                        file,
                        Attachment.AbsOrigin,
                        Owner,
                        RestartType.None,
                        new(0, raze1),
                        new(1, 0, 255, 0),
                        new(2, 200, 255, 40));
                }

                if (targets != null && this.RazeMedium.CanBeCasted() && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable() &&
                    CanHit(this.RazeMedium, targets, GetRazeDelay(targets, RazeMedium), true))
                {
                    ParticleManager.CreateParticle(
                        $"DrawRange_{2}",
                        file,
                        Attachment.AbsOrigin,
                        Owner,
                        RestartType.None,
                        new(0, raze2),
                        new(1, 0, 255, 0),
                        new(2, 200, 255, 40));
                }

                if (targets != null && this.RazeLong.CanBeCasted() && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable() &&
                    CanHit(this.RazeLong, targets, GetRazeDelay(targets, RazeLong), true))
                {
                    ParticleManager.CreateParticle(
                        $"DrawRange_{3}",
                        file,
                        Attachment.AbsOrigin,
                        Owner,
                        RestartType.None,
                        new(0, raze3),
                        new(1, 0, 255, 0),
                        new(2, 200, 255, 40));
                }
            }
            else
            {
                ParticleManager.DestroyParticle($"DrawRange_{1}");
                ParticleManager.DestroyParticle($"DrawRange_{2}");
                ParticleManager.DestroyParticle($"DrawRange_{3}");
            }
        }

        public bool CanHit(Ability ability, Unit targetsama, float customDelay, bool checkForFace = true)
        {
            var modifs = targetsama.HasModifiers(new string[] { "modifier_puck_phase_shift", "modifier_cyclone", "modifier_obsidian_destroyer_astral_imprisonment_prison" }, false);
            if (checkForFace && targetsama.IsValid && targetsama.IsAlive && !targetsama.IsInvulnerable() && !modifs)
            {
                var radius = ability.GetRadius();
                var range = ability.GetAbilitySpecialData("shadowraze_range");
                var predFontPos = UnitExtensions.InFront(Owner, range);
                var input = Input(targetsama, ability, range);
                var output = PredictionManager.GetPrediction(input);
                output.CastPosition = predFontPos;
                PredictedTargetPosition = output.UnitPosition;
                var hullRadius = targetsama.HullRadius;

                var inRange = PredictedTargetPosition.Distance2D(predFontPos) <= radius + hullRadius;

                return inRange;
            }
            else if (checkForFace && targetsama.IsValid && targetsama.IsAlive && !targetsama.IsInvulnerable() && !modifs)
            {
                var radius = ability.GetRadius();
                var range = ability.GetAbilitySpecialData("shadowraze_range");
                var predFontPos = UnitExtensions.InFront(Owner, range);
                var input = Input(targetsama, ability, range);
                var output = PredictionManager.GetPrediction(input);
                output.CastPosition = predFontPos;
                PredictedTargetPosition = output.UnitPosition;
                var hullRadius = targetsama.HullRadius;
                if (targetsama.IsMoving)
                {
                    hullRadius += 20;
                }
                var inRange = PredictedTargetPosition.Distance2D(predFontPos) <= radius + hullRadius;

                return inRange;
            }
            else
            {
                var radius = ability.GetAbilitySpecialData("shadowraze_radius");
                var range = ability.GetAbilitySpecialData("shadowraze_range");
                PredictedTargetPosition = targetsama.Position;
                var dist = PredictedTargetPosition.Distance2D(Owner.Position);
                var inRange = dist <= (range + radius + targetsama.HullRadius) &&
                              dist >= (range - radius - targetsama.HullRadius);

                return inRange;
            }
        }

        /*protected override async Task KillStealAsync(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || this.RazeShort.Level <= 0)
            {
                await Task.Delay(125, token);
                return;
            }

            try
            {
                var damage = this.RazeShort.GetAbilitySpecialData("shadowraze_damage");
                var stackDamage = this.RazeShort.GetAbilitySpecialData("stack_bonus_damage");

                var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
                    x => x.IsAlive && x.Team != this.Owner.Team && x.Distance2D(this.Owner) < 900 && !x.IsIllusion);



                if (killstealTarget != null)
                {
                    var modifier = killstealTarget.FindModifier(RazeModifierName);

                    if (modifier != null && IsRazeModifier)
                    {
                        damage += modifier.StackCount * stackDamage;
                    }

                }

                var talent = Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner,
                    AbilityId.special_bonus_unique_nevermore_2);
                if (talent?.Level > 0)
                {
                    damage += 150;
                }
                // todo: fix
                if (killstealTarget != null)
                {
                    damage *= 1 - killstealTarget.MagicDamageResist;
                    if (killstealTarget.Health < damage && this.Owner.CanCast())
                    {
                        this.Owner.MoveToDirection(killstealTarget.Position);

                        foreach (
                            var raze in
                            Razes.OrderByDescending(
                                x =>
                                    Ensage.SDK.Extensions.EntityExtensions.Distance2D(killstealTarget,
                                        UnitExtensions.InFront(Owner, x.GetCastRange()))))
                        {
                            if (raze != null && killstealTarget.IsAlive && raze.CanBeCasted() &&
                                CanHit(raze, killstealTarget, GetRazeDelay(killstealTarget, raze)))
                            {
                                raze.Cast();
                                var castDelay = GetRazeDelay(killstealTarget, raze);
                                await this.AwaitKillstealDelay(castDelay + 150, token);
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                LogManager.Debug(e);
            }
        }*/

        private void OnRazeAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if ((sender is Hero) && this.Owner.Team != sender.Team &&
                (modifier.Name == "modifier_nevermore_shadowraze_counter"))
            {
                //LogManager.Debug($"Raze modifier added");
                this.IsRazeModifier = true;
            }
        }

        private void OnRazeRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            var sender = modifier.Owner;
            if (this.Owner.Team != sender.Team && (modifier.Name == "modifier_nevermore_shadowraze_counter"))
            {
                this.IsRazeModifier = false;
            }
        }

        private void OnDraw()
        {
            if (this.BlackKingBar == null)
            {
                return;
            }

            var drawPos = new Vector2(DrawingX, DrawingY);
            var mousePos = GameManager.MousePosition;
            var value = BkbToggle.Value;

            var screenPos = RendererManager.WorldToScreen(mousePos, true);
            if (!screenPos.IsZero)
            {
                RendererManager.DrawText($"Bkb Toggle: {value}", drawPos, Color.Red, 30);
            }
        }

        public PredictionInput Input(Unit predTarget, Ability predAbility, float castRange)
        {
            return new PredictionInput(Owner, predTarget, GetRazeDelay(predTarget, predAbility) / 1000f,
                float.MaxValue, predAbility.GetAbilitySpecialData("shadowraze_range"), 250, PredictionSkillshotType.SkillshotCircle, true, null, true);
        }


        public virtual int GetRazeDelay(Unit targetto, Ability ability)
        {
            return
                (int)
                (((ability.AbilityData.GetCastPoint(ability.Level - 1) + Owner.TurnTime(targetto.Position)) * 1000.0f) +
                 GameManager.Ping);
        }
    }
}