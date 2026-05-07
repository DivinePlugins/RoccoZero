using Divine.Input;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public class BaseFarmMenu
    {
        [Item("Farm Hotkey")]
        [Value(Key.Space)]
        public MenuHoldKey FarmHotkeyItem { get; set; }

        [Item("Farm:")]
        [Value("Attack")]
        [Priority(4)]
        public virtual MenuSelector FarmItem { get; set; }

        [Item("Hero Harras:")]
        [Value("Attack", "Disable")]
        [Priority(5)]
        public virtual MenuSelector HeroHarrasItem { get; set; }
    }
}
