//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_antimage;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.PredictionManager.Collision;
//using Ensage.SDK.Renderer.Particle;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;
//using HeroExtensions = Ensage.Common.Extensions.HeroExtensions;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_slark)]
//    internal class Slark : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public Ability DarkPact { get; private set; }

//        public Ability Pounce { get; private set; }

//        public Ability ShadowDance { get; private set; }

//        #endregion

//        #region Items


//        [ItemBinding]
//        public item_abyssal_blade AbyssalBlade { get; private set; }

//        [ItemBinding]
//        public item_manta Manta { get; private set; }

//        [ItemBinding]
//        public item_nullifier Nullifier { get; private set; }

//        [ItemBinding]
//        public item_cyclone Euls { get; private set; }

//        [ItemBinding]
//        public item_diffusal_blade DiffusalBlade { get; private set; }

//        [ItemBinding]
//        public item_invis_sword ShadowBlade { get; private set; }

//        [ItemBinding]
//        public item_silver_edge SilverEdge { get; private set; }

//        [ItemBinding]
//        public item_blink BlinkDagger { get; private set; }

//        [ItemBinding]
//        public item_bloodthorn BloodThorn { get; private set; }

//        [ItemBinding]
//        public item_black_king_bar BlackKingBar { get; set; }

//        [ItemBinding]
//        public item_orchid Orchid { get; private set; }

//        [ItemBinding]
//        public item_mjollnir Mjollnir { get; private set; }

//        [ItemBinding]
//        public item_force_staff ForceStaff { get; private set; }

//        [ItemBinding]
//        public item_ethereal_blade EtherealBlade { get; private set; }

//        [ItemBinding]
//        public item_veil_of_discord VeilOfDiscord { get; private set; }

//        [ItemBinding]
//        public item_shivas_guard ShivasGuard { get; private set; }

//        [ItemBinding]
//        public item_sheepstick Sheepstick { get; private set; }

//        [ItemBinding]
//        public item_rod_of_atos RodOfAtos { get; private set; }

//        [ItemBinding]
//        public item_urn_of_shadows Urn { get; private set; }

//        [ItemBinding]
//        public item_spirit_vessel Vessel { get; private set; }

//        [ItemBinding]
//        public item_lotus_orb Lotus { get; private set; }

//        [ItemBinding]
//        public item_solar_crest SolarCrest { get; private set; }

//        [ItemBinding]
//        public item_blade_mail BladeMail { get; private set; }

//        [ItemBinding]
//        public item_medallion_of_courage Medallion { get; private set; }

//        [ItemBinding]
//        public item_heavens_halberd HeavensHalberd { get; private set; }

//        [ItemBinding]
//        public item_satanic Satanic { get; private set; }

//        [ItemBinding]
//        public item_mask_of_madness Mom { get; private set; }

//        [ItemBinding]
//        public item_power_treads Treads { get; private set; }

//        [ItemBinding]
//        public item_armlet Armlet { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler AbyssalBladeHeroes;
//        public MenuHeroToggler BloodthornOrchidHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler InvisHeroes;
//        public MenuSlider MinimumUltiHp;
//        public MenuSwitcher MinimumUltiToggle;
//        public MenuSwitcher DrawPouncePosition;
//        #endregion

//        private IParticleManager particle;

//        protected override ComboMode GetComboMode()
//        {
//            return new SlarkCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.DarkPact = this.Owner.GetAbilityById(AbilityId.slark_dark_pact);
//            this.Pounce = this.Owner.GetAbilityById(AbilityId.slark_pounce);
//            this.ShadowDance = this.Owner.GetAbilityById(AbilityId.slark_shadow_dance);

//            UpdateManager.CreateIngameUpdate(OnPounce, 20);

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.particle = this.Context.Particle;

//            this.MinimumUltiToggle = factory.Item("Use Ulti?", true);
//            this.MinimumUltiHp = factory.Item("Minimum HP% to Ulti", new Slider(30, 0, 100));
//            this.DrawPouncePosition = factory.Item("Draw Pounce position", true);

