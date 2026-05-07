using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.TargetSelector;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.SkywrathMage.Menus;

namespace Divine.SkywrathMage.Helpers
{
    internal sealed class UpdateMode : BaseUpdateHandler
    {
        private readonly BaseBlinkDaggerMenu BlinkDaggerMenu;

        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        private readonly BaseFarmMenu FarmMenu;

        private readonly RadiusMenu RadiusMenu;

        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        public CUnit UnitTarget { get; private set; }

        public UpdateMode(Common common)
        {
            BlinkDaggerMenu = common.MenuConfig.ComboMenu.BlinkDaggerMenu;
            SmartArcaneBoltMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartArcaneBoltMenu;
            FarmMenu = common.MenuConfig.FarmMenu;
            RadiusMenu = (RadiusMenu)common.MenuConfig.RadiusMenu;

            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;

            Run();
        }

        protected override void Execute()
        {
            var arcaneBolt = Abilities.ArcaneBolt;
            if (RadiusMenu.ArcaneBoltItem || FarmMenu.FarmHotkeyItem && arcaneBolt.Level > 0)
            {
                ParticleManager.CreateRangeParticle(
                    "ArcaneBolt",
                    Owner.Base,
                    arcaneBolt.CastRange,
                    arcaneBolt.IsReady ? (FarmMenu.FarmHotkeyItem ? Color.Yellow : Color.Aqua) : Color.Gray);
            }
            else
            {
                ParticleManager.DestroyParticle("ArcaneBolt");
            }

            var concussiveShot = Abilities.ConcussiveShot;
            if (RadiusMenu.ConcussiveShotItem && concussiveShot.Level > 0)
            {
                ParticleManager.CreateRangeParticle(
                    "ConcussiveShot",
                    Owner.Base,
                    concussiveShot.Radius,
                    concussiveShot.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.DestroyParticle("ConcussiveShot");
            }

            var ancientSeal = Abilities.AncientSeal;
            if (RadiusMenu.AncientSealItem && ancientSeal.Level > 0)
            {
                ParticleManager.CreateRangeParticle(
                    "AncientSeal",
                    Owner.Base,
                    ancientSeal.CastRange,
                    ancientSeal.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.DestroyParticle("AncientSeal");
            }

            var mysticFlare = Abilities.MysticFlare;
            if (RadiusMenu.MysticFlareItem && mysticFlare.Level > 0)
            {
                ParticleManager.CreateRangeParticle(
                    "MysticFlare",
                    Owner.Base,
                    mysticFlare.CastRange,
                    mysticFlare.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.DestroyParticle("MysticFlare");
            }

            var blink = Abilities.Blink;
            if (RadiusMenu.BlinkDaggerItem && blink != null)
            {
                var color = Color.Red;
                if (!blink.IsReady)
                {
                    color = Color.Gray;
                }
                else if (Owner.Distance2D(GameManager.MousePosition) > BlinkDaggerMenu.BlinkActivationItem.Value)
                {
                    color = Color.Aqua;
                }

                ParticleManager.CreateRangeParticle(
                    "Blink",
                    Owner.Base,
                    blink.CastRange,
                    color);
            }
            else
            {
                ParticleManager.DestroyParticle("Blink");
            }

            if (FarmMenu.FarmHotkeyItem)
            {
                ParticleManager.CreateRangeParticle(
                    "Farm",
                    Owner.Base,
                    Owner.AttackRange(Owner),
                    Color.Yellow);
            }
            else
            {
                ParticleManager.DestroyParticle("Farm");
            }

            var targetHit = concussiveShot.TargetHit;
            if (RadiusMenu.TargetHitConcussiveShotItem && targetHit != null && concussiveShot.Cooldown <= 1 && concussiveShot.Level > 0)
            {
                var position = targetHit.Position + new Vector3(0, 200, targetHit.HealthBarOffset);

                ParticleManager.CreateParticle(
                    "TargetHitConcussiveShot",
                    @"particles\units\heroes\hero_skywrath_mage\skywrath_mage_concussive_shot.vpcf",
                    Attachment.AbsOrigin,
                    targetHit.Base,
                    new ControlPoint(0, position),
                    new ControlPoint(1, position),
                    new ControlPoint(2, 1000));
            }
            else
            {
                ParticleManager.DestroyParticle("TargetHitConcussiveShot");
            }
        }
    }
}
