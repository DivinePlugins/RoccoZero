using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.TargetSelector;
using Divine.Core.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle;
using Divine.Zeus.Menus;

namespace Divine.Zeus.Helpers
{
    internal sealed class UpdateMode : BaseUpdateHandler
    {
        private readonly BaseBlinkDaggerMenu BlinkDaggerMenu;

        private readonly BaseFarmMenu FarmMenu;

        private readonly RadiusMenu RadiusMenu;

        private readonly BaseTargetSelectorMenu TargetSelectorMenu;

        private readonly Abilities Abilities;

        public UpdateMode(Common common)
        {
            BlinkDaggerMenu = common.MenuConfig.ComboMenu.BlinkDaggerMenu;
            FarmMenu = common.MenuConfig.FarmMenu;
            RadiusMenu = (RadiusMenu)common.MenuConfig.RadiusMenu;
            TargetSelectorMenu = common.MenuConfig.TargetSelectorMenu;

            Abilities = (Abilities)common.Abilities;

            Run();
        }

        protected override void Execute()
        {
            var arcLightning = Abilities.ArcLightning;
            if (RadiusMenu.ArcLightningItem || FarmMenu.FarmHotkeyItem && arcLightning.Level > 0)
            {
                ParticleManager.CreateRangeParticle(
                    "ArcLightning",
                    Owner.Base,
                    arcLightning.CastRange,
                    arcLightning.IsReady ? (FarmMenu.FarmHotkeyItem ? Color.Yellow : Color.Aqua) : Color.Gray);
            }
            else
            {
                ParticleManager.DestroyParticle("ArcLightning");
            }

            var lightningBolt = Abilities.LightningBolt;
            if (RadiusMenu.LightningBoltItem && lightningBolt.Level > 0)
            {
                ParticleManager.CreateRangeParticle(
                    "LightningBolt",
                    Owner.Base,
                    lightningBolt.CastRange,
                    lightningBolt.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.DestroyParticle("LightningBolt");
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

                ParticleManager.CreateRangeParticle("Blink", Owner.Base, blink.CastRange, color);
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
        }
    }
}