//            this.BloodthornOrchidHeroes = itemMenu.Item("Bloodthorn/Orchid",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.InvisHeroes = itemMenu.Item("ShadowBlade / SilverEdge",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//        }

//        protected override void OnDeactivate()
//        {
//            base.OnDeactivate();
//            UpdateManager.DestroyIngameUpdate(OnPounce);
//        }

//        private void OnPounce()
//        {
//            if (Pounce.Level <= 0)
//            {
//                return;
//            }

//            if (DrawPouncePosition == true)
//            {
//                var pounceCastPos = UnitExtensions.InFront(Owner, PounceDistance + PounceRadius);
//                var red = new Vector3(255, 0, 0);
//                var green = new Vector3(0, 255, 0);
//                const string file = "materials/ensage_ui/particles/rectangle.vpcf";
//                particle.AddOrUpdate(Owner, $"DrawLine_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 1, this.Owner.InFront(0), 2, pounceCastPos,
//                    3, new Vector3(95, 100, 0), 4, red);

//                var targets = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                    x => x.IsAlive && x.Team != this.Owner.Team &&
//                         x.Distance2D(this.Owner) < 1500 && !x.IsIllusion);

//                if (targets != null && AbilityExtensions.CanBeCasted(this.Pounce) && this.Pounce != null
//                    && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable())
//                {
//                    var vectorOfMovement = new Vector2((float)Math.Cos(targets.RotationRad) * targets.MovementSpeed, (float)Math.Sin(targets.RotationRad) * targets.MovementSpeed);
//                    var hitPosition = Intercepts(targets.Position, vectorOfMovement, this.Owner.Position, 933.33f);
//                    var hitPosMod = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(this.Owner, hitPosition)), vectorOfMovement.Y * (TimeToTurn(this.Owner, hitPosition)), 0);
//                    var hitPosMod2 = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(this.Owner, hitPosMod)), vectorOfMovement.Y * (TimeToTurn(this.Owner, hitPosMod)), 0);

//                    if (this.Owner.Distance2D(hitPosMod2) > ((PounceDistance + PounceRadius) + targets.HullRadius))
//                    {
//                        return;
//                    }

//                    if (IsFacing(this.Owner, targets))
//                    {
//                        particle.AddOrUpdate(Owner, $"DrawLine_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 1, this.Owner.InFront(0), 2, pounceCastPos,
//                    3, new Vector3(95, 100, 0), 4, green);
//                    }
//                }
//            }
//            else
//            {
//                particle.Remove($"DrawLine_{1}");
//            }
//        }

//        /*public PredictionOutput Output(Unit predTarget, Ability predAbility)
//        {
//            var input = new PredictionInput(Owner, predTarget, GetJumpDelay(predTarget, predAbility) / 1000f,
//                933.33f, 795, 95, PredictionSkillshotType.SkillshotLine, false, null, true)
//            {
//                CollisionTypes = CollisionTypes.EnemyHeroes
//            };

//            return PredictionManager.GetPrediction(input);
//        }*/

//        public float PounceRadius
//        {
//            get
//            {
//                return this.Pounce.GetAbilitySpecialData("pounce_radius");
//            }
//        }

//        public float PounceDistance
//        {
//            get
//            {
//                return HeroExtensions.AghanimState(this.Owner)
//                    ? this.Pounce.GetAbilitySpecialData("pounce_distance_scepter")
//                    : this.Pounce.GetAbilitySpecialData("pounce_distance");
//            }
//        }

//        public float PounceSpeed
//        {
//            get
//            {
//                return HeroExtensions.AghanimState(this.Owner)
//                    ? this.Pounce.GetAbilitySpecialData("pounce_speed") * 2
//                    : this.Pounce.GetAbilitySpecialData("pounce_speed");
//            }
//        }

