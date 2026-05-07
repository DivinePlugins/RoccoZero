using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.SkywrathMage.Menus.Combo;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.SkywrathMage.Menus
{
    [Menu("Zero.SkywrathMage"), HeroImage(HeroId.npc_dota_hero_skywrath_mage)]
    internal sealed class MenuConfig : BaseMenuConfig
    {
        public override BaseComboMenu ComboMenu { get; } = new ComboMenu();

        public override BaseKillStealMenu KillStealMenu { get; } = new AutoKillStealMenu();

        public override BaseMoreMenu MoreMenu { get; } = new MoreMenu();

        public override BaseFarmMenu FarmMenu { get; } = new FarmMenu();

        public override BaseDisableMenu DisableMenu { get; } = new DisableMenu();

        public override BaseLinkenBreakerMenu LinkenBreakerMenu { get; } = new LinkenBreakerMenu();

        public override BaseBladeMailMenu BladeMailMenu { get; } = new BladeMailMenu();

        public override BaseRadiusMenu RadiusMenu { get; } = new RadiusMenu();
    }
}
