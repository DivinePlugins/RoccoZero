using Divine.Input;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus
{
    public class BaseEscapeMenu
    {
        [Item("Escape Hotkey")]
        [Value(Key.None)]
        public MenuHoldKey EscapeHotkeyItem { get; set; }
    }
}