//        public Vector3 Intercepts(Vector3 x, Vector2 y, Vector3 z, float s)
//        {
//            var x1 = x.X - z.X;
//            var y1 = x.Y - z.Y;

//            var hs = y.X * y.X + y.Y * y.Y - s * s;
//            var h1 = x1 * y.X + y1 * y.Y;
//            float t;

//            const double tolerance = 0;
//            if (Math.Abs(hs) < tolerance)
//            {
//                t = -(x1 * x1 + y1 * y1) / 2 * h1;
//            }
//            else
//            {
//                var mp = -h1 / hs;
//                var d = mp * mp - (x1 * x1 + y1 * y1) / hs;

//                var root = (float)Math.Sqrt(d);

//                var t1 = mp + root;
//                var t2 = mp - root;

//                var tMin = Math.Min(t1, t2);
//                var tMax = Math.Max(t1, t2);

//                t = tMin > 0 ? tMin : tMax;
//            }
//            return new Vector3(x.X + t * y.X, x.Y + t * y.Y, x.Z);
//        }

//        public bool IsFacing(Entity startUnit, dynamic enemy)
//        {
//            if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
//            if (enemy is Unit) enemy = enemy.Position;

//            float deltaY = startUnit.Position.Y - enemy.Y;
//            float deltaX = startUnit.Position.X - enemy.X;
//            var angle = (float)(Math.Atan2(deltaY, deltaX));

//            var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
//            var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

//            return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.1;
//        }

//        public float TimeToTurn(Entity startUnit, dynamic enemy)
//        {
//            if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
//            if (enemy is Unit) enemy = enemy.Position;

//            const double turnRate = 0.5;

//            float deltaY = startUnit.Position.Y - enemy.Y;
//            float deltaX = startUnit.Position.X - enemy.X;
//            var angle = (float)(Math.Atan2(deltaY, deltaX));

//            var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
//            var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

//            var calc = (float)(Math.PI - Math.Abs(Math.Atan2(n1, n2)));

//            if (calc < 0.1 && calc > -0.1) return 0;

//            return (float)(calc * (0.03 / turnRate));
//        }

//        /*public virtual int GetJumpDelay(Unit targetto, Ability ability)
//        {
//            return
//                (int)
//                (((ability.AbilityData.GetCastPoint(ability.Level - 1) + Owner.TurnTime(targetto.Position)) * 1000.0f) +
//                 GameManager.Ping);
//        }*/
//    }
//}//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_antimage;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.PredictionManager.Collision;
//using Ensage.SDK.Renderer.Particle;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;
//using HeroExtensions = Ensage.Common.Extensions.HeroExtensions;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_slark)]
//    internal class Slark : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public Ability DarkPact { get; private set; }

//        public Ability Pounce { get; private set; }

//        public Ability ShadowDance { get; private set; }

//        #endregion

//        #region Items


//        [ItemBinding]
//        public item_abyssal_blade AbyssalBlade { get; private set; }

//        [ItemBinding]
//        public item_manta Manta { get; private set; }

//        [ItemBinding]
//        public item_nullifier Nullifier { get; private set; }

//        [ItemBinding]
//        public item_cyclone Euls { get; private set; }

//        [ItemBinding]
//        public item_diffusal_blade DiffusalBlade { get; private set; }

//        [ItemBinding]
//        public item_invis_sword ShadowBlade { get; private set; }

//        [ItemBinding]
//        public item_silver_edge SilverEdge { get; private set; }

//        [ItemBinding]
//        public item_blink BlinkDagger { get; private set; }

//        [ItemBinding]
//        public item_bloodthorn BloodThorn { get; private set; }

//        [ItemBinding]
//        public item_black_king_bar BlackKingBar { get; set; }

//        [ItemBinding]
//        public item_orchid Orchid { get; private set; }

//        [ItemBinding]
//        public item_mjollnir Mjollnir { get; private set; }

