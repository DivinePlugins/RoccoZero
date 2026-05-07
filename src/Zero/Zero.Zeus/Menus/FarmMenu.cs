using Divine.Core.ComboFactory.Menus;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus
{
    internal sealed class FarmMenu : BaseFarmMenu
    {
        [Value("Arc Lightning & Attack", "Attack")]
        public override MenuSelector FarmItem { get; set; }

        [Value("Attack", "Arc Lightning & Attack", "Disable")]
        public override MenuSelector HeroHarrasItem { get; set; }
    }
}
