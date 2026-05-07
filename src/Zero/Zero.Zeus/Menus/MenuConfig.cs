using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Zeus.Menus.Combo;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Zeus.Menus
{
    [Menu("Zero.Zeus"), HeroImage(HeroId.npc_dota_hero_zuus)]
    internal sealed class MenuConfig : BaseMenuConfig
    {
        public override BaseComboMenu ComboMenu { get; } = new ComboMenu();

        public override BaseKillStealMenu KillStealMenu { get; } = new KillStealMenu();

        public override BaseMoreMenu MoreMenu { get; } = new MoreMenu();

        public override BaseFarmMenu FarmMenu { get; } = new FarmMenu();

        public override BaseLinkenBreakerMenu LinkenBreakerMenu { get; } = new LinkenBreakerMenu();

        public override BaseRadiusMenu RadiusMenu { get; } = new RadiusMenu();
    }
}