//        [ItemBinding]
//        public item_force_staff ForceStaff { get; private set; }

//        [ItemBinding]
//        public item_ethereal_blade EtherealBlade { get; private set; }

//        [ItemBinding]
//        public item_veil_of_discord VeilOfDiscord { get; private set; }

//        [ItemBinding]
//        public item_shivas_guard ShivasGuard { get; private set; }

//        [ItemBinding]
//        public item_sheepstick Sheepstick { get; private set; }

//        [ItemBinding]
//        public item_rod_of_atos RodOfAtos { get; private set; }

//        [ItemBinding]
//        public item_urn_of_shadows Urn { get; private set; }

//        [ItemBinding]
//        public item_spirit_vessel Vessel { get; private set; }

//        [ItemBinding]
//        public item_lotus_orb Lotus { get; private set; }

//        [ItemBinding]
//        public item_solar_crest SolarCrest { get; private set; }

//        [ItemBinding]
//        public item_blade_mail BladeMail { get; private set; }

//        [ItemBinding]
//        public item_medallion_of_courage Medallion { get; private set; }

//        [ItemBinding]
//        public item_heavens_halberd HeavensHalberd { get; private set; }

//        [ItemBinding]
//        public item_satanic Satanic { get; private set; }

//        [ItemBinding]
//        public item_mask_of_madness Mom { get; private set; }

//        [ItemBinding]
//        public item_power_treads Treads { get; private set; }

//        [ItemBinding]
//        public item_armlet Armlet { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler AbyssalBladeHeroes;
//        public MenuHeroToggler BloodthornOrchidHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler InvisHeroes;
//        public MenuSlider MinimumUltiHp;
//        public MenuSwitcher MinimumUltiToggle;
//        public MenuSwitcher DrawPouncePosition;
//        #endregion

//        private IParticleManager particle;

//        protected override ComboMode GetComboMode()
//        {
//            return new SlarkCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.DarkPact = this.Owner.GetAbilityById(AbilityId.slark_dark_pact);
//            this.Pounce = this.Owner.GetAbilityById(AbilityId.slark_pounce);
//            this.ShadowDance = this.Owner.GetAbilityById(AbilityId.slark_shadow_dance);

//            UpdateManager.CreateIngameUpdate(OnPounce, 20);

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.particle = this.Context.Particle;

//            this.MinimumUltiToggle = factory.Item("Use Ulti?", true);
//            this.MinimumUltiHp = factory.Item("Minimum HP% to Ulti", new Slider(30, 0, 100));
//            this.DrawPouncePosition = factory.Item("Draw Pounce position", true);

//            this.BloodthornOrchidHeroes = itemMenu.Item("Bloodthorn/Orchid",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.InvisHeroes = itemMenu.Item("ShadowBlade / SilverEdge",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//        }

//        protected override void OnDeactivate()
//        {
//            base.OnDeactivate();
//            UpdateManager.DestroyIngameUpdate(OnPounce);
//        }

//        private void OnPounce()
//        {
//            if (Pounce.Level <= 0)
//            {
//                return;
//            }

//            if (DrawPouncePosition == true)
//            {
//                var pounceCastPos = UnitExtensions.InFront(Owner, PounceDistance + PounceRadius);
//                var red = new Vector3(255, 0, 0);
//                var green = new Vector3(0, 255, 0);
//                const string file = "materials/ensage_ui/particles/rectangle.vpcf";
//                particle.AddOrUpdate(Owner, $"DrawLine_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 1, this.Owner.InFront(0), 2, pounceCastPos,
//                    3, new Vector3(95, 100, 0), 4, red);

//                var targets = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                    x => x.IsAlive && x.Team != this.Owner.Team &&
//                         x.Distance2D(this.Owner) < 1500 && !x.IsIllusion);

