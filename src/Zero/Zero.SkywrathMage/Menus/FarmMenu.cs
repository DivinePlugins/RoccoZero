using Divine.Core.ComboFactory.Menus;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class FarmMenu : BaseFarmMenu
    {
        [Value("Arcane Bolt & Attack", "Attack")]
        public override MenuSelector FarmItem { get; set; }

        [Value("Attack", "Arcane Bolt & Attack", "Disable")]
        public override MenuSelector HeroHarrasItem { get; set; }
    }
}
