using Divine.Input;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public class BaseWithMuteMenu
    {
        [Item("Toggle Hotkey")]
        [Value(Key.None)]
        public MenuToggleKey ToggleHotkeyItem { get; set; }

        [Item("Combo Hotkey")]
        [Value(Key.None)]
        public MenuHoldKey ComboHotkey { get; set; }
    }
}