//                if (targets != null && AbilityExtensions.CanBeCasted(this.Pounce) && this.Pounce != null
//                    && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable())
//                {
//                    var vectorOfMovement = new Vector2((float)Math.Cos(targets.RotationRad) * targets.MovementSpeed, (float)Math.Sin(targets.RotationRad) * targets.MovementSpeed);
//                    var hitPosition = Intercepts(targets.Position, vectorOfMovement, this.Owner.Position, 933.33f);
//                    var hitPosMod = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(this.Owner, hitPosition)), vectorOfMovement.Y * (TimeToTurn(this.Owner, hitPosition)), 0);
//                    var hitPosMod2 = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(this.Owner, hitPosMod)), vectorOfMovement.Y * (TimeToTurn(this.Owner, hitPosMod)), 0);

//                    if (this.Owner.Distance2D(hitPosMod2) > ((PounceDistance + PounceRadius) + targets.HullRadius))
//                    {
//                        return;
//                    }

//                    if (IsFacing(this.Owner, targets))
//                    {
//                        particle.AddOrUpdate(Owner, $"DrawLine_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 1, this.Owner.InFront(0), 2, pounceCastPos,
//                    3, new Vector3(95, 100, 0), 4, green);
//                    }
//                }
//            }
//            else
//            {
//                particle.Remove($"DrawLine_{1}");
//            }
//        }

//        /*public PredictionOutput Output(Unit predTarget, Ability predAbility)
//        {
//            var input = new PredictionInput(Owner, predTarget, GetJumpDelay(predTarget, predAbility) / 1000f,
//                933.33f, 795, 95, PredictionSkillshotType.SkillshotLine, false, null, true)
//            {
//                CollisionTypes = CollisionTypes.EnemyHeroes
//            };

//            return PredictionManager.GetPrediction(input);
//        }*/

//        public float PounceRadius
//        {
//            get
//            {
//                return this.Pounce.GetAbilitySpecialData("pounce_radius");
//            }
//        }

//        public float PounceDistance
//        {
//            get
//            {
//                return HeroExtensions.AghanimState(this.Owner)
//                    ? this.Pounce.GetAbilitySpecialData("pounce_distance_scepter")
//                    : this.Pounce.GetAbilitySpecialData("pounce_distance");
//            }
//        }

//        public float PounceSpeed
//        {
//            get
//            {
//                return HeroExtensions.AghanimState(this.Owner)
//                    ? this.Pounce.GetAbilitySpecialData("pounce_speed") * 2
//                    : this.Pounce.GetAbilitySpecialData("pounce_speed");
//            }
//        }

//        public Vector3 Intercepts(Vector3 x, Vector2 y, Vector3 z, float s)
//        {
//            var x1 = x.X - z.X;
//            var y1 = x.Y - z.Y;

//            var hs = y.X * y.X + y.Y * y.Y - s * s;
//            var h1 = x1 * y.X + y1 * y.Y;
//            float t;

//            const double tolerance = 0;
//            if (Math.Abs(hs) < tolerance)
//            {
//                t = -(x1 * x1 + y1 * y1) / 2 * h1;
//            }
//            else
//            {
//                var mp = -h1 / hs;
//                var d = mp * mp - (x1 * x1 + y1 * y1) / hs;

//                var root = (float)Math.Sqrt(d);

//                var t1 = mp + root;
//                var t2 = mp - root;

//                var tMin = Math.Min(t1, t2);
//                var tMax = Math.Max(t1, t2);

//                t = tMin > 0 ? tMin : tMax;
//            }
//            return new Vector3(x.X + t * y.X, x.Y + t * y.Y, x.Z);
//        }

//        public bool IsFacing(Entity startUnit, dynamic enemy)
//        {
//            if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
//            if (enemy is Unit) enemy = enemy.Position;

//            float deltaY = startUnit.Position.Y - enemy.Y;
//            float deltaX = startUnit.Position.X - enemy.X;
//            var angle = (float)(Math.Atan2(deltaY, deltaX));

//            var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
//            var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

//            return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.1;
//        }

//        public float TimeToTurn(Entity startUnit, dynamic enemy)
//        {
//            if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
//            if (enemy is Unit) enemy = enemy.Position;

//            const double turnRate = 0.5;

//            float deltaY = startUnit.Position.Y - enemy.Y;
//            float deltaX = startUnit.Position.X - enemy.X;
//            var angle = (float)(Math.Atan2(deltaY, deltaX));

//            var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
//            var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

//            var calc = (float)(Math.PI - Math.Abs(Math.Atan2(n1, n2)));

//            if (calc < 0.1 && calc > -0.1) return 0;

//            return (float)(calc * (0.03 / turnRate));
//        }

//        /*public virtual int GetJumpDelay(Unit targetto, Ability ability)
//        {
//            return
//                (int)
//                (((ability.AbilityData.GetCastPoint(ability.Level - 1) + Owner.TurnTime(targetto.Position)) * 1000.0f) +
//                 GameManager.Ping);
//        }*/
//    }
//}//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Abilities.npc_dota_hero_antimage;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using Ensage.SDK.Prediction;
//using Ensage.SDK.PredictionManager.Collision;
//using Ensage.SDK.Renderer.Particle;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;
//using HeroExtensions = Ensage.Common.Extensions.HeroExtensions;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_slark)]
//    internal class Slark : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public Ability DarkPact { get; private set; }

//        public Ability Pounce { get; private set; }

//        public Ability ShadowDance { get; private set; }

//        #endregion

//        #region Items


//        [ItemBinding]
//        public item_abyssal_blade AbyssalBlade { get; private set; }

//        [ItemBinding]
//        public item_manta Manta { get; private set; }

//        [ItemBinding]
//        public item_nullifier Nullifier { get; private set; }

//        [ItemBinding]
//        public item_cyclone Euls { get; private set; }

//        [ItemBinding]
//        public item_diffusal_blade DiffusalBlade { get; private set; }

//        [ItemBinding]
//        public item_invis_sword ShadowBlade { get; private set; }

//        [ItemBinding]
//        public item_silver_edge SilverEdge { get; private set; }

//        [ItemBinding]
//        public item_blink BlinkDagger { get; private set; }

//        [ItemBinding]
//        public item_bloodthorn BloodThorn { get; private set; }

//        [ItemBinding]
//        public item_black_king_bar BlackKingBar { get; set; }

//        [ItemBinding]
//        public item_orchid Orchid { get; private set; }

//        [ItemBinding]
//        public item_mjollnir Mjollnir { get; private set; }

//        [ItemBinding]
//        public item_force_staff ForceStaff { get; private set; }

//        [ItemBinding]
//        public item_ethereal_blade EtherealBlade { get; private set; }

//        [ItemBinding]
//        public item_veil_of_discord VeilOfDiscord { get; private set; }

//        [ItemBinding]
//        public item_shivas_guard ShivasGuard { get; private set; }

//        [ItemBinding]
//        public item_sheepstick Sheepstick { get; private set; }

//        [ItemBinding]
//        public item_rod_of_atos RodOfAtos { get; private set; }

//        [ItemBinding]
//        public item_urn_of_shadows Urn { get; private set; }

//        [ItemBinding]
//        public item_spirit_vessel Vessel { get; private set; }

//        [ItemBinding]
//        public item_lotus_orb Lotus { get; private set; }

//        [ItemBinding]
//        public item_solar_crest SolarCrest { get; private set; }

//        [ItemBinding]
//        public item_blade_mail BladeMail { get; private set; }

//        [ItemBinding]
//        public item_medallion_of_courage Medallion { get; private set; }

//        [ItemBinding]
//        public item_heavens_halberd HeavensHalberd { get; private set; }

//        [ItemBinding]
//        public item_satanic Satanic { get; private set; }

//        [ItemBinding]
//        public item_mask_of_madness Mom { get; private set; }

//        [ItemBinding]
//        public item_power_treads Treads { get; private set; }

//        [ItemBinding]
//        public item_armlet Armlet { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuHeroToggler AbyssalBladeHeroes;
//        public MenuHeroToggler BloodthornOrchidHeroes;
//        public MenuHeroToggler NullifierHeroes;
//        public MenuHeroToggler InvisHeroes;
//        public MenuSlider MinimumUltiHp;
//        public MenuSwitcher MinimumUltiToggle;
//        public MenuSwitcher DrawPouncePosition;
//        #endregion

//        private IParticleManager particle;

//        protected override ComboMode GetComboMode()
//        {
//            return new SlarkCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.DarkPact = this.Owner.GetAbilityById(AbilityId.slark_dark_pact);
//            this.Pounce = this.Owner.GetAbilityById(AbilityId.slark_pounce);
//            this.ShadowDance = this.Owner.GetAbilityById(AbilityId.slark_shadow_dance);

//            UpdateManager.CreateIngameUpdate(OnPounce, 20);

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.particle = this.Context.Particle;

//            this.MinimumUltiToggle = factory.Item("Use Ulti?", true);
//            this.MinimumUltiHp = factory.Item("Minimum HP% to Ulti", new Slider(30, 0, 100));
//            this.DrawPouncePosition = factory.Item("Draw Pounce position", true);

//            this.BloodthornOrchidHeroes = itemMenu.Item("Bloodthorn/Orchid",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.InvisHeroes = itemMenu.Item("ShadowBlade / SilverEdge",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//        }

//        protected override void OnDeactivate()
//        {
//            base.OnDeactivate();
//            UpdateManager.DestroyIngameUpdate(OnPounce);
//        }

//        private void OnPounce()
//        {
//            if (Pounce.Level <= 0)
//            {
//                return;
//            }

//            if (DrawPouncePosition == true)
//            {
//                var pounceCastPos = UnitExtensions.InFront(Owner, PounceDistance + PounceRadius);
//                var red = new Vector3(255, 0, 0);
//                var green = new Vector3(0, 255, 0);
//                const string file = "materials/ensage_ui/particles/rectangle.vpcf";
//                particle.AddOrUpdate(Owner, $"DrawLine_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 1, this.Owner.InFront(0), 2, pounceCastPos,
//                    3, new Vector3(95, 100, 0), 4, red);

//                var targets = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                    x => x.IsAlive && x.Team != this.Owner.Team &&
//                         x.Distance2D(this.Owner) < 1500 && !x.IsIllusion);

//                if (targets != null && AbilityExtensions.CanBeCasted(this.Pounce) && this.Pounce != null
//                    && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable())
//                {
//                    var vectorOfMovement = new Vector2((float)Math.Cos(targets.RotationRad) * targets.MovementSpeed, (float)Math.Sin(targets.RotationRad) * targets.MovementSpeed);
//                    var hitPosition = Intercepts(targets.Position, vectorOfMovement, this.Owner.Position, 933.33f);
//                    var hitPosMod = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(this.Owner, hitPosition)), vectorOfMovement.Y * (TimeToTurn(this.Owner, hitPosition)), 0);
//                    var hitPosMod2 = hitPosition + new Vector3(vectorOfMovement.X * (TimeToTurn(this.Owner, hitPosMod)), vectorOfMovement.Y * (TimeToTurn(this.Owner, hitPosMod)), 0);

//                    if (this.Owner.Distance2D(hitPosMod2) > ((PounceDistance + PounceRadius) + targets.HullRadius))
//                    {
//                        return;
//                    }

//                    if (IsFacing(this.Owner, targets))
//                    {
//                        particle.AddOrUpdate(Owner, $"DrawLine_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 1, this.Owner.InFront(0), 2, pounceCastPos,
//                    3, new Vector3(95, 100, 0), 4, green);
//                    }
//                }
//            }
//            else
//            {
//                particle.Remove($"DrawLine_{1}");
//            }
//        }

//        /*public PredictionOutput Output(Unit predTarget, Ability predAbility)
//        {
//            var input = new PredictionInput(Owner, predTarget, GetJumpDelay(predTarget, predAbility) / 1000f,
//                933.33f, 795, 95, PredictionSkillshotType.SkillshotLine, false, null, true)
//            {
//                CollisionTypes = CollisionTypes.EnemyHeroes
//            };

//            return PredictionManager.GetPrediction(input);
//        }*/

//        public float PounceRadius
//        {
//            get
//            {
//                return this.Pounce.GetAbilitySpecialData("pounce_radius");
//            }
//        }

//        public float PounceDistance
//        {
//            get
//            {
//                return HeroExtensions.AghanimState(this.Owner)
//                    ? this.Pounce.GetAbilitySpecialData("pounce_distance_scepter")
//                    : this.Pounce.GetAbilitySpecialData("pounce_distance");
//            }
//        }

//        public float PounceSpeed
//        {
//            get
//            {
//                return HeroExtensions.AghanimState(this.Owner)
//                    ? this.Pounce.GetAbilitySpecialData("pounce_speed") * 2
//                    : this.Pounce.GetAbilitySpecialData("pounce_speed");
//            }
//        }

//        public Vector3 Intercepts(Vector3 x, Vector2 y, Vector3 z, float s)
//        {
//            var x1 = x.X - z.X;
//            var y1 = x.Y - z.Y;

//            var hs = y.X * y.X + y.Y * y.Y - s * s;
//            var h1 = x1 * y.X + y1 * y.Y;
//            float t;

//            const double tolerance = 0;
//            if (Math.Abs(hs) < tolerance)
//            {
//                t = -(x1 * x1 + y1 * y1) / 2 * h1;
//            }
//            else
//            {
//                var mp = -h1 / hs;
//                var d = mp * mp - (x1 * x1 + y1 * y1) / hs;

//                var root = (float)Math.Sqrt(d);

//                var t1 = mp + root;
//                var t2 = mp - root;

//                var tMin = Math.Min(t1, t2);
//                var tMax = Math.Max(t1, t2);

//                t = tMin > 0 ? tMin : tMax;
//            }
//            return new Vector3(x.X + t * y.X, x.Y + t * y.Y, x.Z);
//        }

//        public bool IsFacing(Entity startUnit, dynamic enemy)
//        {
//            if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
//            if (enemy is Unit) enemy = enemy.Position;

//            float deltaY = startUnit.Position.Y - enemy.Y;
//            float deltaX = startUnit.Position.X - enemy.X;
//            var angle = (float)(Math.Atan2(deltaY, deltaX));

//            var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
//            var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

//            return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.1;
//        }

//        public float TimeToTurn(Entity startUnit, dynamic enemy)
//        {
//            if (!(enemy is Unit || enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", nameof(enemy));
//            if (enemy is Unit) enemy = enemy.Position;

//            const double turnRate = 0.5;

//            float deltaY = startUnit.Position.Y - enemy.Y;
//            float deltaX = startUnit.Position.X - enemy.X;
//            var angle = (float)(Math.Atan2(deltaY, deltaX));

//            var n1 = (float)Math.Sin(startUnit.RotationRad - angle);
//            var n2 = (float)Math.Cos(startUnit.RotationRad - angle);

//            var calc = (float)(Math.PI - Math.Abs(Math.Atan2(n1, n2)));

//            if (calc < 0.1 && calc > -0.1) return 0;

//            return (float)(calc * (0.03 / turnRate));
//        }

//        /*public virtual int GetJumpDelay(Unit targetto, Ability ability)
//        {
//            return
//                (int)
//                (((ability.AbilityData.GetCastPoint(ability.Level - 1) + Owner.TurnTime(targetto.Position)) * 1000.0f) +
//                 GameManager.Ping);
//        }*/
//    }
//}
